USE [ShopDirect]
GO

/****** Object:  StoredProcedure [dbo].[PopulateAnnualHealthCheck]    Script Date: 02/05/2019 14:39:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[PopulateAnnualHealthCheck] 
as	
BEGIN
SET NOCOUNT ON

INSERT INTO Annualhealthcheck
(
  CustAplId,
  TriggerId,
  TriggerValue,
  ContractDt,
  ContractStart,
  ContractExpires,
  RetailClientId,
  Extension,
  AnnualHealthDueDay
)
SELECT
  cap.CUSTAPLID,
  13,
  CAST(YEAR(getdate()) AS char(4)) + '.' + CAST(cap.CUSTAPLID AS char(15)) AS HealthID,
  cap.CONTRACTDT,
  cap.CONTRACTSTART,
  cap.CONTRACTEXPIRES,
  ctm.RetailClientID,
  0,
  DATEPART(dayofyear, cap.CONTRACTSTART)
FROM
  Custapl cap
  INNER JOIN Customer ctm ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 13 AND res.TRIGGERFIELDLAST = 'HealthID' AND res.TriggerValue = CAST(YEAR(GETDATE()) AS char(4)) + '.' + CAST(cap.CUSTAPLID AS char(15)) 
  LEFT JOIN Annualhealthcheck ann ON ann.TriggerValue = CAST(YEAR(GETDATE()) AS char(4)) + '.' + CAST(cap.CUSTAPLID AS char(15))
WHERE
  dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTSTART, '1900-01-01', DATEADD(day, -1, getdate())) = 1 --The contract has started
  AND DATEDIFF(month, cap.CONTRACTSTART, cap.CONTRACTEXPIRES) > 12 --New business, otherwise extension
  AND DATEDIFF(month, getdate(), cap.CONTRACTEXPIRES) > 2
  AND DATEPART(dayofyear, cap.CONTRACTSTART) = DATEPART(dayofyear, getdate())
  AND res.id IS NULL
  AND ann._id IS NULL
ORDER BY
  cap.CONTRACTSTART

        
               insert into Annualhealthcheck ([CustAplId]
           ,[TriggerId]
           ,[TriggerValue]
           ,[ContractDt]
           ,[ContractStart]
           ,[ContractExpires]
           ,[RetailClientId]
           ,[Extension]
           ,[AnnualHealthDueDay]) 
           SELECT  custapl.custaplid,49,CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15)) AS HealthID,  custapl.CONTRACTDT,custapl.CONTRACTSTART ,
custapl.CONTRACTEXPIRES, customer.RetailClientID,0, datepart(dayofyear,custapl.CONTRACTSTART)--,DATEDIFF(MONTH,GETDATE() ,custapl.CONTRACTEXPIRES)
 --,DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)
    --into #AnnualHealthCheckVery             
     FROM [CustApl]    
                  join [Customer] on Customer.CUSTOMERID=ISNULL(ownercustomerid,custapl.customerid)                   
               
                --  JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID    
                 left join TriggerRes TR on Tr.TRIGGERID=49 and tr.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                  left join Annualhealthcheck A on A.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                    WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=1 and customer.UserID='SDPOLICY' and
                 custapl.POLICYNUMBER like '%ESP' and custapl.CONTRACTSTATUS<>'60'  and CUSTAPL.CONTRACTEXPIRES>GETDATE()  and CUSTAPL.CONTRACTSTART <CONVERT(VARCHAR(10), getdate(), 110)
              and CUSTAPL.CONTRACTDT>'2018-01-28' and dateadd(month,12,[CustApl].contractstart)>='2019-05-01' 
             and DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)>12
           and  DATEDIFF(MONTH,GETDATE() ,custapl.CONTRACTEXPIRES)>2
             
            and tr.id is null   and A._id is null
            
             and datepart(dayofyear,custapl.CONTRACTSTART)=datepart(dayofyear,GETDATE())
              order by custapl.CONTRACTSTART ;
              
              
               insert into Annualhealthcheck ([CustAplId]
           ,[TriggerId]
           ,[TriggerValue]
           ,[ContractDt]
           ,[ContractStart]
           ,[ContractExpires]
           ,[RetailClientId]
           ,[Extension]
           ,[AnnualHealthDueDay]) 
          SELECT  CUSTAPL.custaplid,73,CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15)) AS HealthID,  custapl.CONTRACTDT,custapl.CONTRACTSTART ,
custapl.CONTRACTEXPIRES, [Customer].RetailClientID,case when DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)>12 then 0  else 1 end, 
datepart(dayofyear,custapl.CONTRACTEXPIRES)-60
          
     FROM [CustApl]    
                  join [Customer] on Customer.CUSTOMERID=ISNULL(ownercustomerid,custapl.customerid)                   
               
                --  JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID    
                 left join TriggerRes TR on Tr.TRIGGERID=73 and tr.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                 left join Annualhealthcheck A on A.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                 WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=2
                  and customer.UserID='SDPOLICY' and
                 custapl.POLICYNUMBER like '%ESP' and custapl.CONTRACTSTATUS<>'60'  and CUSTAPL.CONTRACTEXPIRES>GETDATE()  and CUSTAPL.CONTRACTSTART<=CONVERT(VARCHAR(10), getdate(), 110)
              and CUSTAPL.CONTRACTDT>'2018-03-10' and dateadd(month,10,custapl.contractstart)>='2019-05-08'
             and (DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)<=12           or  DATEDIFF(MONTH,GETDATE() ,custapl.CONTRACTEXPIRES)<=2)
             
            and tr.id is null   and A._id is null
            
             and datepart(dayofyear,custapl.CONTRACTEXPIRES)-61=datepart(dayofyear,GETDATE())
              order by CONTRACTSTART;
              
              
               insert into Annualhealthcheck ([CustAplId]
           ,[TriggerId]
           ,[TriggerValue]
           ,[ContractDt]
           ,[ContractStart]
           ,[ContractExpires]
           ,[RetailClientId]
           ,[Extension]
           ,[AnnualHealthDueDay]) 
          SELECT  CUSTAPL.custaplid,74,CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15)) AS HealthID,  custapl.CONTRACTDT,custapl.CONTRACTSTART ,
custapl.CONTRACTEXPIRES, [Customer].RetailClientID,case when DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)>12 then 0  else 1 end, 
datepart(dayofyear,custapl.CONTRACTEXPIRES)-60
          
     FROM [CustApl]    
                  join [Customer] on Customer.CUSTOMERID=ISNULL(ownercustomerid,custapl.customerid)                   
               
                --  JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID    
                 left join TriggerRes TR on Tr.TRIGGERID=74 and tr.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                 left join Annualhealthcheck A on A.TriggerValue=CAST(YEAR(GETDATE()) AS char(4))+'.'+CAST(CUSTAPL.CUSTAPLID AS char(15))
                 WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=1
                  and customer.UserID='SDPOLICY' and
                 custapl.POLICYNUMBER like '%ESP' and custapl.CONTRACTSTATUS<>'60'  and CUSTAPL.CONTRACTEXPIRES>GETDATE()  and CUSTAPL.CONTRACTSTART<=CONVERT(VARCHAR(10), getdate(), 110)
              and CUSTAPL.CONTRACTDT>'2018-03-10' and dateadd(month,10,custapl.contractstart)>='2019-05-08'
             and (DATEDIFF(MONTH,custapl.CONTRACTSTART ,custapl.CONTRACTEXPIRES)<=12           or  DATEDIFF(MONTH,GETDATE() ,custapl.CONTRACTEXPIRES)<=2)
             
            and tr.id is null   and A._id is null
            
             and datepart(dayofyear,custapl.CONTRACTEXPIRES)-61=datepart(dayofyear,GETDATE())
              order by CONTRACTSTART;
              
            
              
            
            
              
            
            
END

GO



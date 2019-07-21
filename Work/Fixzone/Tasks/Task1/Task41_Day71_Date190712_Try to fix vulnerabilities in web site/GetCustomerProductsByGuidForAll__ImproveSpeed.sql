USE [ShopDirect]
GO

/****** Object:  StoredProcedure [dbo].[GetCustomerProductsByGuidForAll]    Script Date: 19/07/2019 17:32:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCustomerProductsByGuidForAll__ImproveSpeed]
@CustomerGuid nvarchar(max)
AS

CREATE TABLE #TmpCustomer
(
  [CUSTOMERID] [int] NOT NULL,
  [TITLE] [varchar](5) NULL,
  [FIRSTNAME] [varchar](20) NULL,
  [SURNAME] [varchar](50) NULL,
  [ADDR1] [varchar](60) NULL,
  [ADDR2] [varchar](60) NULL,
  [ADDR3] [varchar](60) NULL,
  [TOWN] [varchar](60) NULL,
  [COUNTY] [varchar](40) NULL,
  [POSTCODE] [varchar](8) NULL,
  [TEL1] [varchar](20) NULL,
  [TEL2] [varchar](20) NULL,
  [TEL3] [varchar](20) NULL,
  [NOTES] [varchar](max) NULL,
  [EMAIL] [varchar](128) NULL,
  [SENDBY] [smallint] NULL,
  [CLIENTID] [int] NULL,
  [COUNTRY] [varchar](50) NULL,
  [CustomerGuid] [uniqueidentifier] NULL
)

INSERT INTO #TmpCustomer(CUSTOMERID, TITLE, FIRSTNAME, SURNAME, ADDR1, ADDR2, ADDR3, TOWN, COUNTY, POSTCODE, TEL1, TEL2, TEL3, NOTES, EMAIL, SENDBY, CLIENTID, COUNTRY, CustomerGuid)
SELECT CUSTOMERID, TITLE, FIRSTNAME, SURNAME, ADDR1, ADDR2, ADDR3, TOWN, COUNTY, POSTCODE, TEL1, TEL2, TEL3, NOTES, EMAIL, SENDBY, CLIENTID, COUNTRY, CustomerGuid
FROM Customer
WHERE CustomerGuid = @CustomerGuid

	select   s.serviceid, c.TITLE as 'Title', st.statusid, ca.custaplid,

			c.FIRSTNAME as 'Forename',

			c.SURNAME as 'Surname',

			c.POSTCODE as 'Postcode',

			(c.ADDR1 +  isnull(',' +c.NOTES,'')) as 'Addr1',

			c.ADDR2 as 'Addr2',

			c.ADDR3 as 'Addr3',

			c.TEL1 as 'MobileTel',

			c.TEL2 as 'LandlineTel',

			c.TEL3 as 'TEL3',

			c.EMAIL as 'Email',

			c.TOWN as 'Town',

			c.COUNTY as 'County',

			c.COUNTRY as 'Country',

				D.EventDate  as 'VisitDate',

			c.CustomerId as 'CustomerId',

			c.customerGuid as 'CustomerGuid',

			CAST(c.SENDBY as int) as 'ContactMethod',

			m.MODEL as 'ItemCode', 
			c.clientid as ClientId,
			m.ModelId as ModelId,
			isnull(pa.[desc],'') as 'Description', 
			isnull(su.supplier,'') as 'Brand', 
			isnull(m.MODEL,'') as 'ModelNumber',
			isnull(m.notes,'') as 'Notes',
			isnull(m.schemafile,'') as 'ImageFileName',
			isnull(m.MFR,'')	as 'MFR',
			isnull(man.Name,'') as Manufacturer,
			ca.sno as SerialNumber,
			ca.APPLIANCEPRICE as AppliancePrice,
			ca.supplydat as 'SupplyDat',
			ca.ServiceStart as ServiceStartDate,
			ca.ServiceExpiry as ServiceExpiryDate,
		
			st.Status as [status],
		st.statusCustomerDescription,
			ca.appliancecd as appliancecd,
			ca.OnlineBookingExcluded as OnlineBookingExcluded,
			m.processId as processId,
			ca.CONTRACTSTART,ca.CONTRACTEXPIRES,
			pa.MonitorFg as MonitorFlag,
			ft.FraudResult as FraudResult

 from 
                      (
					  select max(s.serviceid) latestServiceiD,s.custaplid from service s 
                      join custapl  ca on ca.CUSTAPLID= s.CUSTAPLID  
                      --and (ca.CUSTOMERID =s.customerid or ca.OwnerCustomerID=s.CUSTOMERID)
                      join #TmpCustomer c on (ca.OwnerCustomerID = ca.CUSTOMERID and ca. CUSTOMERID=c.CUSTOMERID)
                      or
                      (ca.OwnerCustomerID is null and ca.CUSTOMERID=c.CUSTOMERID)
                      or
                      (ca.OwnerCustomerID= c.CUSTOMERID and ca.OwnerCustomerID<> ca.CUSTOMERID)
                      
                      
                      left outer join model m on  CA.MODEL = M.MODEL AND CA.MFR = M.MFR and  CA.APPLIANCECD = M.APPLIANCECD
                      
                      left join Manufact Man on man.MFR =m.MFR
                     
                      
                      left outer join supplier su on su.supplierid=m.supplierid
                      left outer join pop_apl pa on ca.APPLIANCECD=pa.APPLIANCECD
                      --where [EnroleCode] = cast(@EnrolmentCode as uniqueidentifier)
                      WHERE (c.CustomerGuid =  cast(@CustomerGuid  as uniqueidentifier)) and ca.contractcanceldate is null
--or ca.OwnerCustomerID = @CustomerID)
                      group by s.CUSTAPLID
					  )
	
	
	 as latestService  join service s on s.SERVICEID =latestService.latestServiceiD and s.CUSTAPLID =latestService.CUSTAPLID

	inner join Custapl ca on ca.CUSTAPLID= s.CUSTAPLID
		LEFT JOIN DiaryEnt D ON D.TagInteger1 = S.SERVICEID

	inner join #TmpCustomer c on (ca.OwnerCustomerID = ca.CUSTOMERID and ca. CUSTOMERID=c.CUSTOMERID)
or
(ca.OwnerCustomerID is null and ca.CUSTOMERID=c.CUSTOMERID)
or
(ca.OwnerCustomerID= c.CUSTOMERID and ca.OwnerCustomerID<> ca.CUSTOMERID) 

	left outer join model m on  CA.MODEL = M.MODEL AND CA.MFR = M.MFR and  CA.APPLIANCECD = M.APPLIANCECD

	left join Manufact Man on man.MFR =m.MFR
	left outer join supplier su on su.supplierid=m.supplierid
	left outer join pop_apl pa on ca.APPLIANCECD=pa.APPLIANCECD
	left  join FraudTests ft on s.SERVICEID = ft.SERVICEID
	left join pop_cc pc on pc.visitcd=s.visitcd
	 join [status] as st on st.statusid= s.statusid
	--left join [substatus] as sb on st.statusid=sb.statusid
	WHERE (c.CustomerGuid =  cast(@CustomerGuid  as uniqueidentifier)--or ca.OwnerCustomerID = @CustomerID) 	--and (RIGHT(ca.PolicyNumber,3) = 'ESP')
	and ca.contractcanceldate is null
)
	
 	order by ca.CONTRACTEXPIRES desc
GO

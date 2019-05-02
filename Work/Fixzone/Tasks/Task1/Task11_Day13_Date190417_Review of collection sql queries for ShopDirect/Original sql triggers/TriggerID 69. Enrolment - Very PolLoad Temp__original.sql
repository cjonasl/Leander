SELECT TOP 25   Customer.EMAIL AS MESSAGESRV_EMLCTemp_HTML_EMAIL,      CustomerEnrolment.EnroleID,     CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,
CustomerEnrolment.CustomerID,     f.footer as Footer,RC.RetailClientName as Brand,    RC.Domain+'/account/Enrolment?EnrolmentCode='+cast(enrolecode as varchar(200) )AS [Domain],      
RC.Domain+'/Content/img/ClientLogo.png' as Logo,          LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS CustomerName,
COUNT(CustApl.APPLIANCECD) as 'AppCount'   INTO #EnroleEmail_Very  FROM CustomerEnrolment   
LEFT JOIN TriggerRes ON (TRIGGERID=1 or TRIGGERID=69 or TRIGGERID=71)  AND TRIGGERFIELDLAST='EnroleID'    AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID    
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID    
LEFT JOIN Custapl ON isnull(CustApl.OwnerCustomerID,CustApl.CUSTOMERID)=Customer.CUSTOMERID      
LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=CUSTAPL.APPLIANCECD       
LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID       
left join Footer as F on right(custapl.policynumber,3) =f.Type     
 WHERE (ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL   AND COALESCE(Customer.EMAIL,'')<>'' AND Customer.RetailClientID=2   AND Customer.UserID='SDPOLICY'  AND Custapl.CREATEDDATETIME >= '2019-01-01' and 
 Custapl.ContractDt >='2018-12-01'        ANd Custapl.POLICYNUMBER like '%ESP'  AND CustomerEnrolment.LinkType=0  AND TriggerRes.id IS NULL   AND CustApl.CUSTOMERID IS NOT NULL        
 AND (isnull(custapl.CONTRACTCANCELDATE,'1900-01-01') ='1900-01-01'  and custapl.contractstatus<>'60')  )        
group by enroleid, email, enrolecode, CustomerEnrolment.CustomerID, Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME,f.Footer,rc.RetailClientName,rc.Domain        
 order by  min(Custapl.ContractDt )   ;    
              
              
 SELECT   EEV.MESSAGESRV_EMLCTemp_HTML_EMAIL,    EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,    COALESCE(model.[DESCription],          
  'Electrical Items') AS ElectricalItem,        EEV.footer as Footer,     EEV.Brand as Brand,    EEV.Domain AS [Domain],    EEV.Logo as Logo,               
  'Welcome to the Service Guarantee Online Service Centre' AS VeryEnrollment  FROM #EnroleEmail_Very EEV      
  JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID       
  JOIN Custapl ON isnull(CustApl.OwnerCustomerID,CustApl.CUSTOMERID)=Customer.CUSTOMERID           
  LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.MODEL = custapl.MODEL and model.MFR=custapl.MFR      
  where Customer.UserID='SDPOLICY' and Custapl.POLICYNUMBER like '%ESP'and Custapl.CREATEDDATETIME >= '2019-01-01'   and Custapl.ContractDt >='2018-12-01' and    (isnull(custapl.CONTRACTCANCELDATE,'1900-01-01') ='1900-01-01'  and custapl.contractstatus<>'60')
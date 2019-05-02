SELECT TOP 25    Customer.EMAIL AS MESSAGESRV_EMLWTemp_HTML_EMAIL,     CustomerEnrolment.EnroleID,     CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,          CustomerEnrolment.CustomerID,   
LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+          LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' +   LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+          
LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+            LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],
COUNT(CustApl.APPLIANCECD) as "AppCount"  INTO #EnroleEmail_LW              
FROM CustomerEnrolment  
LEFT JOIN TriggerRes ON (TRIGGERID=3 or TRIGGERID=70 or TRIGGERID=72)  AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID            
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID             LEFT JOIN Custapl ON  isnull(CustApl.OwnerCustomerID,CustApl.CUSTOMERID)=Customer.CUSTOMERID              
LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=CUSTAPL.APPLIANCECD            
WHERE ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL     AND COALESCE(Customer.EMAIL,'')<>'' AND Customer.RetailClientID=1   AND Customer.UserID='SDPOLICY'              and Custapl.policynumber like '%ESP'  AND   CustomerEnrolment.LinkType=0 AND TriggerRes.id IS NULL   AND CustApl.CUSTOMERID IS NOT NULL              AND Custapl.CREATEDDATETIME  >= '2019-01-01' AND Custapl.ContractDt  >= '2018-12-01'          
AND (isnull(custapl.CONTRACTCANCELDATE,'1900-01-01') ='1900-01-01'  and custapl.contractstatus<>'60')              
group by enroleid, email, enrolecode, CustomerEnrolment.CustomerID, Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME
Order by  min(Custapl.ContractDt ) ;               

SELECT     EEV.MESSAGESRV_EMLWTemp_HTML_EMAIL,     EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,    COALESCE(model.Description, 'Electrical Items') AS ElectricalItem,         'Littlewoods' AS Brand,    
'Welcome to the Service Guarantee Online Service Centre' AS LWEnrolment  
FROM #EnroleEmail_LW EEV           
JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID            
JOIN Custapl ON isnull( CustApl.ownerCUSTOMERID,CustApl.CUSTOMERID)=Customer.CUSTOMERID                     
LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.model=custapl.model and model.MFR =custapl.MFR                    
where Custapl.policynumber like '%ESP' AND Custapl.CREATEDDATETIME  > '2019-01-01' AND Custapl.ContractDt  >= '2018-12-01'     AND Customer.UserID='SDPOLICY'       and  (isnull(custapl.CONTRACTCANCELDATE,'1900-01-01') ='1900-01-01'  
and custapl.contractstatus<>'60')  
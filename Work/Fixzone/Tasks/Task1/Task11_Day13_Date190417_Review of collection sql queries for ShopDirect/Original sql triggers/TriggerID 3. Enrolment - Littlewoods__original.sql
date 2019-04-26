SELECT 
top 25
  Customer.EMAIL AS MESSAGESRV_EmailCustL_HTML_EMAIL, 
  CustomerEnrolment.EnroleID, 
  CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,    
  CustomerEnrolment.CustomerID, 
LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName], 
   COUNT(CustApl.APPLIANCECD) as "AppCount"
INTO #EnroleEmail_LW  
FROM CustomerEnrolment
LEFT JOIN TriggerRes ON TRIGGERID=3 AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID 
LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=CUSTAPL.APPLIANCECD
WHERE ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL 
--and Custapl.policynumber like '%ESP'
AND COALESCE(Customer.EMAIL,'')<>'' AND Customer.RetailClientID=1
 AND Customer.UserID='STREAMLINE' 
--AND CustomerEnrolment.DateCreated >= '2018-02-02' 
and Custapl.policynumber like '%ESP'
AND 
CustomerEnrolment.LinkType=0 AND TriggerRes.id IS NULL
 AND CustApl.CUSTOMERID IS NOT NULL
group by enroleid, email, enrolecode, CustomerEnrolment.CustomerID, Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME;

SELECT 
  EEV.MESSAGESRV_EmailCustL_HTML_EMAIL, 
  EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,
  COALESCE(model.Description, 'Electrical Items') AS ElectricalItem, 
  'Littlewoods' AS Brand,
  'Welcome to the Service Guarantee Online Service Centre' AS LWEnrolment
FROM #EnroleEmail_LW EEV
LEFT JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID 
LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID AND AppCount=1 and Custapl.policynumber like '%ESP' AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.model=custapl.model and model.MFR =custapl.MFR
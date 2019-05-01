 SELECT 
top 25
 Customer.EMAIL AS MESSAGESRV_EmailCust_HTML_EMAIL, 
   CustomerEnrolment.EnroleID, 
  CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,    
  CustomerEnrolment.CustomerID, 
  f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain+'/account/Enrolment?EnrolmentCode='+cast(enrolecode as varchar(200) )AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
   COUNT(CustApl.APPLIANCECD) as 'AppCount'
INTO #EnroleEmail_Very  
FROM CustomerEnrolment
LEFT JOIN TriggerRes ON TRIGGERID=1 AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID 
LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=CUSTAPL.APPLIANCECD
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
WHERE (ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL 
AND COALESCE(Customer.EMAIL,'')<>'' AND Customer.RetailClientID=2
 AND Customer.UserID='STREAMLINE' 
  ANd Custapl.POLICYNUMBER like '%ESP'
AND CustomerEnrolment.LinkType=0 AND TriggerRes.id IS NULL
 AND CustApl.CUSTOMERID IS NOT NULL 
 

 ) 
group by enroleid, email, enrolecode, CustomerEnrolment.CustomerID, Customer.TITLE, Customer.FIRSTNAME, Customer.SURNAME,f.Footer,rc.RetailClientName,rc.Domain;

SELECT 
  EEV.MESSAGESRV_EmailCust_HTML_EMAIL, 
 EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,
  COALESCE(model.[DESCription], 'Electrical Items') AS ElectricalItem,
  
  EEV.footer as Footer,
   EEV.Brand as Brand,
  EEV.Domain AS [Domain],
  EEV.Logo as Logo,
 
  'Welcome to the Service Guarantee Online Service Centre' AS VeryEnrollment
FROM #EnroleEmail_Very EEV
LEFT JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID 
LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID ANd Custapl.POLICYNUMBER like '%ESP' AND AppCount=1 AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.MODEL = custapl.MODEL and model.MFR=custapl.MFR
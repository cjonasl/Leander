SELECT 
Customer.EMAIL 
 AS MESSAGESRV_EMLCUSTLFP_HTML_EMAIL, 
  CustomerEnrolment.EnroleID, 
  CAST(CustomerEnrolment.EnroleCode AS CHAR(36)) AS EnroleCode,    
  CustomerEnrolment.CustomerID, 
 LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],
   RC.RetailClientName as Brand,
  RC.Domain AS Domain,
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Password Reset' AS LWPasswordReset
FROM CustomerEnrolment
LEFT JOIN TriggerRes ON TRIGGERID=5 AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join custapl on customer.CUSTOMERID=custapl.CUSTOMERID 
WHERE ValidFlag=1 AND
 Customer.CUSTOMERID IS NOT NULL 
  AND TriggerRes.id IS NULL
 and custapl.POLICYNUMBER like '%ESP'
AND Customer.EMAIL IS NOT NULL AND Customer.RetailClientID=1
AND  CustomerEnrolment.LinkType=1
group by Customer.EMAIL,CustomerEnrolment.EnroleID, 
  CustomerEnrolment.EnroleCode , 
  RC.RetailClientName,  
  RC.Domain, 
  CustomerEnrolment.CustomerID,
  Customer.TITLE,
  Customer.FIRSTNAME,
  Customer.SURNAME
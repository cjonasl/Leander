SELECT 
  Customer.EMAIL AS MESSAGESRV_EmlCustVFP_HTML_EMAIL, 
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
  'Password Reset' AS VeryPasswordReset
FROM CustomerEnrolment
LEFT JOIN TriggerRes ON TRIGGERID=4 AND TRIGGERFIELDLAST='EnroleID' AND TriggerRes.TriggerValue=CustomerEnrolment.EnroleID
LEFT JOIN Customer ON Customer.CUSTOMERID=CustomerEnrolment.CustomerID 
left join custapl on customer.CUSTOMERID=custapl.CUSTOMERID or customer.CUSTOMERID=custapl.OwnerCUSTOMERID 
 LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
WHERE ValidFlag=1 AND Customer.CUSTOMERID IS NOT NULL 
 AND TriggerRes.id IS NULL
and custapl.POLICYNUMBER like '%ESP'
AND Customer.EMAIL IS NOT NULL AND Customer.RetailClientID=2
 AND CustomerEnrolment.LinkType=1
group by Customer.EMAIL,CustomerEnrolment.EnroleID, 
  CustomerEnrolment.EnroleCode ,    
  CustomerEnrolment.CustomerID,
  Customer.TITLE,
  RC.RetailClientName,  
  RC.Domain, 
  Customer.FIRSTNAME,
  Customer.SURNAME
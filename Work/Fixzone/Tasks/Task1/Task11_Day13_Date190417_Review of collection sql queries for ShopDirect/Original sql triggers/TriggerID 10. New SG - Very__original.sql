SELECT
  Customer.EMAIL
   AS MESSAGESRV_NewSGV_HTML_EMAIL, 
  [NewCustAplForCustomer].[CustomerID],
  [NewCustAplForCustomer].[CustAplID],
   LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  Coalesce(POP_Apl.[DESC], 'item') AS [DESC],
  [CustApl].[PolicyNumber],
  f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [WebLink],
  RC.Domain AS [Domain],
  'Service Guarantee'
   as 'VeryNewSG',
  RC.Domain+'/Content/img/ClientLogo.png' as Logo   
FROM [NewCustAplForCustomer]
LEFT JOIN TriggerRes ON TRIGGERID=10 AND TRIGGERFIELDLAST='CustAplID' AND TriggerRes.TriggerValue=[NewCustAplForCustomer].CustAplID
  left join [Customer] on Customer.CUSTOMERID=NewCustAplForCustomer.CustomerID
  left join [CustApl] on Custapl.CUSTAPLID=NewCustAplForCustomer.CustAplID
  LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=2 and custapl.policynumber like '%ESP'
AND TriggerRes.id IS NULL
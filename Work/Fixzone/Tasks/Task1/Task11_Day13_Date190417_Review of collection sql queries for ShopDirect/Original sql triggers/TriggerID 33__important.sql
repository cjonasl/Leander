
 SELECT
 Customer.EMAIL
  AS MESSAGESRV_NewSGLW_HTML_EMAIL, 
  [NewCustAplForCustomer].[CustomerID],
  [NewCustAplForCustomer].[CustAplID],
   LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  Coalesce(Model.[DESCription], 'item') AS [DESC],
  [CustApl].[PolicyNumber],
  f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [WebLink],
   RC.Domain AS Domain,
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Thanks for purchasing a Service Guarantee' AS [LWNewSG]
FROM [NewCustAplForCustomer]
LEFT JOIN TriggerRes ON TRIGGERID=33 AND TRIGGERFIELDLAST='CustAplID' AND TriggerRes.TriggerValue=[NewCustAplForCustomer].CustAplID
  left join [Customer] on Customer.CUSTOMERID=NewCustAplForCustomer.CustomerID
  left join [CustApl] on Custapl.CUSTAPLID=NewCustAplForCustomer.CustAplID and Custapl.POLICYNUMBER like '%ESP'
  LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
  LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and Model.MFR = CustApl.MFR and Model.Model= Custapl.Model
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=1
 AND TriggerRes.id IS NULL 
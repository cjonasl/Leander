SELECT
CASE LEFT(Customer.TEL1,2) WHEN '07' THEN Customer.TEL1 ELSE  CASE LEFT(Customer.TEL2,2) WHEN '07' THEN Customer.TEL2 ELSE Customer.TEL3 END END 
--'07472939499'
 AS MESSAGESRV_NewSGSMS_TEXT_SMS, 
'This Service Guarantee for your '+ Coalesce(Model.[DESCription], 'item') +' policy number '+ [CustApl].[PolicyNumber] +' will be added into your Online Service Centre account.'
as  NewSGVSMSTEXT , 
    [NewCustAplForCustomer].[CustomerID],
  [NewCustAplForCustomer].[CustAplID],
     RC.RetailClientName as Brand,
   'Service Guarantee'
   as 'NewSGSMS'
FROM [NewCustAplForCustomer]
LEFT JOIN TriggerRes ON TRIGGERID=67 AND TRIGGERFIELDLAST='CustAplID' AND TriggerRes.TriggerValue=[NewCustAplForCustomer].CustAplID
  left join [Customer] on Customer.CUSTOMERID=NewCustAplForCustomer.CustomerID
  left join [CustApl] on Custapl.CUSTAPLID=NewCustAplForCustomer.CustAplID
  LEFT JOIN POP_Apl ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
  left join Model on model.Model = custapl.model and model.appliancecd = custapl.appliancecd and model.mfr=custapl.mfr
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
WHERE Customer.EMAIL<>'' AND Customer.RetailClientID=2 and custapl.policynumber like '%ESP'
AND TriggerRes.id IS NULL
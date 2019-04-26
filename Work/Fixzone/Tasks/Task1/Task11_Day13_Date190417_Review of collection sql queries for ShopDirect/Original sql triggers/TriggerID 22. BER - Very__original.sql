SELECT 
  'Mary@fixzone.com'
  --Customer.EMAIL 
  AS MESSAGESRV_BERV_HTML_EMAIL,  
  Service.ServiceId, Custapl.POLICYNUMBER,
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'item') AS [DESC], 
  '0800 092 9051' AS UKWPHONENUMBER,  f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo, 
  'We''ve tried to contact you' AS VeryBER,
  case
  when (Custapl.POLICYNUMBER like '%ESP')
  then 
  'Service Request'
   when (Custapl.POLICYNUMBER like '%MPI')
   then
  'claim'
  else
  'Service Request'
  end as 'PolicyType'

FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and Model.MFR = CustApl.MFR and Model.Model= Custapl.Model
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE DiaryEnt.EventDate=CONVERT(DATE, GETDATE()) AND
 ((Customer.RetailClientID=2 AND Customer.EMAIL IS NOT NULL) AND service.STATUSID=19 AND service.SUBSTATUS=1 
 AND (Custapl.POLICYNUMBER like '%ESP' or
  Custapl.POLICYNUMBER like '%MPI%'
 )
 and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
 )
  or DiaryEnt.DiaryID in (3035962,3055022)
SELECT 
Customer.Email AS MESSAGESRV_SURVEYLW_HTML_EMAIL, 
  Service.ServiceID, 
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(model.[DESCription], 'item') AS [DESC], 
  '0800 092 9051' AS UKWPHONENUMBER, 
   f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain+'/SURVEY' AS [Domain],
  
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
   case
  when (Custapl.POLICYNUMBER like '%ESP')
  then 
  'Service Request'
   when (Custapl.POLICYNUMBER like '%MPI' or Custapl.POLICYNUMBER like '%RPG')
   then
  'claim'
  else
  'Service Request'
  end as 'PolicyType',
   'We''d really like to know what you thought of us' AS LWSurvey
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
 LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and Model.MFR = CustApl.MFR and Model.Model= Custapl.Model
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (DiaryEnt.EventDate BETWEEN '2017-12-01' AND GETDATE() AND
 (Customer.RetailClientID=1 AND Customer.EMAIL IS NOT NULL)
   AND service.STATUSID=8
   and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
  ) 
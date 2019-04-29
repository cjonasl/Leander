SELECT 
'Mary@fixzone.com' 
 --Customer.EMAIL 
 AS MESSAGESRV_COURDESLW_HTML_EMAIL,  
  DiaryEnt.DiaryID,  
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'Electrical Item') AS [DESC],
  Enginrs.ENGINEERID, 
  COALESCE(Enginrs.TELNO,'0800 092 9051') AS TELNO , 
 'DPD' AS CourierName, 
  '5148870299' AS COURIERTRACKING,
  '0800 092 9051' AS UKWPHONENUMBER,
   f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Your '+COALESCE(Model.[DESCription], 'item')+ ' has been despatched' AS LWCourierDespatch
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
left join Footer as F on right(custapl.policynumber,3) =f.Type
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and model.mfr=custapl.mfr and model.model=custapl.model
left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (Customer.RetailClientID=1 AND Customer.EMAIL IS NOT NULL 
AND COALESCE(POP_Apl.MONITORFG,'F')='T' AND Enginrs.DumpDiary=0
AND DiaryEnt.EventDate>'2017-12-01'
AND service.STATUSID=803
)
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
or DiaryEnt.Diaryid in (2489464,2833502,2355979)
--and custapl.POLICYnumber like '%esp'
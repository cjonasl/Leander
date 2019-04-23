SELECT 
'Mary@fixzone.com'
--Customer.EMAIL
  AS MESSAGESRV_COUREPV_HTML_EMAIL,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS CourierDeliveryDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
    f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  '0800 092 9051' AS UKWTELEPHONENUMBER,
  case
  when custapl.POLICYnumber like '%rpg'
  then
  'Inspection complete' 
  else
  'Repair complete'
  end
  AS VeryCourierRepaired,
  case
  when custapl.POLICYnumber like '%rpg'
  then
  'inspection' 
  else
  'repair'
  end
  AS VeryTermType
FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and model.mfr=custapl.mfr and model.model=custapl.model
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (Customer.RetailClientID=2 AND Customer.EMAIL IS NOT NULL
 AND POP_Apl.MONITORFG='T'
  AND Enginrs.DumpDiary=0) 
AND DiaryEnt.EventDate>'2017-12-01'
AND service.STATUSID=800 
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
or DiaryEnt.DiaryId in (3034260,3053158,2851968)
--and custapl.POLICYnumber like '%rpg'







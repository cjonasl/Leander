SELECT 
  'Mary@fixzone.com' AS MESSAGESRV_B2BLWRPG_HTML_EMAIL,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
   f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(POP_Apl.[DESC], 'Electrical Item') AS [DESC], 
  '0800 092 9051' AS UKWPHONENUMBER, 
  'VERY' AS Brand, 
  'Your '+COALESCE(POP_Apl.[DESC], 'Electrical Item')+' is with an engineer' AS LWB2BRPG
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE DiaryEnt.EventDate=CONVERT(DATE, GETDATE()) AND
 ((Customer.RetailClientID=1 AND Customer.EMAIL IS NOT NULL)
 AND  (Custapl.POLICYNUMBER LIKE '%RPG' OR Custapl.POLICYNUMBER LIKE '%MPI')
 AND  service.STATUSID=19 AND service.SUBSTATUS=1
 and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
 )
or DiaryID=2461232 or diaryid=2461429
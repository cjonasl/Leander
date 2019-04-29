SELECT 
 Customer.EMAIL AS MESSAGESRV_LOANTV_HTML_EMAIL,  
  DiaryEnt.DiaryID,  
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(POP_Apl.[DESC], 'Electrical Item') AS [DESC],
  Enginrs.ENGINEERID, 
  COALESCE(Enginrs.TELNO,'0800 092 9051') AS TELNO , 
 'DPD' AS CourierName, 
  '5148870299' AS COURIERTRACKING,
  '0800 092 9051' AS UKWPHONENUMBER,
  'VERY' AS Brand,
  'Your '+COALESCE(POP_Apl.[DESC], 'item')+ ' has been despatched' AS LoanTVDesparthV
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (Customer.RetailClientID=2 AND Customer.EMAIL IS NOT NULL AND COALESCE(POP_Apl.MONITORFG,'F')='T' AND Enginrs.DumpDiary=0
AND DiaryEnt.EventDate>'2017-12-01'
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
AND service.STATUSID=804
) or diaryid=2591350
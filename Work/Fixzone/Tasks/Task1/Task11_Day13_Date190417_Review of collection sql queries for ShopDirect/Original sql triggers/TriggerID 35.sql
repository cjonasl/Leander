SELECT 
  Customer.EMAIL AS MESSAGESRV_SURVEYVRPG_HTML_EMAIL, 
  Service.ServiceID, 
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(POP_Apl.[DESC], 'item') AS [DESC], 
  '0800 092 9051' AS UKWPHONENUMBER, 'VERY' AS Brand, 
   'We''d really like to know what you thought of us' AS VerySurvey
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (DiaryEnt.EventDate BETWEEN '2017-12-01' AND GETDATE() 
AND (Customer.RetailClientID=2 AND Customer.EMAIL IS NOT NULL)
AND (Custapl.POLICYNUMBER LIKE '%RPG' or Custapl.POLICYNUMBER LIKE '%MPI') AND service.STATUSID=8) 
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
SELECT 
CASE LEFT(Customer.TEL1,2) WHEN '07' THEN Customer.TEL1 ELSE  CASE LEFT(Customer.TEL2,2) WHEN '07' THEN Customer.TEL2 ELSE Customer.TEL3 END END 
--'07472939499'
 AS MESSAGESRV_FAILEDV_TEXT_SMS,
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'item') AS [DESC], 
  '0800 092 9051' AS UKWPHONENUMBER, RC.RetailClientName as Brand,
  RC.Domain AS [Domain], 'We missed you' AS VeryFailedAppt
FROM [DiaryEnt]
LEFT JOIN [service] ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and Model.MODEL= custapl.MODEL
 LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
 left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE  DiaryEnt.EventDate=CONVERT(DATE, GETDATE()) AND
 (((Customer.RetailClientID=2 or Customer.Retailclientid =1)AND 
(Customer.TEL1 LIKE '07%' OR  Customer.TEL2 LIKE '07%' OR Customer.TEL3 LIKE '07%')
) AND service.STATUSID=19 AND service.SUBSTATUS=1
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
)
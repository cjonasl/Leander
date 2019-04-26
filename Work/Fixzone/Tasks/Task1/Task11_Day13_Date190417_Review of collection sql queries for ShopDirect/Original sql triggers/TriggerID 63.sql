SELECT 
  CASE LEFT(Customer.TEL1,2) WHEN '07' THEN Customer.TEL1 ELSE  CASE LEFT(Customer.TEL2,2) WHEN '07' THEN Customer.TEL2 ELSE Customer.TEL3 END END 
--'07472939499' 
AS MESSAGESRV_COUREPV_TEXT_SMS,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS CourierDeliveryDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
      RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  '0800 092 9051' AS UKWTELEPHONENUMBER,
  'Repair complete' AS VeryCourierRepaired
FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD  and model.model=custapl.model
 left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE ((Customer.RetailClientID=2 OR CUSTOMER.RetailClientID=1) AND (Customer.TEL1 LIKE '07%' OR  Customer.TEL2 LIKE '07%' OR Customer.TEL3 LIKE '07%')
 AND POP_Apl.MONITORFG='T'
  AND Enginrs.DumpDiary=0
  ) 
AND DiaryEnt.EventDate>'2017-12-01'
AND service.STATUSID=800  
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)

SELECT 
  CASE LEFT(Customer.TEL1,2) WHEN '07' THEN Customer.TEL1 ELSE  CASE LEFT(Customer.TEL2,2) WHEN '07' THEN Customer.TEL2 ELSE Customer.TEL3 END END 
  --'07472939499'
  AS  MESSAGESRV_COURV_TEXT_SMS,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(POP_Apl.[DESC], 'item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
   'VERY' AS Brand,
  'Collection confirmation' AS COURV
FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
left join model on Custapl.appliancecd = Model.appliancecd and Custapl.MFR = model.MFR and model.MODEL=custapl.model
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (EnterDate >= GETDATE()  AND 
(Customer.RetailClientID=2 AND (Customer.TEL1 LIKE '07%' OR  Customer.TEL2 LIKE '07%' OR Customer.TEL3 LIKE '07%')) AND POP_Apl.MONITORFG='T' 
AND Enginrs.DumpDiary=0)
and Service.Statusid <>2 and Service.Statusid <> 10 
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
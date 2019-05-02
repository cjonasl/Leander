SELECT top 25
    Customer.EMAIL
  AS MESSAGESRV_CONFBKV_HTML_EMAIL,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(model.[DESCription], 'item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
   f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS Domain,
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Confirmation of Service Request booking' AS VeryConfAppt
FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID 
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and model.mfr=custapl.mfr and model.model=custapl.model
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
 LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (DiaryEnt.EventDate > GETDATE()  AND (Customer.RetailClientID=2 AND Customer.EMAIL IS NOT NULL) AND POP_Apl.MONITORFG='F' AND Enginrs.DumpDiary=0)  
and custapl.POLICYNUMBER like '%ESP' 
 AND Service.StatusId<>2 and Service.StatusId<>10
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)

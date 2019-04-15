 SELECT top 25
  Customer.Email 
  AS MESSAGESRV_INITCONFV_HTML_EMAIL,  
  DiaryEnt.DiaryID, 
   f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
 LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],
  COALESCE(model.[DESCription], 'item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
  'Service Request raised' AS VeryIntialAppt
FROM DiaryEnt
LEFT JOIN TriggerRes ON TRIGGERID=2 AND TRIGGERFIELDLAST='DiaryID' AND TriggerRes.TriggerValue=DiaryEnt.DiaryID
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID 
LEFT JOIN model ON model.APPLIANCECD=Custapl.APPLIANCECD and model.MFR = custapl.MFR and model.model=custapl.model
left join pop_apl on pop_apl.APPLIANCECD=custapl.APPLIANCECD
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type  
 left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (DiaryEnt.EventDate > GETDATE() AND
Customer.RetailClientID=2 
AND TriggerRes.id IS NULL AND
 custapl.policynumber like '%ESP' and
 Customer.EMAIL IS NOT NULL AND COALESCE(POP_Apl.MONITORFG, 'F')='F'
  and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null) 
  and (service.statusid != 2 and service.statusid != 8) )  
 group by  
 Customer.TITLE,
 Customer.Email  ,Customer.FIRSTNAME,  Customer.SURNAME,
 Customer.ADDR1,Customer.ADDR2,Customer.ADDR3,Customer.POSTCODE,
  DiaryEnt.DiaryID, 
  model.[DESCription],
   F.footer ,
   RC.RetailClientName,
  RC.Domain,
  DiaryEnt.EventDate

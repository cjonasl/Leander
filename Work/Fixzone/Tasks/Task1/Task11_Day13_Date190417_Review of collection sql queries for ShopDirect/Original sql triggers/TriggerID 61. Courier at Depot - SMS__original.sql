SELECT 
CASE LEFT(Customer.TEL1,2) WHEN '07' THEN Customer.TEL1 ELSE  CASE LEFT(Customer.TEL2,2) WHEN '07' THEN Customer.TEL2 ELSE Customer.TEL3 END END 
--'07472939499'
 AS MESSAGESRV_Depot_TEXT_SMS,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(model.[DESCription], 'Electrical Item') AS [DESC],
   COALESCE(Customer.ADDR1,'') AS ADDR1 ,  COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
    'Your '+COALESCE(Model.[DESCription], 'item')+' has arrived' AS VeryDepot,
	 case
  when (Custapl.POLICYNUMBER like '%ESP')
  then 
  'Service Guarantee'
   when (Custapl.POLICYNUMBER like '%MPI')
   then
  'Mobile Phone Insurance'
  else
  'Replacement Guarantee'
  end as 'subjectCA',
   case
  when (Custapl.POLICYNUMBER like '%ESP')
  then 
  'Don’t forget you can also track the progress by logging in to the Online Service Centre using the link below.'
   when (Custapl.POLICYNUMBER like '%MPI')
   then
  ''
  else
  ''
  end as 'SerGuarText',
  case
  when (Custapl.POLICYNUMBER like '%ESP')
  then 
   RC.Domain
   else
  ''
  end  AS [Domain]

FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD  and model.model=custapl.model
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
  WHERE (EnterDate >= GETDATE()  AND 
(Customer.TEL1 LIKE '07%' OR  Customer.TEL2 LIKE '07%' OR Customer.TEL3 LIKE '07%') AND POP_Apl.MONITORFG='T' AND Enginrs.DumpDiary=0 
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
) 
SELECT top 25
 'Mary@fixzone.com'
 --Customer.EMAIL
  AS MESSAGESRV_REMINDL_HTML_EMAIL,  
  DiaryEnt.DiaryID,  CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
  LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],  
  COALESCE(Model.[DESCription], 'item') AS [DESC],
  COALESCE(Customer.ADDR1,'') AS ADDR1, COALESCE(Customer.ADDR2,'') AS ADDR2,  COALESCE(Customer.ADDR3,'') AS ADDR3, Customer.POSTCODE,
  Enginrs.ENGINEERID, 
  COALESCE(replace(Enginrs.TELNO,'*',''),'0800 092 9051') AS TELNO , 
  COALESCE(Enginrs.DISPLAYNAME,Enginrs.NAME) AS NAME, 
  '0800 092 9051' AS UKWPHONENUMBER,
  f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
  'Appointment reminder' AS LWRemind
FROM DiaryEnt
LEFT JOIN service ON Service.ServiceId=DiaryEnt.TagInteger1
LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CustomerID
LEFT JOIN [CustApl] ON Custapl.CUSTAPLID=service.CustAplID
LEFT JOIN [POP_Apl] ON POP_Apl.APPLIANCECD=Custapl.APPLIANCECD 
LEFT JOIN [Enginrs] ON Enginrs.EngineerId=DiaryEnt.UserID 
  LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  LEFT JOIN [Model] ON Model.APPLIANCECD=Custapl.APPLIANCECD and Model.MFR = CustApl.MFR and Model.Model= Custapl.Model
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
WHERE (DiaryEnt.EventDate=CONVERT(DATE, GETDATE()+1) AND 
 Custapl.POLICYNUMBER like '%ESP'AND
(Customer.RetailClientID=1 AND Customer.EMAIL IS NOT NULL)  AND COALESCE(POP_Apl.MONITORFG,'F')='F' AND Enginrs.DumpDiary=0
)
--or Diaryid=3018955
and Service.Statusid <>2 and Service.Statusid <> 10
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null) 
group by Customer.Email,  
  DiaryEnt.DiaryID,   DiaryEnt.EventDate, 
  Customer.TITLE,Customer.FIRSTNAME,Customer.SURNAME,
 Model.[DESCription],
  Customer.ADDR1, Customer.ADDR2,Customer.ADDR3, Customer.POSTCODE,
  Enginrs.ENGINEERID, 
 Enginrs.TELNO,
  Enginrs.DISPLAYNAME,  
  f.footer ,
   RC.RetailClientName ,
  RC.Domain,
  Enginrs.NAME 
SELECT 
    Customer.EMAIL AS MESSAGESRV_CANCELV_HTML_EMAIL,  
    [CancelledID],
    CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
	 f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
    LTRIM(REPLACE(Ltrim(COALESCE(UPPER(LEFT(Customer.TITLE,1))+
        LOWER(RIGHT(Customer.TITLE, LEN(Customer.TITLE) - 1)),'')) + ' ' + LTRIM(COALESCE(UPPER(LEFT(Customer.FIRSTNAME,1))+
        LOWER(RIGHT(Customer.FIRSTNAME, LEN(Customer.FIRSTNAME) - 1)),'')) + ' '   + LTRIM(COALESCE(UPPER(LEFT(Customer.SURNAME,1))+
        LOWER(RIGHT(Customer.SURNAME, LEN(Customer.SURNAME) - 1)),'')), '  ', ' ')) AS [CustomerName],
	'Appointment cancellation' AS CANCELV
  FROM [ShopDirect].[dbo].[CustomerCancelLog]
  LEFT JOIN [service] ON service.serviceid=CustomerCancelLog.ServiceID
  left join [custapl] on service.custaplid = custapl.custaplid
  LEFT JOIN [DiaryEnt] ON DiaryEnt.TagInteger1=service.SERVICEID  
  LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CUSTOMERID
    LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
  WHERE Customer.EMAIL IS NOT NULL AND service.STATUSID=2 AND Customer.RetailClientID=2  
and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null) 
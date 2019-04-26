SELECT 
    Customer.EMAIL
	 AS MESSAGESRV_CANCELLW_HTML_EMAIL,  
    [CancelledID],
	 f.footer as Footer,
   RC.RetailClientName as Brand,
  RC.Domain AS [Domain],
  RC.Domain+'/Content/img/ClientLogo.png' as Logo,
    CONVERT(char(10), DiaryEnt.EventDate, 103) AS EventDate,
    LTRIM(REPLACE(Ltrim(COALESCE(Customer.TITLE,'')) + ' ' + LTRIM(COALESCE(Customer.FIRSTNAME,'')) + ' '   + LTRIM(COALESCE(Customer.SURNAME,'')), '  ', ' ')) AS [CustomerName],
	'Appointment cancellation' AS CancelLW
  FROM [ShopDirect_test].[dbo].[CustomerCancelLog]
  LEFT JOIN [service] ON service.serviceid=CustomerCancelLog.ServiceID
  LEFT JOIN [DiaryEnt] ON DiaryEnt.TagInteger1=service.SERVICEID
  left join custapl on custapl.CUSTAPLID=service.CUSTAPLID
  LEFT JOIN [Customer] ON Customer.CUSTOMERID=service.CUSTOMERID
    LEFT JOIN [RetailClient] as RC on RC.RetailCode=Customer.RetailClientID and RC.RetailClientID=Customer.CLIENTID
  left join Footer as F on right(custapl.policynumber,3) =f.Type
  left join SpecJobMapping on SpecJobMapping.VisitType = service.VISITCD
  WHERE Customer.EMAIL IS NOT NULL 
  AND service.STATUSID=2 
  and  (SpecJobMapping.DummyJob <> 1   or SpecJobMapping.DummyJob is  null)
  AND Customer.RetailClientID=1 

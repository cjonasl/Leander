SELECT 
  ctm.EMAIL AS 'MESSAGESRV_CANCELLW_HTML_EMAIL',  
  can.CancelledID,
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  'Appointment cancellation' AS 'CancelLW'
FROM
  CustomerCancelLog can
  LEFT JOIN [service] ser ON can.ServiceID = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 43 AND res.TRIGGERFIELDLAST = 'CancelledId' AND res.TriggerValue = can.CancelledID
  LEFT JOIN DiaryEnt dia ON ser.SERVICEID = dia.TagInteger1
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Littlewoods') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, 'Cancelled') = 1
  AND dbo.fnFilter_ValueExists(ftr.footer) = 1
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1
SELECT 
  ctm.EMAIL AS 'MESSAGESRV_FAILEDV_HTML_EMAIL',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  '0800 092 9051' AS 'UKWPHONENUMBER',
  'We missed you' AS 'VeryFailedAppt'
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 14 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND (( ser.STATUSID=8 and ser.substatus=23) or ( ser.STATUSID=29 and ser.substatus=2) )
  
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
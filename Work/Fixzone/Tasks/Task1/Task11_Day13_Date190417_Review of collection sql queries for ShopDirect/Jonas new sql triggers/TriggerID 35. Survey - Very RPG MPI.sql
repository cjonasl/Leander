SELECT
  ctm.EMAIL AS 'MESSAGESRV_SURVEYVRPG_HTML_EMAIL',
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(pap.[DESC], 'Product') AS 'DESC',
  '0800 092 9051' AS 'UKWPHONENUMBER',
  'VERY' AS 'Brand',
  'We''d really like to know what you thought of us' AS 'SURVEYVRPG'
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 35 AND res.TRIGGERFIELDLAST = 'ServiceId' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0 
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Replacement Guarantee') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Mobile') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, 'Complete') = 1
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1
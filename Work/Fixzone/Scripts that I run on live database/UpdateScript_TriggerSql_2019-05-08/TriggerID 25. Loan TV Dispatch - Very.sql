SELECT
  ctm.EMAIL AS MESSAGESRV_LOANTV_HTML_EMAIL,
  dia.DiaryID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(pap.[DESC], 'Product') AS 'DESC',
  eng.ENGINEERID,
  ISNULL(eng.TELNO,'0800 092 9051') AS TELNO ,
  'DPD' AS 'CourierName',
  '5148870299' AS 'COURIERTRACKING',
  '0800 092 9051' AS 'UKWPHONENUMBER',
  'VERY' AS 'Brand',
  'Your '+ ISNULL(pap.[DESC], 'Product') + ' has been despatched' AS 'LoanTVDesparthV'
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 25 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, 'LoanTV Dispatch') = 1
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1
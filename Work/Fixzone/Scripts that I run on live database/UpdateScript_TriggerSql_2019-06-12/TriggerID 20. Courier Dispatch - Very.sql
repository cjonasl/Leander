SELECT 
  ctm.EMAIL AS 'MESSAGESRV_COURDESV_HTML_EMAIL',
  dia.DiaryID,  
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS  'DESC',
  eng.ENGINEERID,
  ISNULL(eng.TELNO,'0800 092 9051') AS 'TELNO', 
  'DPD' AS 'CourierName', 
  '5148870299' AS 'COURIERTRACKING',
  '0800 092 9051' AS 'UKWPHONENUMBER',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  'Your '+ ISNULL(mdl.[DESCRIPTION], 'Product') + ' has been despatched' AS 'VeryCourierDespatch'
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 20 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1

  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
   AND ser.STATUSID =5 and ser.substatus=20  --Engineer allocated (5)”   & “collection booked (20)”
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
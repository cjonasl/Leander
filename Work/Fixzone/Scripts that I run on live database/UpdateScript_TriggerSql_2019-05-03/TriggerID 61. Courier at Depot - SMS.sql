SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS 'MESSAGESRV_Depot_TEXT_SMS',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC',
  ISNULL(ctm.ADDR1, '') AS 'ADDR1', 
  ISNULL(ctm.ADDR2, '') AS 'ADDR2', 
  ISNULL(ctm.ADDR3, '') AS 'ADDR3',
  ctm.POSTCODE,
  'Your '+ ISNULL(mdl.[DESCRIPTION], 'Product') + ' has arrived' AS 'VeryDepot',
  CASE
    WHEN (cap.POLICYNUMBER LIKE '%ESP') THEN  'Service Guarantee'
    WHEN (cap.POLICYNUMBER LIKE '%MPI') THEN 'Mobile Phone Insurance'
	ELSE 'Replacement Guarantee'
  END AS 'subjectCA',
  CASE
    WHEN (cap.POLICYNUMBER LIKE '%ESP') THEN 'Don�t forget you can also track the progress by logging in to the Online Service Centre using the link below.'
    ELSE ''
  END AS 'SerGuarText',
  CASE
    WHEN (cap.POLICYNUMBER LIKE '%ESP') THEN rcl.Domain
    ELSE ''
  END AS 'Domain'
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 61 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
SELECT TOP 25
  ctm.Email AS MESSAGESRV_REMINDV_HTML_EMAIL,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC', 
  ISNULL(ctm.ADDR1, '') AS 'ADDR1',
  ISNULL(ctm.ADDR2, '') AS 'ADDR2',
  ISNULL(ctm.ADDR3, '') AS 'ADDR3',
  ctm.POSTCODE,
  eng.ENGINEERID, 
  ISNULL(REPLACE(eng.TELNO, '*', ''), '0800 092 9051') AS 'TELNO',   
  ISNULL(eng.DISPLAYNAME, eng.NAME) AS 'NAME',
  '0800 092 9051' AS 'UKWPHONENUMBER',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  'Appointment reminder' AS 'VeryRemind'
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 11 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_DiaryEntDateIsTomorrow(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.Email,
  dia.DiaryID,
  dia.EventDate,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  mdl.[DESCRIPTION],
  ctm.ADDR1,
  ctm.ADDR2,
  ctm.ADDR3,
  ctm.POSTCODE,
  eng.ENGINEERID,
  eng.TELNO,
  eng.DISPLAYNAME,
  eng.NAME,
  ftr.footer,
  rcl.RetailClientName,
  rcl.Domain
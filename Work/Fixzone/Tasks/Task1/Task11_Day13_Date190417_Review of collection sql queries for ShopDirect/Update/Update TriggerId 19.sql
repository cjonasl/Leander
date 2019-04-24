UPDATE [Triggers]
SET [TRIGGERSQL] =
'SELECT 
  ctm.EMAIL AS MESSAGESRV_COUREPV_HTML_EMAIL,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''CourierDeliveryDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  COALESCE(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  COALESCE(ctm.ADDR1, '''') AS ''ADDR1'' , 
  COALESCE(ctm.ADDR2, '''') AS ''ADDR2'', 
  COALESCE(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWTELEPHONENUMBER'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%rpg'' THEN ''Inspection complete'' ELSE ''Repair complete'' END AS ''VeryCourierRepaired'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%rpg'' THEN ''inspection'' ELSE ''repair'' END AS ''VeryTermType''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 19 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN Customer ctm ON ser.CUSTOMERID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD 
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, ''2017-12-02'', ''2100'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Complete'') = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_ValueExists(ftr.footer) = 1
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1'
WHERE
  TRIGGERID = 19
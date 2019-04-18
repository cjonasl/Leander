USE [ShopDirect_test]
GO

UPDATE [Triggers]
SET [TRIGGERSQL] =
'SELECT
  ctm.EMAIL AS MESSAGESRV_SURVEYV_HTML_EMAIL,
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  COALESCE(mdl.[DESCRIPTION], ''item'') AS ''DESC'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ftr.footer AS Footer,
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain + ''/Survey'' AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''We''''d really like to know what you thought of us'' AS ''VerySurvey'',
  CASE
    WHEN (cap.POLICYNUMBER LIKE ''%ESP'') THEN ''Service Request''
	WHEN (cap.POLICYNUMBER LIKE ''%MPI'') THEN  ''claim'' ELSE  ''Claim''
  END AS ''PolicyType''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 21 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN Customer ctm ON ser.CUSTOMERID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0 --Record is not already handled
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, ''2017-12-01'', getdate()) = 1 --EventDate between 2017-12-01 and current date
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1 --Retailer is Very
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1 --Customer email exists
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Complete'') = 1 --The service is complete
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 --Entitled service type
  AND dbo.fnFilter_ValueExists(ftr.footer) = 1 --Footer exists
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1 --Domain exists'
WHERE
  TRIGGERID = 21
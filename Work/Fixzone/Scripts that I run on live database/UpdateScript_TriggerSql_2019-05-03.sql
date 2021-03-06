﻿--Ran ok 2019-05-03 kl. 17:25
UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_NEWSGV_HTML_EMAIL'', 
  new.CustomerID,
  new.CustAplID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  cap.PolicyNumber,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''WebLink'',
  rcl.Domain AS ''Domain'',
  ''Service Guarantee'' AS ''VeryNewSG'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Thanks for purchasing a Service Guarantee'' AS ''VeryNewSG''
FROM
  NewCustAplForCustomer new
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 10 AND res.TRIGGERFIELDLAST = ''CustAplID'' AND res.TriggerValue = new.CustAplID
  LEFT JOIN Custapl cap ON new.CustomerID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 10

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25 
  ctm.Email AS ''MESSAGESRV_REMINDV_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  eng.ENGINEERID, 
  ISNULL(REPLACE(eng.TELNO, ''*'', ''''), ''0800 092 9051'') AS ''TELNO'',   
  ISNULL(eng.DISPLAYNAME, eng.NAME) AS ''NAME'', 
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Appointment reminder'' AS ''VeryRemind''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 11 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
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
  rcl.Domain'
WHERE TRIGGERID = 11

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_CANCELV_HTML_EMAIL'',  
  can.CancelledID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ''Appointment cancellation'' AS ''CANCELV''
FROM
  CustomerCancelLog can
  LEFT JOIN [service] ser ON can.ServiceID = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 12 AND res.TRIGGERFIELDLAST = ''CancelledId'' AND res.TriggerValue = can.CancelledID
  LEFT JOIN DiaryEnt dia ON ser.SERVICEID = dia.TagInteger1
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Cancelled'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 12

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DEPOTV_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has arrived'' AS ''VeryDepot''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 17 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 17

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_COUREPV_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''CourierDeliveryDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Complete'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 19

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.Email AS ''MESSAGESRV_INITCONFV_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Service Request raised'' AS ''VeryIntialAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 2 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 8) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.TITLE,
  ctm.Email,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ctm.ADDR1,
  ctm.ADDR2,
  ctm.ADDR3,
  ctm.POSTCODE,
  dia.DiaryID,
  dia.EventDate, 
  mdl.[DESCRIPTION],
  ftr.footer,
  rcl.RetailClientName,
  rcl.Domain'
WHERE TRIGGERID = 2

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_COURDESV_HTML_EMAIL'',
  dia.DiaryID,  
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS  ''DESC'',
  eng.ENGINEERID,
  ISNULL(eng.TELNO,''0800 092 9051'') AS ''TELNO'', 
  ''DPD'' AS ''CourierName'', 
  ''5148870299'' AS ''COURIERTRACKING'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has been despatched'' AS ''VeryCourierDespatch''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 20 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Dispatch'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 20

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_INITCONFV_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ''Service Request raised'' AS ''IntialAppt''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 23 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1'
WHERE TRIGGERID = 23

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DEPOTVRPG_HTML_EMAIL'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%RPG'' THEN ''inspection'' ELSE ''repair'' END AS ''TermType'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') +'' has arrived'' AS ''VeryDepotSub''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 24 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 24

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_CONFBKV_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  rcl.RetailClientName AS ''Brand'',
  ''Confirmation of Service Request booking'' AS ''CONFBKV''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 28 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 28

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_COURV_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(pap.[DESC], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ''VERY'' AS ''Brand'',
  ''Collection confirmation'' AS ''COURV''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 29 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1'
WHERE TRIGGERID = 29

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_REMINDV_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(pap.[DESC], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  eng.ENGINEERID, 
  ISNULL(eng.TELNO, ''0800 092 9051'') AS ''TELNO'', 
  ISNULL(eng.DISPLAYNAME, eng.NAME) AS ''NAME'', 
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  rcl.RetailClientName AS ''Brand'',
  ''Appointment reminder'' AS ''Remind''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 30 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsTomorrow(dia.EventDate) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1'
WHERE TRIGGERID = 30

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_NEWSGLW_HTML_EMAIL'',
  new.CustAplID,
  new.CustomerID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  cap.PolicyNumber,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''WebLink'',
  rcl.Domain AS ''Domain'',
  ''Service Guarantee'' AS ''VeryNewSG'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Thanks for purchasing a Service Guarantee'' AS ''LWNewSG''
FROM
  NewCustAplForCustomer new
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 33 AND res.TRIGGERFIELDLAST = ''CustAplID'' AND res.TriggerValue = new.CustAplID
  LEFT JOIN Custapl cap ON new.CustomerID = cap.CUSTAPLID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 33

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.Email AS ''MESSAGESRV_INITCONFL_HTML_EMAIL'',                          
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Service Request raised'' AS ''LittlewoodsIntialAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 34 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 8) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.TITLE,
  ctm.Email,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ctm.ADDR1,
  ctm.ADDR2,
  ctm.ADDR3,
  ctm.POSTCODE,
  dia.DiaryID,
  dia.EventDate, 
  mdl.[DESCRIPTION],
  ftr.footer,
  rcl.RetailClientName,
  rcl.Domain'
WHERE TRIGGERID = 34

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25 
  ctm.Email AS ''MESSAGESRV_REMINDL_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  eng.ENGINEERID, 
  ISNULL(REPLACE(eng.TELNO, ''*'', ''''), ''0800 092 9051'') AS ''TELNO'',   
  ISNULL(eng.DISPLAYNAME, eng.NAME) AS ''NAME'', 
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Appointment reminder'' AS ''LWRemind''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 37 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
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
  rcl.Domain'
WHERE TRIGGERID = 37

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_COURLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE, 
  CASE
    WHEN cap.POLICYNUMBER like ''%ESP'' THEN ''SERVICE GUARANTEE''
    WHEN cap.POLICYNUMBER like ''%RPG'' THEN ''REPLACEMENT GUARANTEE'' 
    WHEN cap.POLICYNUMBER like ''%MPI'' THEN ''MOBILE PHONE INSURANCE''
  END AS ''Subject'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Collection confirmation'' AS ''LWCourier''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 39 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 39

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_EmlCustVFP_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS ''EnroleCode'',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain,
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Password Reset'' AS ''VeryPasswordReset''
FROM 
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 4 AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 1) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  rcl.RetailClientName,
  rcl.Domain,
  ctm.FIRSTNAME,
  ctm.SURNAME'
WHERE TRIGGERID = 4

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_COUREPLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''CourierDeliveryDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWTELEPHONENUMBER'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%rpg'' THEN ''Inspection complete'' ELSE ''Repair complete'' END AS ''LWCourierRepaired'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%rpg'' THEN ''inspection'' ELSE ''repair'' END AS ''LWTermType''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 40 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Complete'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 40

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_COURDESLW_HTML_EMAIL'',
  dia.DiaryID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  eng.ENGINEERID, 
  ISNULL(eng.TELNO,''0800 092 9051'') AS ''TELNO'', 
  ''DPD'' AS ''CourierName'', 
  ''5148870299'' AS ''COURIERTRACKING'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has been despatched'' AS ''LWCourierDespatch''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 41 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Dispatch'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 41

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_COURLW_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(pap.[DESC], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Collection confirmation'' AS ''LWCourier''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 42 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1 
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 42

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_CANCELLW_HTML_EMAIL'',  
  can.CancelledID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ''Appointment cancellation'' AS ''CancelLW''
FROM
  CustomerCancelLog can
  LEFT JOIN [service] ser ON can.ServiceID = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 43 AND res.TRIGGERFIELDLAST = ''CancelledId'' AND res.TriggerValue = can.CancelledID
  LEFT JOIN DiaryEnt dia ON ser.SERVICEID = dia.TagInteger1
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Cancelled'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 43

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.EMAIL AS ''MESSAGESRV_CONFBKLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Confirmation of Service Request booking'' AS ''CONFBKLW''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 44 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 44

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DEPOTLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has arrived'' AS ''LWDepot''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 46 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 46

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DEPOTLWRPG_HTML_EMAIL'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%RPG'' THEN ''inspection'' ELSE ''repair'' END AS ''TermType'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') +'' has arrived'' AS ''LWDepotRPG''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 47 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 47

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_EMLCUSTLFP_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS ''EnroleCode'',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain,
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Password Reset'' AS ''LWPasswordReset''
FROM 
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 5 AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 1) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  rcl.RetailClientName,
  rcl.Domain,
  ctm.FIRSTNAME,
  ctm.SURNAME'
WHERE TRIGGERID = 5

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  his.Userid AS ''MESSAGESRV_EMLCUSTVPC_HTML_EMAIL'',                        
  his._id,
  his.Created,
  ctm.RetailClientID,
  ctm.CUSTOMERID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your password has been changed'' AS ''VeryPasswordChanged''
FROM
  UserPasswordChangeHistory his
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 6 AND res.TRIGGERFIELDLAST = ''_id'' AND res.TriggerValue = his._id
  INNER JOIN Customer ctm ON his.CustomerClientRef = ctm.CLIENTCUSTREF AND his.Userid = ctm.EMAIL
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(his.Userid) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  his.Userid,
  his._id,
  his.Created,
  ctm.RetailClientID,
  ctm.CUSTOMERID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ftr.footer,
  rcl.RetailClientName,
  rcl.Domain'
WHERE TRIGGERID = 6

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_Depot_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has arrived'' AS ''VeryDepot'',
  CASE
    WHEN (cap.POLICYNUMBER LIKE ''%ESP'') THEN  ''Service Guarantee''
    WHEN (cap.POLICYNUMBER LIKE ''%MPI'') THEN ''Mobile Phone Insurance''
	ELSE ''Replacement Guarantee''
  END AS ''subjectCA'',
  CASE
    WHEN (cap.POLICYNUMBER LIKE ''%ESP'') THEN ''Don�t forget you can also track the progress by logging in to the Online Service Centre using the link below.''
    ELSE ''''
  END AS ''SerGuarText'',
  CASE
    WHEN (cap.POLICYNUMBER LIKE ''%ESP'') THEN rcl.Domain
    ELSE ''''
  END AS ''Domain''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 61 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 61

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_COUREPV_TEXT_SMS'',  
  dia.DiaryID,
  CONVERT(char(10), DiaryEnt.EventDate, 103) AS ''CourierDeliveryDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'',
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'',
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWTELEPHONENUMBER'',
  ''Repair complete'' AS ''CourierRepaired''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 63 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Complete'') = 1
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 63

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.EMAIL AS ''MESSAGESRV_EMLCTemp_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS ''EnroleCode'',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain + ''/account/Enrolment?EnrolmentCode='' + CAST(enr.EnroleCode AS varchar(200) ) AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  COUNT(cap.APPLIANCECD) AS ''AppCount''
INTO #EnroleEmail_Very
FROM
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON (res.TRIGGERID = 1 OR res.TRIGGERID = 69 OR res.TRIGGERID = 71) AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-12-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ftr.Footer,
  rcl.RetailClientName,
  rcl.Domain
ORDER BY
  MIN(cap.CONTRACTDT)   
                           
SELECT
  eev.MESSAGESRV_EMLCTemp_HTML_EMAIL,
  eev.EnroleID, 
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  ISNULL(mdl.[DESCRIPTION], ''Electrical Items'') AS ''ElectricalItem'',
  eev.footer AS ''Footer'',
  eev.Brand AS ''Brand'',
  eev.Domain AS ''Domain'',
  eev.Logo AS ''Logo'',               
  ''Welcome to the Service Guarantee Online Service Centre'' AS ''VeryEnrollment''
FROM
  #EnroleEmail_Very eev       
  LEFT JOIN Customer ctm ON eev.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-12-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1'
WHERE TRIGGERID = 69

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  his.Userid AS ''MESSAGESRV_EmlCustLWP_HTML_EMAIL'',                        
  his._id,
  his.Created,
  ctm.RetailClientID,
  ctm.CUSTOMERID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your password has been changed'' AS ''LWPasswordChanged''
FROM
  UserPasswordChangeHistory his
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 7 AND res.TRIGGERFIELDLAST = ''_id'' AND res.TriggerValue = his._id
  INNER JOIN Customer ctm ON his.CustomerClientRef = ctm.CLIENTCUSTREF AND his.Userid = ctm.EMAIL
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(his.Userid) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  his.Userid,
  his._id,
  his.Created,
  ctm.RetailClientID,
  ctm.CUSTOMERID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ftr.footer,
  rcl.RetailClientName,
  rcl.Domain'
WHERE TRIGGERID = 7

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.EMAIL AS ''MESSAGESRV_EMLWTEMP_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS ''EnroleCode'',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  COUNT(cap.APPLIANCECD) AS ''AppCount''
INTO #EnroleEmail_LW
FROM
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON (res.TRIGGERID = 3 OR res.TRIGGERID = 70 OR res.TRIGGERID = 72) AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-12-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME
ORDER BY
  MIN(cap.CONTRACTDT)   
                           
SELECT
  eev.MESSAGESRV_EMLWTEMP_HTML_EMAIL,
  eev.EnroleID, 
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  ISNULL(mdl.[DESCRIPTION], ''Electrical Items'') AS ''ElectricalItem'',              
  ''Welcome to the Service Guarantee Online Service Centre'' AS ''LWEnrolment''
FROM
  #EnroleEmail_LW eev
  LEFT JOIN Customer ctm ON eev.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-12-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1'
WHERE TRIGGERID = 70

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 30
  ctm.EMAIL AS ''MESSAGESRV_EMLBESV_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS char(36)) AS ''EnroleCode'',
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  CASE WHEN acc.ID IS NULL THEN rcl.Domain + ''/account/Enrolment?EnrolmentCode='' + CAST(enr.EnroleCode AS char(200)) ELSE rcl.Domain + ''/account/SignIn'' END AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  CASE WHEN cap.CONTRACTDT < ''2018-12-01'' THEN ''Don&#39;t forget you have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'' ELSE ''You now have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'' END AS ''Content'',
  COUNT(cap.APPLIANCECD) AS ''AppCount''
INTO #EnroleEmail_VeryTemp
FROM
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON (res.TRIGGERID = 71 OR res.TRIGGERID = 1 OR res.TRIGGERID = 69) AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN CustomerAccount acc ON enr.CustomerID = acc.CustomerID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', ''2018-11-30'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ftr.Footer,
  rcl.RetailClientName,
  rcl.Domain,
  cap.CONTRACTDT,
  acc.ID
ORDER BY
  MAX(cap.CONTRACTDT)  

SELECT TOP 25
  eev.MESSAGESRV_EMLBESV_HTML_EMAIL,
  eev.EnroleID, 
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  ISNULL(mdl.[DESCRIPTION], ''Electrical Items'') AS ''ElectricalItem'',
  eev.footer AS ''Footer'',
  eev.Brand AS ''Brand'',
  eev.Domain AS ''Domain'',
  eev.Logo AS ''Logo'',
  ''Welcome to the Service Guarantee Online Service Centre'' AS ''VeryEnrollment'',
  eev.Content
FROM
  #EnroleEmail_VeryTemp eev
  LEFT JOIN Customer ctm ON eev.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', ''2018-11-30'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
GROUP BY
  eev.MESSAGESRV_EMLBESV_HTML_EMAIL,
  eev.EnroleID,
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  mdl.[DESCRIPTION],
  eev.footer,
  eev.Brand,
  eev.Domain,
  eev.Logo,
  eev.Content'
WHERE TRIGGERID = 71

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 30
  ctm.EMAIL AS ''MESSAGESRV_EMLBESLW_HTML_EMAIL'',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS char(36)) AS ''EnroleCode'',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  CASE WHEN acc.ID IS NULL THEN rcl.Domain + ''/account/Enrolment?EnrolmentCode='' + CAST(enr.EnroleCode AS char(200)) ELSE rcl.Domain + ''/account/SignIn'' END AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  CASE WHEN cap.CONTRACTDT < ''2018-12-01'' THEN ''Don&#39;t forget you have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'' ELSE ''You now have access to our Product Support within the Online Service Centre where you will find set up support, hints and tips and annual health checks or if you experience any faults with your electrical item in the future you can raise a Service Request.'' END AS ''Content'',
  COUNT(cap.APPLIANCECD) AS ''AppCount''
INTO #EnroleEmail_LWTemp
FROM
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON (res.TRIGGERID = 72 OR res.TRIGGERID = 2 OR res.TRIGGERID = 70) AND res.TRIGGERFIELDLAST = ''EnroleID'' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN CustomerAccount acc ON enr.CustomerID = acc.CustomerID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', ''2018-11-30'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME,
  ftr.Footer,
  rcl.RetailClientName,
  rcl.Domain,
  cap.CONTRACTDT,
  acc.ID
ORDER BY
  MAX(cap.CONTRACTDT)  

SELECT TOP 25
  eev.MESSAGESRV_EMLBESLW_HTML_EMAIL,
  eev.EnroleID,
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  ISNULL(mdl.[DESCRIPTION], ''Electrical Items'') AS ''ElectricalItem'',
  eev.footer AS ''Footer'',
  eev.Brand AS ''Brand'',
  eev.Domain AS ''Domain'',
  eev.Logo AS ''Logo'',
  ''Welcome to the Service Guarantee Online Service Centre'' AS ''LWEnrolment'',
  eev.Content
FROM
  #EnroleEmail_LWTemp eev
  LEFT JOIN Customer ctm ON eev.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, ''2019-01-01'', ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', ''2018-11-30'') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
GROUP BY
  eev.MESSAGESRV_EMLBESV_HTML_EMAIL,
  eev.EnroleID,
  eev.EnroleCode,
  eev.CustomerID,
  eev.CustomerName,
  mdl.[DESCRIPTION],
  eev.footer,
  eev.Brand,
  eev.Domain,
  eev.Logo,
  eev.Content'
WHERE TRIGGERID = 72

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 25
  ctm.Email AS ''MESSAGESRV_CONFBKV_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Confirmation of Service Request booking'' AS ''VeryConfAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 8 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), ''2100-01-01'') = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 10) = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 0
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 8

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_COURV_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ISNULL(ctm.ADDR1, '''') AS ''ADDR1'', 
  ISNULL(ctm.ADDR2, '''') AS ''ADDR2'', 
  ISNULL(ctm.ADDR3, '''') AS ''ADDR3'',
  ctm.POSTCODE,
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Collection confirmation'' AS ''VeryCourier''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 9 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_EntitledEngineer(eng.DumpDiary) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_EligibleForCourierCollection(pap.MONITORFG) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 9


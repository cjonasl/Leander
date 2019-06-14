--Ran ok 2019-06-12 kl. 16:19
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
  ''Thanks for purchasing a Service Guarantee'' AS ''VeryNewSG'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Thanks for purchasing a Service Guarantee'' AS ''VeryNewSG''
FROM
  NewCustAplForCustomer new
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 10 AND res.TRIGGERFIELDLAST = ''CustAplID'' AND res.TriggerValue = new.CustAplID
  LEFT JOIN Custapl cap ON new.CustAplID = cap.CUSTAPLID
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
  and ser.VISITCD=''000''
  AND dbo.fnFilter_DiaryEntDateIsTomorrow(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null)
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1   and ser.VISITCD=''000''
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Cancelled'') = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 12

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_FAILEDV_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ''We missed you'' AS ''VeryFailedAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 14 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND (( ser.STATUSID=8 and ser.substatus=23) or ( ser.STATUSID=29 and ser.substatus=2) )
  
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 14

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DELAYEDV_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Service Request Reschedule'' AS ''VeryDelayedAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 15 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ( ser.STATUSID=29 and ser.substatus=3)

  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 15

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.Email AS ''MESSAGESRV_COMPLETEV_HTML_EMAIL'',
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'', 
  ''Job Completed'' AS ''VeryJobComplete''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 16 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0 
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND (ser.STATUSID=8 and ser.substatus  in (2,7,1,6) )
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 16

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
  
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.statusid=18 and ser.substatus=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 17

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_B2BV_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ''0800 092 9051'' AS UKWPHONENUMBER,  
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' is with an engineer'' AS ''VeryB2B'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%ESP'' THEN ''Don&#39;t forget you can track the progress of your repair by logging in to the Online Service Centre <a href='' + rcl.Domain + ''>here</a>.'' ELSE '''' END AS ''SGText'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%RPG'' THEN ''inspected'' ELSE ''repaired'' END AS ''RGText''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 18 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
   AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1 
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
      AND ( ser.STATUSID=18 and ser.substatus=6)
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 18

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
 AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  and Ser.statusid=18 and ser.substatus=3
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
  and ser.visitcd=''000''
  AND (( ser.statusid=3 and ser.substatus=0)  or ser.statusid=4)
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 2) = 1
  AND dbo.fnFilter_NotServiceStatus(ser.Statusid, 8) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1  
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1

  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
   AND ser.STATUSID =5 and ser.substatus=20  --Engineer allocated (5)�   & �collection booked (20)�
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
  and ser.visitcd=''000''
  AND (( ser.statusid=3 and ser.substatus=0)  or ser.statusid=4)
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1

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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
   AND ser.statusid=18 and ser.substatus=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 24

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.Email AS ''MESSAGESRV_CLAIMEDV_HTML_EMAIL'',
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Claim closed'' AS ''VeryClaimClosed''
FROM
  DiaryEnt dia
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 26 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, ''2017-12-01'', getdate()) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.STATUSID=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 26

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
  AND ser.Statusid= 5  AND ser.substatus= 0
  AND ser.VISITCD=''000''
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND Ser.VISITCD=''016''
  AND dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null)
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
    AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1   and ser.VISITCD=''000''
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1'
WHERE TRIGGERID = 30

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_FAILEDV_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',	 
  ''0800 092 9051'' AS ''UKWPHONENUMBER'', 
   rcl.RetailClientName AS ''Brand'',
   rcl.Domain AS ''Domain'',
  ''We missed you'' AS ''FailedAppt''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 31 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
   AND (( ser.STATUSID=8 and ser.substatus=23) or ( ser.STATUSID=29 and ser.substatus=2) )
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 31

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
ctm.EMAIL AS MESSAGESRV_B2BRPGV_HTML_EMAIL,  
 dia.DiaryID,
    CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(pap.[DESC], ''Product'') AS ''DESC'', 
  ''0800 092 9051'' AS UKWPHONENUMBER, 
  ''VERY'' AS Brand, 
  ''Your ''+COALESCE(POP_Apl.[DESC], ''Electrical Item'')+'' is with an engineer'' AS VeryB2B
FROM [DiaryEnt] dia
LEFT JOIN TriggerRes res ON res.TRIGGERID = 32 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
    AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
    AND ser.STATUSID=18 and ser.SUBSTATUS=6  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 32

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
 and ser.visitcd=''000''
  AND (( ser.statusid=3 and ser.substatus=0)  or ser.statusid=4)
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  and ser.VISITCD=''000''
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
  AND dbo.fnFilter_WithinDateRange(dia.EnterDate, getdate(), ''2100-01-01'') = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND Ser.VISITCD=''016''
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 39

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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  and Ser.statusid=18 and ser.substatus=3
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1 
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.STATUSID =5 and ser.substatus=20
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND Ser.VISITCD=''016''
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null)
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ServiceStatus(ser.STATUSID, ''Cancelled'') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1 and ser.VISITCD=''000''
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
  AND ser.Statusid= 5  AND ser.substatus= 0
  AND ser.VISITCD=''000''
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 44

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.EMAIL AS ''MESSAGESRV_FAILEDLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  ''We missed you'' AS ''FAILEDLW''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 45 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
   AND (( ser.STATUSID=8 and ser.substatus=23) or ( ser.STATUSID=29 and ser.substatus=2) )
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 45

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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.statusid=18 and ser.substatus=8
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.statusid=18 and ser.substatus=8 
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 47

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.Email AS ''MESSAGESRV_CLAIMEDLW_HTML_EMAIL'',
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'', 
  ''Claim closed'' AS ''LWClaimClosed''
FROM
  DiaryEnt dia
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 48 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, ''2017-12-01'', getdate()) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1  AND ser.STATUSID=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 48

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_DELAYEDLW_HTML_EMAIL'', 
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'', 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Service Request Reschedule'' AS ''LWDelayedAppt''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 50 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND (ser.STATUSID=29 and ser.substatus=3)
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 50

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT     
ctm.Email AS MESSAGESRV_COMPLETELW_HTML_EMAIL,    ser.ServiceID,    dia.DiaryID,    
CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',     
dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',    
ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',    ftr.footer AS ''Footer'',    rcl.RetailClientName AS ''Brand'',    
rcl.Domain AS ''Domain'',    rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',    ''0800 092 9051'' AS ''UKWPHONENUMBER'',     
''Job Completed'' AS ''LWJobComplete''  FROM    DiaryEnt dia     
LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID    
LEFT JOIN TriggerRes res ON res.TRIGGERID = 51 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID    
LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType    
LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID    
LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID   
 LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID   
 LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]   
 LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL 
 WHERE   
 dbo.fnFilter_ValueExists(res.id) = 0
 AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
 AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1   
 AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
 AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1    
 AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
 AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
 AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1    
 AND ser.STATUSID=8
 AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 51

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_B2BLW_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ''0800 092 9051'' AS UKWPHONENUMBER,  
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' is with an engineer'' AS ''LWB2B'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%ESP'' THEN ''Don&#39;t forget you can track the progress of your repair by logging in to the Online Service Centre <a href='' + rcl.Domain + ''>here</a>.'' ELSE '''' END AS ''SGText'',
  CASE WHEN cap.POLICYNUMBER LIKE ''%RPG'' THEN ''inspected'' ELSE ''repaired'' END AS ''RGText''
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 52 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.STATUSID=18 and ser.SUBSTATUS=6
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 52

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  ctm.EMAIL AS ''MESSAGESRV_B2BLWRPG_HTML_EMAIL'',
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(pap.[DESC], ''Product'') AS ''DESC'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'', 
  ''Littlewoods'' AS ''Brand'', 
  ''Your ''+ ISNULL(pap.[DESC], ''Product'') + '' is with an engineer'' AS ''LWB2BRPG''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 54 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN Enginrs eng ON dia.UserID = eng.EngineerId
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Replacement Guarantee'') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Mobile'') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND ser.STATUSID = 18 and ser.SUBSTATUS=6
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 54

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  ctm.Email AS ''MESSAGESRV_CLAIMEDLWS_HTML_EMAIL'',
  ser.ServiceID,
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Claim closed'' AS ''LWClaimClosed''
FROM
  DiaryEnt dia
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 56 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, ''2017-12-01'', getdate()) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Littlewoods'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1   AND ser.STATUSID=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 56

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_Delayed_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'', 
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',  
  ftr.footer AS ''Footer'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Service Guarantee'' AS ''Delayed''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 59 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND ((ser.STATUSID=8 AND ser.substatus=23) OR (ser.STATUSID=29 and ser.substatus=2))
  AND dbo.fnFilter_ServiceSubStatus(ser.SUBSTATUS, 1) = 2
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 59

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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1  and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND ser.statusid=18 and ser.substatus=8
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 61

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_COURDESV_TEXT_SMS'',  
  dia.DiaryID,  
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  eng.ENGINEERID, 
  ISNULL(eng.TELNO, ''0800 092 9051'') AS ''TELNO'', 
  ''DPD'' AS ''CourierName'', 
  ''5148870299'' AS ''COURIERTRACKING'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
   ftr.footer AS ''Footer'',
   rcl.RetailClientName AS ''Brand'',
   rcl.Domain AS ''Domain'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' has been despatched'' AS ''VeryCourierDespatch''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 62 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND ser.STATUSID =5 and ser.substatus=20
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 62

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_COUREPV_TEXT_SMS'',  
  dia.DiaryID,
  CONVERT(char(10), dia.EventDate, 103) AS ''CourierDeliveryDate'', 
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND Ser.statusid=18 and ser.substatus=3
  AND dbo.fnFilter_ValueExists(rcl.Domain) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 63

UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS ''MESSAGESRV_B2B_TEXT_SMS'',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS ''EventDate'',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS ''CustomerName'',
  ISNULL(mdl.[DESCRIPTION], ''Product'') AS ''DESC'',
  ''0800 092 9051'' AS ''UKWPHONENUMBER'',
  rcl.RetailClientName AS ''Brand'',
  rcl.Domain AS ''Domain'',
  rcl.Domain + ''/Content/img/ClientLogo.png'' AS ''Logo'',
  ''Your ''+ ISNULL(mdl.[DESCRIPTION], ''Product'') + '' is with an engineer being repaired.'' AS ''VeryB2B'',
  CASE 
    WHEN cap.POLICYNUMBER LIKE ''%ESP'' THEN ''You can track the progress of your repair by logging in to the Online Service Centre using the link below . '' + rcl.Domain
    ELSE ''We will update you shortly.''
  END
  AS ''SGText''
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 64 AND res.TRIGGERFIELDLAST = ''DiaryID'' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND ser.STATUSID=18 and ser.SUBSTATUS=6  
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 64

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
  AND ser.Statusid= 5  AND ser.substatus= 0-- ENgineer allocated
  AND ser.VISITCD=''000''--Newservice call 
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, ''Service Guarantee'') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
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
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND Ser.VISITCD=''016''
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, ''2018-01-29'', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, ''SDPOLICY'') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, ''Very'') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1'
WHERE TRIGGERID = 9


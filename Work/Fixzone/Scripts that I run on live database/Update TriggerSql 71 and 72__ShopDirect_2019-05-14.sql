--Ran ok 2019-05-14 kl. 16:07
UPDATE [Triggers]
SET TRIGGERSQL = 'SELECT TOP 30
  ctm.EMAIL AS MESSAGESRV_EMLBESV_HTML_EMAIL,
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
  MIN(cap.CONTRACTDT)  

SELECT TOP 30
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
  ctm.EMAIL AS MESSAGESRV_EMLBESLW_HTML_EMAIL,
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
  MIN(cap.CONTRACTDT)  

SELECT TOP 30
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
  eev.MESSAGESRV_EMLBESLW_HTML_EMAIL,
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
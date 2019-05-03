SELECT
  ctm.EMAIL AS 'MESSAGESRV_HEALTHV_HTML_EMAIL',
  ann.TriggerValue AS 'HealthId',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC',
  cap.POLICYNUMBER,
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  'Your Annual Health Check for your product' AS 'HealthCheckVery'
FROM
  AnnualHealthCheck ann
  INNER JOIN Custapl cap ON ann.CustAplId = cap.CUSTAPLID
  INNER JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 13 AND res.TRIGGERFIELDLAST = 'HealthID' AND res.TriggerValue = ann.TriggerValue
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  ann.TriggerId = 13
  AND dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
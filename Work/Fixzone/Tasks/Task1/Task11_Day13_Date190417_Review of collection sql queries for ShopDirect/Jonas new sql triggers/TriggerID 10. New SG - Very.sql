SELECT
  ctm.EMAIL AS 'MESSAGESRV_NEWSGV_HTML_EMAIL', 
  new.CustomerID,
  new.CustAplID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(pap.[DESC], 'Product') AS 'DESC',
  cap.PolicyNumber,
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'WebLink',
  rcl.Domain AS 'Domain',
  'Service Guarantee' AS 'VeryNewSG',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo'
FROM
  NewCustAplForCustomer new
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 10 AND res.TRIGGERFIELDLAST = 'CustAplID' AND res.TriggerValue = new.CustAplID
  LEFT JOIN Custapl cap ON new.CustomerID = cap.CUSTAPLID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type] 
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
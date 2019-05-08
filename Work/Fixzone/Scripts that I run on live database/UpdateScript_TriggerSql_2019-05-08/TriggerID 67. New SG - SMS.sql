SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS MESSAGESRV_NewSGSMS_TEXT_SMS,
  'This Service Guarantee for your '+ ISNULL(mdl.[DESCRIPTION], 'Product') + ' policy number '+ cap.POLICYNUMBER + ' will be added into your Online Service Centre account.' AS 'NewSGVSMSTEXT', 
  new.CustomerID,
  new.CustAplID,
  rcl.RetailClientName AS 'Brand',
  'Service Guarantee' AS 'NewSGSMS'
FROM
  NewCustAplForCustomer new
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 67 AND res.TRIGGERFIELDLAST = 'CustAplID' AND res.TriggerValue = new.CustAplID
  LEFT JOIN Custapl cap ON new.CustomerID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
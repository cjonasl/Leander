SELECT
  his.Userid AS MESSAGESRV_EMLCUSTVPC_HTML_EMAIL,
  his._id,
  his.Created,
  ctm.RetailClientID,
  ctm.CUSTOMERID,
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  'Your password has been changed' AS 'VeryPasswordChanged'
FROM
  UserPasswordChangeHistory his
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 6 AND res.TRIGGERFIELDLAST = '_id' AND res.TriggerValue = his._id
  INNER JOIN Customer ctm ON his.CustomerClientRef = ctm.CLIENTCUSTREF AND his.Userid = ctm.EMAIL
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
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
  rcl.Domain
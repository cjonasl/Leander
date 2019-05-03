SELECT TOP 25
  ctm.EMAIL AS 'MESSAGESRV_EMLCTemp_HTML_EMAIL',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS 'EnroleCode',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain + '/account/Enrolment?EnrolmentCode=' + CAST(enr.EnroleCode AS varchar(200) ) AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  COUNT(cap.APPLIANCECD) AS 'AppCount'
INTO #EnroleEmail_Very
FROM
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON (res.TRIGGERID = 1 OR res.TRIGGERID = 69 OR res.TRIGGERID = 71) AND res.TRIGGERFIELDLAST = 'EnroleID' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Footer ftr ON RIGHT(cap.policynumber, 3) = ftr.[Type]
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-12-01', '2100-01-01') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, '2019-01-01', '2100-01-01') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
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
  ISNULL(mdl.[DESCRIPTION], 'Electrical Items') AS 'ElectricalItem',
  eev.footer AS 'Footer',
  eev.Brand AS 'Brand',
  eev.Domain AS 'Domain',
  eev.Logo AS 'Logo',               
  'Welcome to the Service Guarantee Online Service Centre' AS 'VeryEnrollment'
FROM
  #EnroleEmail_Very eev       
  LEFT JOIN Customer ctm ON eev.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CREATEDDATETIME, '2019-01-01', '2100-01-01') = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-12-01', '2100-01-01') = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ContractNotCancelled(cap.CONTRACTCANCELDATE) = 1
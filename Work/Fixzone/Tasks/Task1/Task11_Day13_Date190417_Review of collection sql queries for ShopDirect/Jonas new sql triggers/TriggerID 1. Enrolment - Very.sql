SELECT TOP 25
  ctm.EMAIL AS 'MESSAGESRV_EMAILCUST_HTML_EMAIL',
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
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 1 AND res.TRIGGERFIELDLAST = 'EnroleID' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_WithinDateRange(cap.SUPPLYDAT, '2018-07-01', '2100-01-01') = 1
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

SELECT 
  EEV.MESSAGESRV_EmailCust_HTML_EMAIL, 
  EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,
  COALESCE(model.[DESCription], 'Electrical Items') AS ElectricalItem,
  EEV.footer as Footer,
  EEV.Brand as Brand,
  EEV.Domain AS [Domain],
  EEV.Logo as Logo,
  'Welcome to the Service Guarantee Online Service Centre' AS VeryEnrollment
FROM #EnroleEmail_Very EEV
LEFT JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID 
LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID ANd Custapl.POLICYNUMBER like '%ESP' AND AppCount=1 AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.MODEL = custapl.MODEL and model.MFR=custapl.MFR
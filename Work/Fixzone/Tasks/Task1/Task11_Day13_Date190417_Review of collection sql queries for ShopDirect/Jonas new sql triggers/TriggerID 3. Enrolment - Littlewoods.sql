SELECT TOP 25
  ctm.EMAIL AS 'MESSAGESRV_EMAILCUSTL_HTML_EMAIL',
  enr.EnroleID, 
  CAST(enr.EnroleCode AS CHAR(36)) AS 'EnroleCode',    
  enr.CustomerID, 
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  COUNT(cap.APPLIANCECD) AS 'AppCount'
INTO #EnroleEmail_LW 
FROM 
  CustomerEnrolment enr
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 3 AND res.TRIGGERFIELDLAST = 'EnroleID' AND res.TriggerValue = enr.EnroleID
  LEFT JOIN Customer ctm ON enr.CustomerID = ctm.CUSTOMERID
  LEFT JOIN Custapl cap ON ctm.CUSTOMERID = ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID)
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_CustomerEnrolmentIsValid(enr.ValidFlag) = 1
  AND dbo.fnFilter_ValidLinkType(enr.LinkType, 0) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Littlewoods') = 1
  AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1
  AND dbo.fnFilter_ValueExists(ctm.CUSTOMERID) = 1
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_ValueExists(cap.CUSTOMERID) = 1
  AND dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1
  AND dbo.fnFilter_WithinDateRange(cap.SUPPLYDAT, '2018-07-01', '2100-01-01') = 1
  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1
GROUP BY
  ctm.EMAIL,
  enr.EnroleID, 
  enr.EnroleCode,
  enr.CustomerID,
  ctm.TITLE,
  ctm.FIRSTNAME,
  ctm.SURNAME

SELECT 
  EEV.MESSAGESRV_EmailCustL_HTML_EMAIL, 
  EEV.EnroleID, EEV.EnroleCode, EEV.CustomerID, EEV.CustomerName,
  COALESCE(model.Description, 'Electrical Items') AS ElectricalItem, 
  'Littlewoods' AS Brand,
  'Welcome to the Service Guarantee Online Service Centre' AS LWEnrolment
FROM
  #EnroleEmail_LW EEV
  LEFT JOIN Customer ON Customer.CUSTOMERID=EEV.CustomerID 
  LEFT JOIN Custapl ON CustApl.CUSTOMERID=Customer.CUSTOMERID AND AppCount=1 and Custapl.policynumber like '%ESP' AND (Custapl.SUPPLYDAT is not null AND Custapl.SUPPLYDAT > '2018-06-30') 
  LEFT JOIN model ON model.APPLIANCECD=CUSTAPL.APPLIANCECD and model.model=custapl.model and model.MFR =custapl.MFR
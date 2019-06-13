SELECT 
ctm.EMAIL AS MESSAGESRV_B2BRPGV_HTML_EMAIL,  
 dia.DiaryID,
    CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  ftr.footer AS 'Footer',
  rcl.RetailClientName AS 'Brand',
  rcl.Domain AS 'Domain',
  rcl.Domain + '/Content/img/ClientLogo.png' AS 'Logo',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(pap.[DESC], 'Product') AS 'DESC', 
  '0800 092 9051' AS UKWPHONENUMBER, 
  'VERY' AS Brand, 
  'Your '+COALESCE(POP_Apl.[DESC], 'Electrical Item')+' is with an engineer' AS VeryB2B
FROM [DiaryEnt] dia
LEFT JOIN TriggerRes res ON res.TRIGGERID = 32 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
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
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Replacement Guarantee') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Mobile') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
    AND dbo.fnFilter_ValueExists(ctm.EMAIL) = 1  AND dbo.fnFilter_RetailClient(ctm.RetailClientID, 'Very') = 1
    AND ser.STATUSID=18 and ser.SUBSTATUS=6  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
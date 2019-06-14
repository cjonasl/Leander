SELECT 
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS 'MESSAGESRV_BERSMS_TEXT_SMS',
  ser.ServiceId,
  cap.POLICYNUMBER,
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC',	 
  '0800 092 9051' AS 'UKWPHONENUMBER', 
   rcl.RetailClientName AS 'Brand',
   rcl.Domain AS 'Domain'
FROM
  DiaryEnt dia 
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.SERVICEID
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 60 AND res.TRIGGERFIELDLAST = 'ServiceId' AND res.TriggerValue = ser.SERVICEID
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN RetailClient rcl ON ctm.RetailClientID = rcl.RetailCode AND ctm.CLIENTID = rcl.RetailClientID
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_DiaryEntDateIsToday(dia.EventDate) = 1
      AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null)
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND (dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Service Guarantee') = 1 OR
  dbo.fnFilter_PolicyType(cap.POLICYNUMBER, 'Mobile') = 1)
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1
  AND (ser.STATUSID=12 and ser.substatus=7)
  AND dbo.fnFilter_RetailClientID(rcl.RetailClientID, 673) = 1
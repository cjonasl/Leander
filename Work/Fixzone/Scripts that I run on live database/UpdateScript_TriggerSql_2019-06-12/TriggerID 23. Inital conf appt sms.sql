SELECT
  dbo.fn_getCustomerTel(ctm.TEL1, ctm.TEL2, ctm.TEL3) AS 'MESSAGESRV_INITCONFV_TEXT_SMS',  
  dia.DiaryID,  
  CONVERT(char(10), dia.EventDate, 103) AS 'EventDate',
  dbo.fn_getCustomerName(ctm.TITLE, ctm.FIRSTNAME, ctm.SURNAME) AS 'CustomerName',
  ISNULL(mdl.[DESCRIPTION], 'Product') AS 'DESC',
  ISNULL(ctm.ADDR1, '') AS 'ADDR1',
  ISNULL(ctm.ADDR2, '') AS 'ADDR2',
  ISNULL(ctm.ADDR3, '') AS 'ADDR3',
  ctm.POSTCODE,
  'Service Request raised' AS 'IntialAppt'
FROM
  DiaryEnt dia
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 23 AND res.TRIGGERFIELDLAST = 'DiaryID' AND res.TriggerValue = dia.DiaryID
  LEFT JOIN [service] ser ON dia.TagInteger1 = ser.ServiceId
  LEFT JOIN SpecJobMapping map ON ser.VISITCD = map.VisitType
  LEFT JOIN Custapl cap ON ser.CUSTAPLID = cap.CUSTAPLID
  LEFT JOIN Customer ctm ON ISNULL(cap.OwnerCustomerID, cap.CUSTOMERID) = ctm.CUSTOMERID
  LEFT JOIN POP_Apl pap ON cap.APPLIANCECD = pap.APPLIANCECD
  LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE
  dbo.fnFilter_ValueExists(res.id) = 0
  AND dbo.fnFilter_WithinDateRange(dia.EventDate, DATEADD(day, 1, getdate()), '2100-01-01') = 1
  and ser.visitcd='000'
  AND (( ser.statusid=3 and ser.substatus=0)  or ser.statusid=4)
  AND dbo.fnFilter_EntitledServiceType(map.DummyJob) = 1 and (map.Clientid=673 or map._id is null) 
  AND dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
  AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
  AND dbo.fnFilter_CustomerUserID(ctm.UserID, 'SDPOLICY') = 1
  AND dbo.fnFilter_CustomerHas07TelNumber(ctm.TEL1, ctm.TEL2, ctm.TEL3) = 1

  AND dbo.fnFilter_RetailClientID(ctm.CLIENTID, 673) = 1
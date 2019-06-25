UPDATE
  Letter
SET
  LetterName = 'AEP delivery notification',
  LetterDesc = 'TNT delivery',
  LetterBody = '
  TNT will delivering your AEP shortly, details as follows;

  ETA: ~ETA~
  Job Number: ~JobNumber~
  Case ID: ~CaseID~
  Reservation ID: ~ReservationID~
  Customer Surname: ~CustomerSurname~

  Please ensure the customer is aware and an appointment booked.

  Thanks
  Pacifica Service Desk'
WHERE
  TriggerID = 11
GO

UPDATE [Triggers]
SET TRIGGERSQL =
'SELECT
  sc.EmailAddress AS MESSAGESRV_AEPSENG_TEXT_EMAIL,
  ses.awbDeliveryETA AS ETA,
  ses.ServiceID AS JobNumber,
  ISNULL(ses.caseId, ''?'') AS CaseID,
  ISNULL(ses.awbId, 0) AS ReservationID,
  ISNULL(ses.lastName, ''?'') AS CustomerSurname
FROM
  SonyEventStataus ses
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 11 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ses.ServiceID
  INNER JOIN SAEDICalls job ON CAST(ses.ServiceID AS varchar(20)) = job.ClientRef
  INNER JOIN SAEDIClient sc ON job.SAEDIFromID = sc.SAEDIID
WHERE
  res.id iS NULL AND
  ses.aepType IS NOT NULL AND
  ses.aepType <> ''N/A'' AND
  ses.serviceEventType IS NOT NULL AND
  ses.serviceEventType LIKE ''AEP%'' AND
  ses.successful = 1 AND
  ses.awbDeliveryETA IS NOT NULL AND
  ses.awbDeliveryETA >= DATEADD(day, -2, getdate()) AND
  ses.ServiceID IS NOT NULL AND
  job.SAEDIToID = ''SONY3C'' AND
  sc.EmailAddress IS NOT NULL AND
  sc.EmailAddress LIKE ''%_@__%.__%'' AND
  PATINDEX(''%[^a-z,0-9,@,.,_,\-]%'', sc.EmailAddress) = 0'
WHERE
  [TRIGGERID] = 11
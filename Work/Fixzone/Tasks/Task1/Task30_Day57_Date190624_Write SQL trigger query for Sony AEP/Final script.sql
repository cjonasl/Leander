UPDATE
  Letter
SET
  LetterName = 'Sony AEP/parts despatched ETA job no. ~ServiceID~',
  LetterDesc = 'TNT delivery',
  LetterBody = '
  TNT will delivering your AEP shortly, details as follows;

  ETA: ~ETA~
  Job Number: ~ServiceID~
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
  ses.ServiceID,
  CAST(ses.awbDeliveryETA AS date) AS ETA,
  ISNULL(ses.caseId, ''?'') AS CaseID,
  ISNULL(ses.awbId, 0) AS ReservationID,
  ISNULL(ses.lastName, ''?'') AS CustomerSurname
FROM
  SonyEventStataus ses
  LEFT JOIN TriggerRes res ON res.TRIGGERID = 11 AND res.TRIGGERFIELDLAST = ''ServiceId'' AND res.TriggerValue = ses.ServiceID
  INNER JOIN SAEDICalls job ON ses.mainAscReferenceId = job.ClientRef COLLATE Database_Default
  INNER JOIN SAEDIClient sc ON job.SAEDIFromID = sc.SAEDIID
WHERE
  res.id iS NULL AND
  ISNULL(ses.aepType, ''N/A'') <> ''N/A'' AND
  ISNULL(ses.serviceEventType, '''') LIKE ''AEP%'' AND
  ses.successful = 1 AND
  ses.awbDeliveryETA IS NOT NULL AND
  ses.awbDeliveryETA >= getdate() AND
  ses.ServiceID IS NOT NULL AND
  job.SAEDIToID = ''SONY3C'' AND
  sc.EmailAddress IS NOT NULL AND
  sc.EmailAddress LIKE ''%_@__%.__%'' AND
  PATINDEX(''%[^a-z,0-9,@,.,_,\-]%'', sc.EmailAddress) = 0'
WHERE
  [TRIGGERID] = 11
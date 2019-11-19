ALTER PROCEDURE [dbo].[Report_Jobs]
@ClientId int
AS
BEGIN
SELECT 
  c.[TITLE] AS 'Title',
  c.[SURNAME] AS 'Surname',
  CAST(s1.SERVICEID as varchar(20)) AS 'Job number',
  cap.[APPLIANCECD] AS 'Appliance category',
  cap.MFR AS 'Brand',
  cap.[MODEL] AS 'Model',
  a.[FirstDateoffered] AS 'First date offered',
  a.[DateChosen] AS 'Customer selected date',
  d1.[EventDate] AS 'Date of first visit',
  d2.[EventDate] AS 'Date of second visit',
  d3.[EventDate] AS 'Date of third visit',
  s.[Status] AS 'Job status',
  CASE
    WHEN s1.STATUSID IN(8, 30) THEN CAST(COALESCE(s1.DONEDATE, sn2.[DateTime], s1.[TODAYDATETIME]) AS date)
	ELSE NULL
  END
  AS 'Complete date',
  CASE
    WHEN s1.STATUSID = 2 THEN  COALESCE(CAST(sn1.[DateTime] AS date), d3.[EventDate], d2.[EventDate], d1.[EventDate], CAST(s1.[TODAYDATETIME] AS date))
	ELSE NULL
  END
  AS 'Cancelled date',
  cap.[SNO] AS 'Serial number',
  s1.[CLIENTREF] AS 'Client reference number',
  cap.[POLICYNUMBER] AS 'Policy number'
FROM 
  [service] s1
  INNER JOIN Customer c ON s1.CUSTOMERID = c.CUSTOMERID
  INNER JOIN Custapl cap ON s1.CUSTAPLID = cap.CUSTAPLID
  INNER JOIN [status] s ON s1.STATUSID = s.StatusID
  LEFT OUTER JOIN AppointmentTrack a ON s1.SERVICEID = a.ServiceID
  LEFT OUTER JOIN DiaryEnt d1 ON (s1.SERVICEID = d1.TagInteger1 AND s1.SERVICEID = s1.JOBID)
  LEFT OUTER JOIN [service] s2 ON (s1.SERVICEID <> s2.SERVICEID AND s1.SERVICEID = s2.JOBID)
  LEFT OUTER JOIN DiaryEnt d2 ON s2.SERVICEID = d2.TagInteger1
  LEFT OUTER JOIN [service] s3 ON (s1.SERVICEID <> s3.SERVICEID AND s2.SERVICEID <> s3.SERVICEID AND s1.SERVICEID = s3.JOBID)
  LEFT OUTER JOIN DiaryEnt d3 ON s3.SERVICEID = d3.TagInteger1
  LEFT OUTER JOIN ServiceNotes sn1 ON s1.SERVICEID = sn1.serviceId AND sn1.[notes] LIKE 'Job Cancelled%'
  LEFT OUTER JOIN ServiceNotes sn2 ON s1.SERVICEID = sn2.serviceId AND sn2.Complete IS NOT NULL AND sn2.NotesID =
  (
    SELECT TOP 1 tmp.NotesID
	FROM ServiceNotes tmp
	WHERE tmp.serviceId = s1.SERVICEID
	ORDER BY tmp.[DateTime] desc
  )
WHERE
  s1.CLIENTID = @Clientid
ORDER BY
  s1.SERVICEID

OPTION(RECOMPILE)
END
GO
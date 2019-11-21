CREATE PROCEDURE [dbo].[Report_JobsByStatus]
@ClientId int,
@StatusId int
AS
BEGIN
SELECT
  dbo.fn_Help1GetValue(c.[TITLE]) AS Title,
  dbo.fn_Help1GetValue(c.[SURNAME]) AS Surname,
  CAST(s1.SERVICEID AS varchar(20)) AS JobNumber,
  dbo.fn_Help1GetValue(cap.[APPLIANCECD]) AS ApplianceCategory,
  dbo.fn_Help1GetValue(cap.[MFR]) AS Brand,
  dbo.fn_Help1GetValue(cap.[MODEL]) AS Model,
  dbo.fn_Help2GetValue(a.[FirstDateoffered]) AS FirstDateOffered,
  dbo.fn_Help2GetValue(a.[DateChosen] ) AS CustomerSelectedDate,
  dbo.fn_Help2GetValue(d1.[EventDate] ) AS DateOfFirstVisit,
  dbo.fn_Help2GetValue(d2.[EventDate]) AS DateOfSecondVisit,
  dbo.fn_Help2GetValue(d3.[EventDate]) AS DateOfThirdVisit,
  dbo.fn_Help1GetValue(s.[Status]) AS JobStatus,
  dbo.fn_Help2GetValue(CASE WHEN s1.STATUSID IN(8, 30) THEN CAST(COALESCE(s1.[DONEDATE], sn2.[DateTime], s1.[TODAYDATETIME]) AS date) ELSE NULL END) AS CompleteDate,
  dbo.fn_Help2GetValue(CASE WHEN s1.STATUSID = 2 THEN  COALESCE(CAST(sn1.[DateTime] AS date), d3.[EventDate], d2.[EventDate], d1.[EventDate], CAST(s1.[TODAYDATETIME] AS date)) ELSE NULL END) AS CancelledDate,
  dbo.fn_Help1GetValue(cap.[SNO]) AS SerialNumber,
  dbo.fn_Help1GetValue(s1.[CLIENTREF]) AS ClientReferenceNumber,
  dbo.fn_Help1GetValue(cap.[POLICYNUMBER]) AS PolicyNumber
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
  s1.CLIENTID = @Clientid AND
  s1.STATUSID = @StatusId 
ORDER BY
  s1.SERVICEID

OPTION(RECOMPILE)
END
GO
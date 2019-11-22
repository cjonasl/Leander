ALTER PROCEDURE [dbo].[Report_JobsByStatus]
@ClientId int,
@StatusId int
AS
BEGIN

DECLARE @TmpTableService TABLE
(
  [SERVICEID] int NOT NULL,
  [CUSTOMERID] int NULL,
  [CUSTAPLID] int NULL,
  [STATUSID] smallint NULL,
  [TODAYDATETIME] datetime NULL,
  [DONEDATE] date NULL,
  [JOBID] int NULL,
  [CLIENTREF] varchar(20) NULL
)

INSERT INTO @TmpTableService
(
  [SERVICEID],
  [CUSTOMERID],
  [CUSTAPLID],
  [STATUSID],
  [TODAYDATETIME],
  [DONEDATE],
  [JOBID],
  [CLIENTREF]
)
SELECT
  [SERVICEID],
  [CUSTOMERID],
  [CUSTAPLID],
  [STATUSID],
  [TODAYDATETIME],
  [DONEDATE],
  [JOBID],
  [CLIENTREF]
FROM
  [service]
WHERE
  CLIENTID = @Clientid

SELECT
  s2.SERVICEID,
  dbo.fn_Help1GetValue(c.[TITLE]) AS Title,
  dbo.fn_Help1GetValue(c.[SURNAME]) AS Surname,
  CAST(s1.SERVICEID AS varchar(20)) AS JobNumber,
  dbo.fn_Help1GetValue(cap.[APPLIANCECD]) AS ApplianceCategory,
  dbo.fn_Help1GetValue(cap.[MFR]) AS Brand,
  dbo.fn_Help1GetValue(cap.[MODEL]) AS Model,
  dbo.fn_Help2GetValue(a.[FirstDateoffered]) AS FirstDateOffered,
  dbo.fn_Help2GetValue(a.[DateChosen]) AS CustomerSelectedDate,
  dbo.fn_Help2GetValue(d2.[EventDate]) AS DateOfFirstVisit,
  dbo.fn_Help2GetValue(d4.[EventDate]) AS DateOfSecondVisit,
  dbo.fn_Help2GetValue(d6.[EventDate]) AS DateOfThirdVisit,
  dbo.fn_Help1GetValue(s.[Status]) AS JobStatus,
  dbo.fn_Help2GetValue(CASE WHEN s1.STATUSID IN(8, 30) THEN CAST(COALESCE(s1.[DONEDATE], sn2.[DateTime], s1.[TODAYDATETIME]) AS date) ELSE NULL END) AS CompleteDate,
  dbo.fn_Help2GetValue(CASE WHEN s1.STATUSID = 2 THEN COALESCE(CAST(sn1.[DateTime] AS date), d6.[EventDate], d4.[EventDate], d2.[EventDate], CAST(s1.[TODAYDATETIME] AS date)) ELSE NULL END) AS CancelledDate,
  dbo.fn_Help1GetValue(cap.[SNO]) AS SerialNumber,
  dbo.fn_Help1GetValue(s1.[CLIENTREF]) AS ClientReferenceNumber,
  dbo.fn_Help1GetValue(cap.[POLICYNUMBER]) AS PolicyNumber
FROM 
  @TmpTableService s1
  INNER JOIN Customer c ON s1.CUSTOMERID = c.CUSTOMERID
  INNER JOIN Custapl cap ON s1.CUSTAPLID = cap.CUSTAPLID
  INNER JOIN [status] s ON s1.STATUSID = s.StatusID
  LEFT OUTER JOIN AppointmentTrack a ON s1.SERVICEID = a.ServiceID
  LEFT OUTER JOIN @TmpTableService s2 ON s2.SERVICEID =
  (
    SELECT TOP 1 s3.SERVICEID
	FROM @TmpTableService s3 LEFT OUTER JOIN DiaryEnt d1 ON s3.SERVICEID = d1.TagInteger1
	WHERE s3.JOBID = s1.JOBID AND d1.EventDate IS NOT NULL
	ORDER BY d1.[EventDate]
  )
  LEFT OUTER JOIN DiaryEnt d2 ON s2.SERVICEID = d2.TagInteger1
  LEFT OUTER JOIN @TmpTableService s4 ON s4.SERVICEID =
  (
    SELECT TOP 1 s5.SERVICEID
	FROM @TmpTableService s5 LEFT OUTER JOIN DiaryEnt d3 ON s5.SERVICEID = d3.TagInteger1
	WHERE s5.JOBID = s1.JOBID AND s5.SERVICEID <> s2.SERVICEID AND d3.EventDate IS NOT NULL
	ORDER BY d3.[EventDate]
  )
  LEFT OUTER JOIN DiaryEnt d4 ON s4.SERVICEID = d4.TagInteger1
  LEFT OUTER JOIN @TmpTableService s6 ON s6.SERVICEID = 
  (
    SELECT TOP 1 s7.SERVICEID
	FROM @TmpTableService s7 LEFT OUTER JOIN DiaryEnt d5 ON s7.SERVICEID = d5.TagInteger1
	WHERE s7.JOBID = s1.JOBID AND s7.SERVICEID <> s2.SERVICEID AND s7.SERVICEID <> s4.SERVICEID AND d5.EventDate IS NOT NULL
	ORDER BY d5.[EventDate]
  )
  LEFT OUTER JOIN DiaryEnt d6 ON s6.SERVICEID = d6.TagInteger1
  LEFT OUTER JOIN ServiceNotes sn1 ON s1.SERVICEID = sn1.serviceId AND sn1.[notes] LIKE 'Job Cancelled%' AND sn1.NotesID =
  (
    SELECT TOP 1 tmp1.NotesID
	FROM ServiceNotes tmp1
	WHERE tmp1.serviceId IN(SELECT tmp2.[SERVICEID] FROM  @TmpTableService tmp2 WHERE tmp2.JOBID = s1.JOBID)
	ORDER BY tmp1.[DateTime] desc
  )
  LEFT OUTER JOIN ServiceNotes sn2 ON s1.SERVICEID = sn2.serviceId AND sn2.Complete IS NOT NULL AND sn2.NotesID =
  (
    SELECT TOP 1 tmp3.NotesID
	FROM ServiceNotes tmp3
	WHERE tmp3.serviceId IN(SELECT tmp4.[SERVICEID] FROM  @TmpTableService tmp4 WHERE tmp4.JOBID = s1.JOBID)
	ORDER BY tmp3.[DateTime] desc
  )
WHERE
  s1.STATUSID = @StatusId AND
  s1.SERVICEID = s1.JOBID
ORDER BY
  s1.SERVICEID

OPTION(RECOMPILE)
END
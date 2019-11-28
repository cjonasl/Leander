CREATE PROCEDURE [dbo].[Report_GetClosedJobs_Export]
@ClientId int
AS
BEGIN
DECLARE
@StartDate date,
@EndDate date

SET @EndDate = CAST(getdate() as date)
SET @StartDate = DATEADD(month, -1, @EndDate)

DECLARE @TmpTableService1 TABLE
(
  [SERVICEIDFIRST] int NOT NULL primary key,
  [SERVICEIDLAST] int NOT NULL,
  [CLOSEDDATE] date NULL
)

DECLARE @TmpTableService2 TABLE
(
  [SERVICEID] int NOT NULL primary key,
  [DONEDATE] date NULL,
  [TODAYDATETIME] date NULL
)

DECLARE @TmpTableService3 TABLE
(
  [SERVICEID] int NOT NULL,
  [CUSTOMERID] int NULL,
  [CUSTAPLID] int NULL,
  [STATUSID] smallint NULL,
  [CLOSEDDATE] date NULL,
  [JOBID] int NULL,
  [CLIENTREF] varchar(20) NULL
)

DECLARE @TmpTableDiaryEnt TABLE
(
  [SERVICEID] int NOT NULL,
  [EventDate] date NOT NULL
)

INSERT INTO
  @TmpTableService1 ([SERVICEIDFIRST], [SERVICEIDLAST])
SELECT
  MIN([SERVICEID]), MAX([SERVICEID])
FROM
  [service]
WHERE
  CLIENTID = @ClientId AND
  JOBID IS NOT NULL AND
  STATUSID IS NOT NULL AND
  (DONEDATE IS NULL OR (ISNULL(DONEDATE, '1900-01-01') > @StartDate) OR SERVICEID = JOBID) AND
  (DONEDATE IS NULL OR (ISNULL(DONEDATE, '2100-01-01') < @EndDate) OR SERVICEID = JOBID)
GROUP BY
  JOBID

DELETE FROM @TmpTableService1
FROM @TmpTableService1 t INNER JOIN [service] s ON t.[SERVICEIDLAST] = s.SERVICEID
WHERE s.STATUSID NOT IN (8, 19, 30, 46, 53)

INSERT INTO
  @TmpTableService2([SERVICEID], [DONEDATE], [TODAYDATETIME])
SELECT
  s.[SERVICEID], s.[DONEDATE], CAST(s.[TODAYDATETIME] AS date)
FROM
  @TmpTableService1 tmp
  INNER JOIN [service] s ON tmp.SERVICEIDLAST = s.SERVICEID

UPDATE
  @TmpTableService1
SET
  [CLOSEDDATE] = s.DONEDATE
FROM
  @TmpTableService1 tmp
  INNER JOIN @TmpTableService2 s ON tmp.SERVICEIDLAST = s.SERVICEID

DELETE FROM
  @TmpTableService1
WHERE
  [CLOSEDDATE] IS NOT NULL AND
  ([CLOSEDDATE] < @StartDate OR [CLOSEDDATE] > @EndDate)


INSERT INTO
  @TmpTableDiaryEnt([SERVICEID], [EventDate])
SELECT
  TagInteger1, EventDate
FROM
  DiaryEnt
WHERE
  EventDate IS NOT NULL AND
  EventDate > @StartDate AND
  EventDate < @EndDate AND
  TagInteger1 IN(SELECT SERVICEIDLAST FROM @TmpTableService1)

UPDATE
  @TmpTableService1
SET
  [CLOSEDDATE] = d.EventDate
FROM
  @TmpTableService1 tmp
  LEFT OUTER JOIN @TmpTableDiaryEnt d ON tmp.[SERVICEIDLAST] = d.SERVICEID
 WHERE
   [CLOSEDDATE] IS NULL

DELETE FROM
  @TmpTableService1
WHERE
  [CLOSEDDATE] IS NOT NULL AND
  ([CLOSEDDATE] < @StartDate OR [CLOSEDDATE] > @EndDate)


UPDATE
  @TmpTableService1
SET
  [CLOSEDDATE] = s.TODAYDATETIME
FROM
  @TmpTableService1 tmp
  INNER JOIN @TmpTableService2 s ON tmp.SERVICEIDLAST = s.SERVICEID
WHERE
  [CLOSEDDATE] IS NULL

DELETE FROM
  @TmpTableService1
WHERE
  [CLOSEDDATE] IS NOT NULL AND
  ([CLOSEDDATE] < @StartDate OR [CLOSEDDATE] > @EndDate)


INSERT INTO @TmpTableService3
(
  [SERVICEID],
  [CUSTOMERID],
  [CUSTAPLID],
  [STATUSID],
  [CLOSEDDATE],
  [JOBID],
  [CLIENTREF]
)
SELECT
  s.[SERVICEID],
  s.[CUSTOMERID],
  s.[CUSTAPLID],
  s.[STATUSID],
  tmp.[CLOSEDDATE],
  s.[JOBID],
  s.[CLIENTREF]
FROM
  [service] s
  INNER JOIN @TmpTableService1 tmp ON s.JOBID = tmp.SERVICEIDFIRST


DELETE FROM @TmpTableDiaryEnt
WHERE 1 = 1

INSERT INTO
  @TmpTableDiaryEnt([SERVICEID], [EventDate])
SELECT
  d.TagInteger1, d.EventDate
FROM
  DiaryEnt d
  INNER JOIN @TmpTableService3 tmp ON d.TagInteger1 = tmp.[SERVICEID]
WHERE
  d.EventDate IS NOT NULL

SELECT
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
  dbo.fn_Help2GetValue(s1.CLOSEDDATE) AS ClosedDate,
  dbo.fn_Help1GetValue(cap.[SNO]) AS SerialNumber,
  dbo.fn_Help1GetValue(s1.[CLIENTREF]) AS ClientReferenceNumber,
  dbo.fn_Help1GetValue(cap.[POLICYNUMBER]) AS PolicyNumber

FROM 
  @TmpTableService3 s1
  INNER JOIN Customer c ON s1.CUSTOMERID = c.CUSTOMERID
  INNER JOIN Custapl cap ON s1.CUSTAPLID = cap.CUSTAPLID
  INNER JOIN [status] s ON s1.STATUSID = s.StatusID
  LEFT OUTER JOIN AppointmentTrack a ON s1.SERVICEID = a.ServiceID
  LEFT OUTER JOIN @TmpTableService3 s2 ON s2.SERVICEID =
  (
    SELECT TOP 1 s3.SERVICEID
	FROM @TmpTableService3 s3 LEFT OUTER JOIN @TmpTableDiaryEnt d1 ON s3.SERVICEID = d1.SERVICEID
	WHERE s3.JOBID = s1.JOBID AND d1.EventDate IS NOT NULL
	ORDER BY d1.[EventDate]
  )
  LEFT OUTER JOIN @TmpTableDiaryEnt d2 ON s2.SERVICEID = d2.SERVICEID
  LEFT OUTER JOIN @TmpTableService3 s4 ON s4.SERVICEID =
  (
    SELECT TOP 1 s5.SERVICEID
	FROM @TmpTableService3 s5 LEFT OUTER JOIN @TmpTableDiaryEnt d3 ON s5.SERVICEID = d3.SERVICEID
	WHERE s5.JOBID = s1.JOBID AND s5.SERVICEID <> s2.SERVICEID AND d3.EventDate IS NOT NULL
	ORDER BY d3.[EventDate]
  )
  LEFT OUTER JOIN @TmpTableDiaryEnt d4 ON s4.SERVICEID = d4.SERVICEID
  LEFT OUTER JOIN @TmpTableService3 s6 ON s6.SERVICEID = 
  (
    SELECT TOP 1 s7.SERVICEID
	FROM @TmpTableService3 s7 LEFT OUTER JOIN @TmpTableDiaryEnt d5 ON s7.SERVICEID = d5.SERVICEID
	WHERE s7.JOBID = s1.JOBID AND s7.SERVICEID <> s2.SERVICEID AND s7.SERVICEID <> s4.SERVICEID AND d5.EventDate IS NOT NULL
	ORDER BY d5.[EventDate]
  )
  LEFT OUTER JOIN DiaryEnt d6 ON s6.SERVICEID = d6.TagInteger1
WHERE
  s1.SERVICEID = s1.JOBID
ORDER BY
  s1.SERVICEID desc

OPTION(RECOMPILE)
END

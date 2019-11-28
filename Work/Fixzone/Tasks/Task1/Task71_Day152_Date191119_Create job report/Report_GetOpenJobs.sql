CREATE PROCEDURE [dbo].[Report_GetOpenJobs]
@ClientId int
AS
BEGIN

DECLARE @TmpTableService1 TABLE
(
  [SERVICEIDFIRST] int NOT NULL primary key,
  [SERVICEIDLAST] int NOT NULL
)

DECLARE @TmpTableService2 TABLE
(
  [SERVICEID] int NOT NULL,
  [CUSTOMERID] int NULL,
  [CUSTAPLID] int NULL,
  [STATUSID] smallint NULL,
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
  MIN(s.[SERVICEID]), MAX(s.[SERVICEID])
FROM
  [service] s
  INNER JOIN [status] st ON s.STATUSID = st.StatusID
WHERE
  s.CLIENTID = @ClientId AND
  s.JOBID IS NOT NULL AND
  s.STATUSID IS NOT NULL
GROUP BY
  s.JOBID
HAVING
  MAX(CAST(st.StatusWIP as int)) = 1

INSERT INTO @TmpTableService2
(
  [SERVICEID],
  [CUSTOMERID],
  [CUSTAPLID],
  [STATUSID],
  [JOBID],
  [CLIENTREF]
)
SELECT
  s.[SERVICEID],
  s.[CUSTOMERID],
  s.[CUSTAPLID],
  s.[STATUSID],
  s.[JOBID],
  s.[CLIENTREF]
FROM
  [service] s
  INNER JOIN @TmpTableService1 tmp ON s.JOBID = tmp.SERVICEIDFIRST

INSERT INTO
  @TmpTableDiaryEnt([SERVICEID], [EventDate])
SELECT
  d.TagInteger1, d.EventDate
FROM
  DiaryEnt d
  INNER JOIN @TmpTableService2 tmp ON d.TagInteger1 = tmp.[SERVICEID]
WHERE
  d.EventDate IS NOT NULL

SELECT
  dbo.fn_Help1GetValue(c.[SURNAME]) AS Surname,
  CAST(s1.SERVICEID AS varchar(20)) AS JobNumber,
  dbo.fn_Help1GetValue(cap.[APPLIANCECD]) AS ApplianceCategory,
  dbo.fn_Help1GetValue(cap.[MFR]) AS Brand,
  dbo.fn_Help1GetValue(cap.[MODEL]) AS Model,
  dbo.fn_Help2GetValue(d2.[EventDate]) AS DateOfFirstVisit,
  dbo.fn_Help2GetValue(d4.[EventDate]) AS DateOfSecondVisit,
  dbo.fn_Help1GetValue(s.[Status]) AS JobStatus
FROM 
  @TmpTableService2 s1
  INNER JOIN Customer c ON s1.CUSTOMERID = c.CUSTOMERID
  INNER JOIN Custapl cap ON s1.CUSTAPLID = cap.CUSTAPLID
  INNER JOIN [status] s ON s1.STATUSID = s.StatusID
  LEFT OUTER JOIN AppointmentTrack a ON s1.SERVICEID = a.ServiceID
  LEFT OUTER JOIN @TmpTableService2 s2 ON s2.SERVICEID =
  (
    SELECT TOP 1 s3.SERVICEID
	FROM @TmpTableService2 s3 LEFT OUTER JOIN @TmpTableDiaryEnt d1 ON s3.SERVICEID = d1.SERVICEID
	WHERE s3.JOBID = s1.JOBID AND d1.EventDate IS NOT NULL
	ORDER BY d1.[EventDate]
  )
  LEFT OUTER JOIN @TmpTableDiaryEnt d2 ON s2.SERVICEID = d2.SERVICEID
  LEFT OUTER JOIN @TmpTableService2 s4 ON s4.SERVICEID =
  (
    SELECT TOP 1 s5.SERVICEID
	FROM @TmpTableService2 s5 LEFT OUTER JOIN @TmpTableDiaryEnt d3 ON s5.SERVICEID = d3.SERVICEID
	WHERE s5.JOBID = s1.JOBID AND s5.SERVICEID <> s2.SERVICEID AND d3.EventDate IS NOT NULL
	ORDER BY d3.[EventDate]
  )
  LEFT OUTER JOIN @TmpTableDiaryEnt d4 ON s4.SERVICEID = d4.SERVICEID
  LEFT OUTER JOIN @TmpTableService2 s6 ON s6.SERVICEID = 
  (
    SELECT TOP 1 s7.SERVICEID
	FROM @TmpTableService2 s7 LEFT OUTER JOIN @TmpTableDiaryEnt d5 ON s7.SERVICEID = d5.SERVICEID
	WHERE s7.JOBID = s1.JOBID AND s7.SERVICEID <> s2.SERVICEID AND s7.SERVICEID <> s4.SERVICEID AND d5.EventDate IS NOT NULL
	ORDER BY d5.[EventDate]
  )
  LEFT OUTER JOIN DiaryEnt d6 ON s6.SERVICEID = d6.TagInteger1
WHERE
  s1.SERVICEID = s1.JOBID
ORDER BY
  s1.SERVICEID

OPTION(RECOMPILE)
END

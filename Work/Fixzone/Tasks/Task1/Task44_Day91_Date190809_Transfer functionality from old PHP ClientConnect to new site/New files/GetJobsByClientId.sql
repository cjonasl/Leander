SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetJobsByClientId]
@ClientId int,
@ServiceId varchar(20),
@Surname varchar(100),
@Postcode varchar(100),
@TelNo varchar(100),
@PolicyNumber varchar(100),
@ClientRef varchar(100),
@Address varchar(100),
@UseAndInWhereCondition bit,
@ReturnLines int,
@PageNumber int
AS
DECLARE
@SearchCondition varchar(110),
@postcodeWithoutSpaces varchar(10),
@FirstInsertHasBeenDone bit,
@StartRowNum int,
@RecordCount int

SET @FirstInsertHasBeenDone = 0
SET @StartRowNum = (@PageNumber - 1) * @ReturnLines + 1
SET @postcodeWithoutSpaces = '%' + REPLACE(LTRIM(RTRIM(@Postcode)), ' ', '') + '%'


CREATE TABLE #TmpTableService
(
  SERVICEID int NOT NULL
)

CREATE TABLE #TmpTableSearchJobs
(
  Id int NOT NULL,
  RepairNo varchar(20) NOT NULL,
  Logged datetime NULL,
  SURNAME varchar(50) NOT NULL,
  CustomerName varchar(max) NOT NULL,
  [Address] varchar(150) NOT NULL,
  [Description] varchar(40) NOT NULL,
  [Status] varchar(30) NOT NULL,
  LeadTime datetime NULL,
  Postcode varchar(8) NOT NULL,
  TEL1 varchar(20) NOT NULL,
  TEL2 varchar(20) NOT NULL,
  StoreId int NULL,
  StoreName varchar(50) NOT NULL,
  RetailClientName varchar(50) NOT NULL,
  RecordCount int NULL,
  POLICYNUMBER varchar(25) NOT NULL,
  CLIENTREF varchar(20) NOT NULL,
  ADDR1 varchar(60) NOT NULL
)

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@ServiceId, ''))) + '%'

IF (@UseAndInWhereCondition = 1 AND @SearchCondition <> '%%')
BEGIN
  INSERT INTO
    #TmpTableService(SERVICEID)
  SELECT
    SERVICEID
  FROM
    [service]
  WHERE
    CLIENTID = @ClientId AND
	(JOBID IS NULL OR (SERVICEID = JOBID)) AND
	CAST(SERVICEID AS varchar(25)) LIKE @SearchCondition
END
ELSE
BEGIN
  INSERT INTO
    #TmpTableService(SERVICEID)
  SELECT
    SERVICEID
  FROM
    [service]
  WHERE
    CLIENTID = @ClientId AND
	(JOBID IS NULL OR (SERVICEID = JOBID))
END

IF (@SearchCondition <> '%%')
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    LEFT JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    CAST(s.SERVICEID AS varchar(25)) LIKE @SearchCondition OR CAST(JOBID AS varchar(25)) LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Surname , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    cu.SURNAME LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE SURNAME NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Postcode , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    ISNULL(cu.POSTCODE, '') LIKE @SearchCondition OR
    REPLACE(ISNULL(cu.POSTCODE, ''), ' ', '') LIKE @postcodeWithoutSpaces

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE Postcode NOT LIKE @SearchCondition AND REPLACE(Postcode, ' ', '') NOT LIKE @postcodeWithoutSpaces
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@TelNo , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    cu.TEL1 LIKE @SearchCondition OR
    cu.TEL2 LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE TEL1 NOT LIKE @SearchCondition AND TEL2 NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@PolicyNumber , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    INNER JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
   LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    ca.POLICYNUMBER LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE POLICYNUMBER NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@ClientRef , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    s.CLIENTREF LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE CLIENTREF NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Address , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchJobs(Id, RepairNo, Logged, CustomerName, SURNAME, [Address], [Description], [Status], LeadTime, Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTREF, ADDR1)
  SELECT
    s.SERVICEID,
    'FZ' + CAST(s.SERVICEID AS char(10)),
    s.CALLDATETIME,
    ISNULL(cu.SURNAME, ''),
    LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + ISNULL(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
    ISNULL(m.[DESCRIPTION], ''),
    ISNULL(st.[Status], ''),
    e.ExpectedTurnaroundDays,
    ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
    cu.CLIENTID,
    ISNULL(cl.ClientName, ''),
    ISNULL(rc.RetailClientName, ''),
    ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(s.CLIENTREF, ''),
    ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableService tmp
    INNER JOIN [service] s ON tmp.SERVICEID = s.SERVICEID
    INNER JOIN Customer cu ON s.CUSTOMERID = cu.CUSTOMERID
    LEFT JOIN Client cl ON s.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
    LEFT JOIN Custapl ca ON cu.CUSTOMERID = ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID)
    LEFT JOIN Model m ON ca.MODEL = m.MODEL AND ca.MFR = m.MFR AND ca.APPLIANCECD = m.APPLIANCECD
    LEFT JOIN [status] st ON s.[STATUSID] = st.Statusid
    LEFT JOIN RepairProfile r ON s.RepairId = r.RepairID
    LEFT JOIN Enginrs e ON r.RepairBookRepairEngineerID = e.ENGINEERID
  WHERE
    cu.ADDR1 LIKE @SearchCondition
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchJobs
  WHERE ADDR1 NOT LIKE @SearchCondition
END

SELECT @RecordCount = COUNT(*)
FROM (SELECT DISTINCT Id, RepairNo, Logged, CustomerName, PostCode, [Description], [Status], LeadTime, [Address], StoreId, StoreName FROM #TmpTableSearchJobs) R

  DECLARE @Results TABLE(
     Id INT,
     RepairNo VARCHAR(20),
     Logged DATETIME,
     CustomerName VARCHAR(MAX),
     PostCode VARCHAR(8),	
     [Description] VARCHAR(40),
     [Status] VARCHAR(30),
     LeadTime DATETIME,
     [Address] varchar(150),
     StoreId INT,
     StoreName VARCHAR(50));

WITH JobsFiltered AS
(
  SELECT
    ROW_NUMBER() OVER (ORDER BY R.Logged DESC) AS RowNumber,
    R.Id,
    R.RepairNo,
    R.Logged,
    R.CustomerName,
    R.Postcode,
    R.[Description],
    R.[Status],
    R.LeadTime,
    R.[Address],
    R.StoreId,
    R.StoreName
FROM
(
  SELECT DISTINCT
    Id,
    RepairNo,
    Logged,
    CustomerName,
    Postcode,
    [Description],
    [Status],
    LeadTime,
    [Address],
    StoreId,
    StoreName
  FROM
    #TmpTableSearchJobs
) AS R)
INSERT intO @Results
SELECT Id, RepairNo, Logged, CustomerName, Postcode, [Description], [Status], LeadTime, [Address], StoreId, StoreName
FROM JobsFiltered
WHERE RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1

SELECT
  Id,
  RepairNo,
  Logged,
  CustomerName,
  Postcode,
  [Description],
  [Status],
  CASE WHEN LeadTime IS NOT NULL THEN CONVERT(varchar(12), LeadTime, 103) ELSE '' END AS 'LeadTime',
  [Address],
  CASE WHEN StoreId IS NOT NULL THEN StoreId ELSE 0 END AS 'StoreId',
  StoreName,
  @RecordCount as 'ElemCount',
  ISNULL(@startRowNum, 0) AS 'StartElem', 
  CASE WHEN (@startRowNum + @ReturnLines - 1) > @RecordCount THEN @RecordCount ELSE (@startRowNum + @ReturnLines - 1) END AS 'LastElem'
FROM
  @Results
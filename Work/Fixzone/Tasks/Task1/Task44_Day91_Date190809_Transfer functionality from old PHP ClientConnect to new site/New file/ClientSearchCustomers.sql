SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ClientSearchCustomers]
@ClientId int,
@Surname varchar(100) = '',
@Postcode varchar(100) = '',
@TelNo varchar(100) = '',
@PolicyNumber varchar(100) = '',
@ClientCustRef varchar(100) = '',
@Address varchar(100) = '',
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


CREATE TABLE #TmpTableCustomer
(
  CUSTOMERID int NOT NULL,
  TITLE varchar(5) NULL,
  FIRSTNAME varchar(20) NULL,
  SURNAME varchar(50) NULL,
  ADDR1 varchar(60) NULL,
  ADDR2 varchar(60) NULL,
  POSTCODE varchar(8) NULL,
  TEL1 varchar(20) NULL,
  TEL2 varchar(20) NULL,
  CLIENTID int NULL,
  CLIENTCUSTREF varchar(20) NULL,
  CreatedDateTime datetime NULL,
  RetailClientID int NULL
)

CREATE TABLE #TmpTableSearchCustomers
(
  CUSTOMERID int NOT NULL,
  Logged datetime NULL,
  CustomerName varchar(max) NOT NULL,
  [Address] varchar(150) NOT NULL,
  Postcode varchar(8) NOT NULL,
  TEL1 varchar(20) NOT NULL,
  TEL2 varchar(20) NOT NULL,
  StoreId int NOT NULL,
  StoreName varchar(50) NOT NULL,
  RetailClientName varchar(50) NOT NULL,
  RecordCount int NULL,
  POLICYNUMBER varchar(25) NOT NULL,
  CLIENTCUSTREF varchar(20) NOT NULL,
  ADDR1 varchar(60) NOT NULL
)

INSERT INTO
  #TmpTableCustomer(CUSTOMERID, TITLE, FIRSTNAME, SURNAME, ADDR1, ADDR2, POSTCODE, TEL1, TEL2, CLIENTID, CLIENTCUSTREF, CreatedDateTime, RetailClientID)
SELECT DISTINCT
  cu.CUSTOMERID, cu.TITLE, cu.FIRSTNAME, cu.SURNAME, cu.ADDR1, cu.ADDR2, cu.POSTCODE, cu.TEL1, cu.TEL2, cu.CLIENTID, cu.CLIENTCUSTREF, cu.CreatedDateTime, cu.RetailClientID
FROM
  Customer cu
  LEFT OUTER JOIN [service] s ON cu.CUSTOMERID = s.CUSTOMERID
WHERE
  cu.CLIENTID = @ClientId OR
  s.CLIENTID = @ClientId 

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Surname, ''))) + '%'
IF (@SearchCondition <> '%%')
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    SURNAME LIKE @SearchCondition

	SET @FirstInsertHasBeenDone = 1
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Postcode , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu					
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    ISNULL(cu.POSTCODE, '') LIKE @SearchCondition OR
	REPLACE(ISNULL(cu.POSTCODE, ''), ' ', '') LIKE @postcodeWithoutSpaces

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchCustomers
  WHERE Postcode NOT LIKE @SearchCondition AND REPLACE(Postcode, ' ', '') NOT LIKE @postcodeWithoutSpaces
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@TelNo , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu					
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    cu.TEL1 LIKE @SearchCondition OR
	cu.TEL2 LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchCustomers
  WHERE TEL1 NOT LIKE @SearchCondition AND TEL2 NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@PolicyNumber , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu					
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    ca.POLICYNUMBER LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchCustomers
  WHERE POLICYNUMBER NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@ClientCustRef , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu					
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    cu.CLIENTCUSTREF LIKE @SearchCondition

  SET @FirstInsertHasBeenDone = 1
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchCustomers
  WHERE CLIENTCUSTREF NOT LIKE @SearchCondition
END

SET @SearchCondition = '%' + LTRIM(RTRIM(ISNULL(@Address , ''))) + '%'
IF (@SearchCondition  <> '%%' AND (@UseAndInWhereCondition = 0 OR @FirstInsertHasBeenDone = 0))
BEGIN
  INSERT INTO
    #TmpTableSearchCustomers(CUSTOMERID, Logged, CustomerName, [Address], Postcode, TEL1, TEL2, StoreId, StoreName, RetailClientName, POLICYNUMBER, CLIENTCUSTREF, ADDR1)
  SELECT
    cu.CUSTOMERID,
	cu.CreatedDateTime,
	LTRIM(RTRIM(ISNULL(cu.TITLE, '') + ' ' + ISNULL(cu.FIRSTNAME, '') + ' ' + COALESCE(cu.SURNAME, ''))),
    LTRIM(RTRIM(ISNULL(cu.ADDR1, '') + ' ,' + ISNULL(cu.ADDR2, ''))),
	ISNULL(cu.POSTCODE, ''),
    ISNULL(cu.TEL1, ''),
    ISNULL(cu.TEL2, ''),
	cu.CLIENTID,
	cl.ClientName,
	ISNULL(rc.RetailClientName, ''),
	ISNULL(ca.POLICYNUMBER, ''),
	ISNULL(cu.CLIENTCUSTREF, ''),
	ISNULL(cu.ADDR1, '')
  FROM
    #TmpTableCustomer cu					
    INNER JOIN Client cl ON cu.CLIENTID = cl.ClientID
    LEFT JOIN RetailClient rc ON cu.RetailClientID = rc.RetailID
	LEFT JOIN Custapl ca ON ISNULL(ca.OwnerCustomerID, ca.CUSTOMERID) = cu.CUSTOMERID
  WHERE
    cu.ADDR1 LIKE @SearchCondition
END
ELSE IF (@SearchCondition  <> '%%')
BEGIN
  DELETE FROM #TmpTableSearchCustomers
  WHERE ADDR1 NOT LIKE @SearchCondition
END

SELECT @RecordCount = COUNT(*)
FROM (SELECT DISTINCT CustomerId, Logged, CustomerName, PostCode, [Address], RetailClientName, StoreId, StoreName FROM #TmpTableSearchCustomers) R

DECLARE @Results TABLE(
		CustomerId int,
		Logged datetime,
		CustomerName varchar(MAX),
		PostCode varchar(8),
		[Address] varchar(150),
		RetailClientid varchar(20),
		StoreId int,
		StoreName varchar(50)
		);

WITH CustomersFiltered AS
(
  SELECT
    ROW_NUMBER() OVER (ORDER BY R.Logged DESC) AS RowNumber,
    R.CustomerId,
	R.Logged,
	R.CustomerName,
	R.Postcode,
    R.[Address],
	R.RetailClientName,
	R.StoreId,
	R.StoreName
FROM
(
  SELECT DISTINCT
    CustomerId,
	Logged,
	CustomerName,
	PostCode,
	[Address],
	RetailClientName,
	StoreId,
	StoreName
  FROM
	 #TmpTableSearchCustomers
) AS R)
INSERT intO @Results
SELECT CustomerId, Logged, CustomerName, Postcode, [Address], RetailClientName, StoreId, StoreName
FROM CustomersFiltered
WHERE RowNumber BETWEEN @startRowNum AND @startRowNum + @ReturnLines - 1

SELECT
  CUSTOMERID,
  Logged,
  CustomerName,
  PostCode,
  [Address],
  StoreId,
  StoreName,
  RetailClientid,
  @RecordCount as 'ElemCount',
  ISNULL(@startRowNum, 0) AS 'StartElem', 
  CASE WHEN (@startRowNum + @ReturnLines - 1) > @RecordCount THEN @RecordCount ELSE (@startRowNum + @ReturnLines - 1) END AS 'LastElem'
FROM
  @Results R

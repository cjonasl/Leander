CREATE PROCEDURE [dbo].[ClientsList]
@UseAndInWhereCondition bit,
@ClientID int,
@Name varchar(50),
@Location varchar(50),
@Postcode varchar(50),
@Contact varchar(50),
@ClientPriorityBooking int,
@ReturnLines int,
@PageNumber  int,
@startRowNum int OUTPUT,
@endRowNum int OUTPUT,
@countItems int OUTPUT
AS
SET NOCOUNT ON
DECLARE
@SearchClientID varchar(100),
@SearchName varchar(100),
@SearchLocation varchar(100),
@SearchPostcode varchar(100),
@SearchContact varchar(100),
@SearchClientPriorityBooking varchar(100)

SET @SearchClientID = CASE WHEN @ClientId = 0 THEN '' ELSE CAST(@ClientID AS varchar(100)) END
SET @SearchName = CASE WHEN @Name = '' THEN '' ELSE '%' + @Name + '%' END
SET @SearchLocation = CASE WHEN @Location = '' THEN '' ELSE '%' + @Location + '%' END
SET @SearchPostcode = CASE WHEN @Postcode = '' THEN '' ELSE  '%' + REPLACE(LTRIM(RTRIM(@Postcode)), ' ', '') + '%' END
SET @SearchContact  = CASE WHEN @Contact = '' THEN '' ELSE '%' + @Contact + '%' END
SET @SearchClientPriorityBooking = CASE WHEN @ClientPriorityBooking = 2 THEN '' ELSE CAST(@ClientPriorityBooking AS varchar(100)) END

set @startRowNum = (@PageNumber - 1) * @ReturnLines + 1

IF (@UseAndInWhereCondition = 1)
BEGIN
SELECT * 
FROM (
  SELECT ROW_NUMBER() OVER(ORDER BY c.ClientID) AS 'RowNum',
  c.ClientID AS 'ClientID',
  c.ClientName AS 'Name',
  c.ClientShortname AS 'Location',
  c.ClientPostcode AS 'Postcode',
  c.ClientContact AS 'Contact', 
  CASE WHEN c.ClientPriorityBooking = 0 THEN 'Callcenter' ELSE 'Store' END AS 'Type'
FROM [Client] c			
WHERE
  dbo.CheckCondition(1, @SearchClientID, CAST(c.ClientID AS varchar(100))) = 1 AND
  dbo.CheckCondition(1, @SearchName, c.ClientName) = 1 AND
  dbo.CheckCondition(1, @SearchLocation, c.ClientShortname) = 1 AND
  dbo.CheckCondition(1, @SearchPostcode, REPLACE(LTRIM(RTRIM(c.ClientPostcode)), ' ', '')) = 1 AND
  dbo.CheckCondition(1, @SearchContact, c.ClientContact) = 1 AND
  dbo.CheckCondition(1, @SearchClientPriorityBooking, CAST(c.ClientPriorityBooking AS varchar(100))) = 1
) AS StoreList
WHERE StoreList.RowNum BETWEEN @startRowNum AND (@startRowNum + @ReturnLines - 1)
ORDER BY StoreList.ClientID

--List information
SELECT
  @countItems = ISNULL(COUNT(*), 0), 
  @startRowNum = ISNULL(@startRowNum,0),
  @endRowNum = 
	CASE
	  WHEN (@startRowNum + @ReturnLines - 1) > COUNT(*) THEN COUNT(*)
	  ELSE (@startRowNum + @ReturnLines - 1)
	END
FROM [Client] c			
WHERE
  dbo.CheckCondition(1, @SearchClientID, CAST(c.ClientID AS varchar(100))) = 1 AND
  dbo.CheckCondition(1, @SearchName, c.ClientName) = 1 AND
  dbo.CheckCondition(1, @SearchLocation, c.ClientShortname) = 1 AND
  dbo.CheckCondition(1, @SearchPostcode, REPLACE(LTRIM(RTRIM(c.ClientPostcode)), ' ', '')) = 1 AND
  dbo.CheckCondition(1, @SearchContact, c.ClientContact) = 1 AND
  dbo.CheckCondition(1, @SearchClientPriorityBooking, CAST(c.ClientPriorityBooking AS varchar(100))) = 1
END
ELSE
BEGIN
SELECT * 
FROM (
  SELECT ROW_NUMBER() OVER(ORDER BY c.ClientID) AS 'RowNum',
  c.ClientID AS 'ClientID',
  c.ClientName AS 'Name',
  c.ClientShortname AS 'Location',
  c.ClientPostcode AS 'Postcode',
  c.ClientContact AS 'Contact', 
  CASE WHEN c.ClientPriorityBooking = 0 THEN 'Callcenter' ELSE 'Store' END AS 'Type'
FROM [Client] c			
WHERE
  dbo.CheckCondition(0, @SearchClientID, CAST(c.ClientID AS varchar(100))) = 1 OR
  dbo.CheckCondition(0, @SearchName, c.ClientName) = 1 OR
  dbo.CheckCondition(0, @SearchLocation, c.ClientShortname) = 1 OR
  dbo.CheckCondition(0, @SearchPostcode, REPLACE(LTRIM(RTRIM(c.ClientPostcode)), ' ', '')) = 1 OR
  dbo.CheckCondition(0, @SearchContact, c.ClientContact) = 1 OR
  dbo.CheckCondition(0, @SearchClientPriorityBooking, CAST(c.ClientPriorityBooking AS varchar(100))) = 1
) AS StoreList
WHERE StoreList.RowNum BETWEEN @startRowNum AND (@startRowNum + @ReturnLines - 1)
ORDER BY StoreList.ClientID

--List information
SELECT
  @countItems = ISNULL(COUNT(*), 0), 
  @startRowNum = ISNULL(@startRowNum,0),
  @endRowNum = 
	CASE
	  WHEN (@startRowNum + @ReturnLines - 1) > COUNT(*) THEN COUNT(*)
	  ELSE (@startRowNum + @ReturnLines - 1)
	END
FROM [Client] c			
WHERE
  dbo.CheckCondition(0, @SearchClientID, CAST(c.ClientID AS varchar(100))) = 1 OR
  dbo.CheckCondition(0, @SearchName, c.ClientName) = 1 OR
  dbo.CheckCondition(0, @SearchLocation, c.ClientShortname) = 1 OR
  dbo.CheckCondition(0, @SearchPostcode, REPLACE(LTRIM(RTRIM(c.ClientPostcode)), ' ', '')) = 1 OR
  dbo.CheckCondition(0, @SearchContact, c.ClientContact) = 1 OR
  dbo.CheckCondition(0, @SearchClientPriorityBooking, CAST(c.ClientPriorityBooking AS varchar(100))) = 1
END
GO
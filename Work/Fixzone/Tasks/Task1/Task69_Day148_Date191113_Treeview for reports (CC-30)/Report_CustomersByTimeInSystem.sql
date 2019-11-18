ALTER PROCEDURE Report_CustomersByTimeInSystem
@Clientid int
AS
BEGIN
DECLARE
@N int

DECLARE @TmpTableCustomers TABLE
(
  MonthsSinceCustomerWasCreated int NOT NULL,
  [Months since created] varchar(25) NULL,
  [Number of customers] int NOT NULL
)

INSERT INTO @TmpTableCustomers(MonthsSinceCustomerWasCreated, [Number of customers])
SELECT dateDiff(month ,[CreatedDateTime], getdate()), COUNT(*)
FROM  [dbo].[Customer]
WHERE ClientId = @Clientid AND [CreatedDateTime] IS NOT NULL
GROUP BY dateDiff(month ,[CreatedDateTime], getdate())

UPDATE @TmpTableCustomers
SET [Months since created] = CAST(MonthsSinceCustomerWasCreated AS varchar(25))

SELECT @N = COUNT(*)
FROM [dbo].[Customer]
WHERE ClientId = @Clientid AND [CreatedDateTime] IS NULL

IF (@N > 0)
BEGIN
  INSERT INTO @TmpTableCustomers(MonthsSinceCustomerWasCreated, [Months since created], [Number of customers])
  VALUES(99999999, 'Unknown', @N)
END

SELECT @N = ISNULL(SUM([Number of customers]), 0)
FROM @TmpTableCustomers

INSERT INTO @TmpTableCustomers(MonthsSinceCustomerWasCreated, [Months since created], [Number of customers])
VALUES(999999999, 'Total number of customers', @N)

SELECT [MonthsSinceCustomerWasCreated], [Months since created], [Number of customers]
FROM @TmpTableCustomers
ORDER BY MonthsSinceCustomerWasCreated

OPTION(RECOMPILE)

END
CREATE VIEW NewCustomerAccounts
AS
SELECT TOP 1000
  CAST(YEAR(CreatedDate) AS varchar(4)) + CASE WHEN MONTH(CreatedDate) < 10 THEN '-0' ELSE '-' END + CAST(MONTH(CreatedDate) AS varchar(2)) AS 'Year-Month', 
  COUNT(*) AS 'Count'
FROM
  CustomerAccount
WHERE
  YEAR(CreatedDate) >= 2019
GROUP BY CAST(YEAR(CreatedDate) AS varchar(4)) + CASE WHEN MONTH(CreatedDate) < 10 THEN '-0' ELSE '-' END + CAST(MONTH(CreatedDate) AS varchar(2)) 
ORDER BY CAST(YEAR(CreatedDate) AS varchar(4)) + CASE WHEN MONTH(CreatedDate) < 10 THEN '-0' ELSE '-' END + CAST(MONTH(CreatedDate) AS varchar(2))  
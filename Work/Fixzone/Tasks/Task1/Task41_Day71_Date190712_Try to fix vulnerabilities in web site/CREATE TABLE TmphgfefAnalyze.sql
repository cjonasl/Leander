CREATE TABLE TmphgfefAnalyze
(
  ID int identity(1,1) NOT NULL,
  Email varchar(128) NULL,
  LogInDateTime datetime NULL,
  NumberOfRecords int NULL,
  CustomerId int NULL,
  Psw varchar(100) NULL,
  ClientName varchar(50) NULL
)

SELECT * FROM TmphgfefAnalyze
ORDER BY ID

SELECT * FROM CustomerAccount 
WHERE Email IN(SELECT DISTINCT Email FROM TmphgfefAnalyze)

SELECT * FROM Customer
WHERE Email IN(SELECT DISTINCT Email FROM TmphgfefAnalyze)
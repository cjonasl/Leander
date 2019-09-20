CREATE TABLE AnalyzeLogIn2
(
  ID int identity(1, 1) NOT NULL,
  DeleteOrUpdate varchar(20) NOT NULL,
  _id int NOT NULL,
  Userid varchar(25) NULL,
  LogInDate datetime NOT NULL,
  [Enabled] bit NULL,
  NumberOfLogInFailures tinyint NULL
)
CREATE TABLE AnalyzeLogIn
(
  ID int identity(1, 1) NOT NULL,
  UserId varchar(25) NULL,
  LogInDate datetime NOT NULL,
  UserIdExists bit NOT NULL,
  LogInSuccess bit NOT NULL,
  NumberOfLogInFailures int NULL
)

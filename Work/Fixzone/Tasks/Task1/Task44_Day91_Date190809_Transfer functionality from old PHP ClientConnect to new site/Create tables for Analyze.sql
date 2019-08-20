CREATE TABLE AnalyzeLogIn
(
  ID int identity(1,1) NOT NULL,
  DateAndTime datetime NOT NULL,
  UserId varchar(1000) NULL,
  N int NULL
)

CREATE TABLE AnalyzeProductSearch
(
  ID int identity(1,1) NOT NULL,
  DateAndTime datetime NOT NULL,
  Criteria varchar(1000) NULL, 
  ReturnLines int NULL,
  PageNumber  int NULL,
  Clientid int NULL
)

CREATE TABLE AnalyzeJobSearch
(
  ID int identity(1,1) NOT NULL,
  DateAndTime datetime NOT NULL,
  Criteria varchar(1000) NULL, 
  ReturnLines int NULL,
  PageNumber  int NULL,
  StoreId int NULL
)

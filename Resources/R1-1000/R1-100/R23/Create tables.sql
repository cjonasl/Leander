CREATE TABLE PublishedApplication
(
  Id int primary key identity(1, 1) NOT NULL,
  [Name] varchar(25) NOT NULL,
  PublishedDate datetime NOT NULL
)
GO

CREATE TABLE RequestToApplication
(
  Id int primary key identity(1, 1) NOT NULL,
  ApplicationId int NOT NULL,
  DateTimeUTC datetime NOT NULL,
  RequestHeader nvarchar(max) NOT NULL,
  foreign key(ApplicationId) references PublishedApplication(Id)
)
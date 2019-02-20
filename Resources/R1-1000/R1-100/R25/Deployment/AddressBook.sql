USE [AddressBook]
GO

CREATE TABLE [User]
(
  Id int primary key identity(1, 1),
  [Name] nvarchar(50) NOT NULL,
  [Password] nchar(432) NOT NULL,
  CreatedDate datetime NOT NULL
)
GO

INSERT INTO [User]([Name], [Password], CreatedDate)
VALUES('Anonymous', '+bfAL41f5fLSNUmern9cT3LyJi/3GiRgWR18TZFdZF5796H1t30fbjbMUzoDNOMYxtK8w72UoYHmjNyzEAqDUxhxi+sE6ZGMRg5veC0oeUdQhQd505x83CdJz5+skROHFu/RNKUyHykhq+AScvwRjdmxFQtgKmF7FxCrn/PU1QFEKcfUe22gd88SUSATi/B62Yw+b6xXyqx9Xn3Q6v673K0CRI81VpvVjNEilH+XKd2ap6BY9o1nDsc4uGyNW2jvzB/b3Sse1kflpxXdVTXIhZIRqpaXmv3Q2npx2x1hFfM2lsBEXKwKYrdZO5iUEWPI6WjA0cUu0iy3D72dOALNUw==RJ1u7t+3fSVhbgZ9HYayWX0G1hC5OncCKOVODpWJs64INuNWh61bteasQ5lUbofrelUP+iIBgigVbdiqp+dFeA==', getdate())
GO

CREATE TABLE PersonInfo
(
  UserId int NOT NULL,
  PersonId int NOT NULL,
  FirstName nvarchar(50) NOT NULL,
  LastName nvarchar(50) NOT NULL,
  Gender nvarchar(10) NOT NULL,
  DateOfBirth varchar(10) NOT NULL,
  Phone nvarchar(25) NOT NULL,
  [Address] nvarchar(100) NOT NULL,
  Town nvarchar(100) NOT NULL,
  PostCode nvarchar(10) NOT NULL,
  Country nvarchar(100) NOT NULL,
  IsCloseFriend bit NOT NULL,
  foreign key(UserId) references [User](Id),
 CONSTRAINT [PK_PersonInfo] PRIMARY KEY CLUSTERED 
(
  [UserId] ASC,
  [PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE LOGIN [IIS APPPOOL\AddressBook] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO

CREATE USER [AddressBook] FOR LOGIN [IIS APPPOOL\AddressBook] WITH DEFAULT_SCHEMA=[AddressBook]
GO

sp_addrolemember 'db_accessadmin', 'AddressBook'
GO
sp_addrolemember 'db_datareader', 'AddressBook'
GO
sp_addrolemember 'db_datawriter', 'AddressBook'
GO
sp_addrolemember 'db_securityadmin', 'AddressBook'
GO
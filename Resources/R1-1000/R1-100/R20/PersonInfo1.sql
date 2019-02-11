-- If have problem with access tights maybe can be solved by running one or several of these --
sp_grantdbaccess 'IIS APPPOOL\Nr2Web1', 'Nr2Web1'
GO
sp_addrolemember 'db_accessadmin', 'Nr2Web1'
GO
sp_addrolemember 'db_datareader', 'Nr2Web1'
GO
sp_addrolemember 'db_datawriter', 'Nr2Web1'
GO
sp_addrolemember 'db_securityadmin', 'Nr2Web1'
GO
-----------------------------------------------------------------------------------------------

CREATE TABLE PersonInfo1
(
  Id int primary key identity(1, 1),
  FirstName nvarchar(50) NOT NULL,
  LastName nvarchar(50) NOT NULL,
  Gender nvarchar(10) NOT NULL,
  DateOfBirth nvarchar(15) NOT NULL,
  Phone nvarchar(25),
  [Address] nvarchar(100),
  Town nvarchar(100),
  PostCode nvarchar(10),
  Country nvarchar(100) NOT NULL,
  IsCloseFriend bit NOT NULL
)
GO

INSERT INTO PersonInfo1(FirstName, LastName, Gender, DateOfBirth, Phone, [Address], Town, PostCode, Country, IsCloseFriend)
VALUES('Carl', 'Leander', 'Male', '1969-10-04', '07553501577', '19 Coombe Gardens', 'London', 'sw20 0qu', 'United Kingdom', 1)

INSERT INTO PersonInfo1(FirstName, LastName, Gender, DateOfBirth, Phone, [Address], Town, PostCode, Country, IsCloseFriend)
VALUES('Daniel', 'Leander', 'Male', '1995-11-01', '08-6057681', 'Horisontvägen 45, 1 tr', 'Skarpnäck', '12834', 'Sweden', 1)
GO



CREATE TABLE Department
(
  ID int identity(1,1) NOT NULL primary key,
  [Name] varchar(25) NULL
)

CREATE TABLE Employee
(
  ID int identity(1,1) NOT NULL primary key,
  FirstName varchar(25) NULL,
  LastName varchar(25) NULL,
  DepartmentID int NULL references Department(ID)
)


INSERT INTO Department([Name])
VALUES('HR')

INSERT INTO Department([Name])
VALUES('IT')

INSERT INTO Department([Name])
VALUES('Business')

INSERT INTO Employee(FirstName, LastName, DepartmentID)
VALUES('Jonas', 'Leander', 2)

INSERT INTO Employee(FirstName, LastName, DepartmentID)
VALUES('Anna', 'Johansson', 1)

INSERT INTO Employee(FirstName, LastName, DepartmentID)
VALUES('Malin', 'Bengtsson', 2)

INSERT INTO Employee(FirstName, LastName, DepartmentID)
VALUES('Knut', 'Knaust', 3)

INSERT INTO Employee(FirstName, LastName, DepartmentID)
VALUES('Henrik', 'Olsson', 3)


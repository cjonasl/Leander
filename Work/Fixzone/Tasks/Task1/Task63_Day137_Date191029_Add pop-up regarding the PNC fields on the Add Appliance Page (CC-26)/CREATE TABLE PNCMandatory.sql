CREATE TABLE PNCMandatory
(
  _id int NOT NULL identity(0, 1) primary key,
  MFR varchar(3) NOT NULL UNIQUE,
  [Active] bit NOT NULL
)
GO

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('ELE', 1)

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('REX', 1)

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('EDA', 1)

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('ELA', 1)

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('AEG', 1)

INSERT INTO PNCMandatory(MFR, [Active])
VALUES('ZAN', 1)
GO
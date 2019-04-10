--https://blog.reckonedforce.com/alter-column-varcharmax-varbinarymax/
DROP INDEX idxPwd ON Leander123

ALTER TABLE Leander123
ALTER COLUMN [Password] varchar(20) NULL

ALTER TABLE Leander123
ADD tmpPassword varchar(20) NULL
GO

UPDATE Leander123
SET tmpPassword = [Password]

UPDATE Leander123
SET tmpPassword = NULL
WHERE LTRIM(RTRIM(tmpPassword)) = ''

UPDATE Leander123
SET [Password] = NULL

ALTER TABLE Leander123
ALTER COLUMN [Password] decimal(1,0)

ALTER TABLE Leander123
ALTER COLUMN [Password] varbinary(20) NULL

UPDATE Leander123
SET [Password] = HASHBYTES('SHA1', tmpPassword)
WHERE tmpPassword IS NOT NULL

ALTER TABLE Leander123
DROP COLUMN tmpPassword

SELECT * FROM Leander123
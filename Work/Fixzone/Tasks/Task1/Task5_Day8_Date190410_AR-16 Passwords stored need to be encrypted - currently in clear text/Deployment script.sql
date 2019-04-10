--https://blog.reckonedforce.com/alter-column-varcharmax-varbinarymax/
DROP INDEX idxPwd ON UserWeb

ALTER TABLE UserWeb
ALTER COLUMN [Password] varchar(20) NULL

ALTER TABLE UserWeb
ADD tmpPassword varchar(20) NULL
GO

UPDATE UserWeb
SET tmpPassword = [Password]

UPDATE UserWeb
SET tmpPassword = NULL
WHERE LTRIM(RTRIM(tmpPassword)) = ''

UPDATE UserWeb
SET [Password] = NULL

ALTER TABLE UserWeb
ALTER COLUMN [Password] decimal(1,0)

ALTER TABLE UserWeb
ALTER COLUMN [Password] varbinary(20) NULL

UPDATE UserWeb
SET [Password] = HASHBYTES('SHA1', tmpPassword)
WHERE tmpPassword IS NOT NULL

ALTER TABLE UserWeb
DROP COLUMN tmpPassword
ALTER PROCEDURE [dbo].[ChangePassword]
@pass VARCHAR(max), 
@userId VARCHAR(max)
/*
  Modify 11/4/2019, Jonas Leander
  1. Handle hash of password
  2. Add update of column PasswordValidUntilDate
*/
AS
DECLARE
@passwordHashed varbinary(20)
	
SET @passwordHashed = HASHBYTES('SHA1', @pass)

UPDATE
  [UserWeb]
SET
  [Password] = @passwordHashed,
  [PasswordValidUntilDate] = DATEADD(DAY, 60, GETDATE())
WHERE
  [Userid] = @userId
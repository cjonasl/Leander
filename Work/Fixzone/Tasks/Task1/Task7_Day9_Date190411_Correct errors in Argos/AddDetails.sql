ALTER PROCEDURE [dbo].[AddDetails]
@pass VARCHAR(max), 
@question VARCHAR(max),
@answer VARCHAR(max),
@userId varchar(max)
/*
   Modify 11/4/2019, Jonas Leander
   1. Change from 90 to 60 days in DATEADD
   2. Handle hash of password
*/
AS
DECLARE
@passwordHashed varbinary(20)
	
SET @passwordHashed = HASHBYTES('SHA1', @pass)

UPDATE
  [UserWeb] 
SET
  [Password]= @passwordHashed,
  [ReminderQuestion]= @question,
  [ReminderAnswer]= @answer,
  [PasswordValidUntilDate] = DATEADD(DAY, 60, GETDATE())
WHERE 
  UPPER([Userid]) = UPPER(@userId)
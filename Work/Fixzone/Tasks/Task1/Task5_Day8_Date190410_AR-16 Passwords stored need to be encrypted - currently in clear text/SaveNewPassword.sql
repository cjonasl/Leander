ALTER PROCEDURE [dbo].[SaveNewPassword]
	@pass VARCHAR(max), 
	@userId VARCHAR(max)
/*
  Modify in AR-11, 8/4/2019, Jonas Leander
  Change frm 90 to 60 days in DATEADD for PasswordValidUntilDate

  Modify in AR-16, 9/4/2019, Jonas Leander
  Hash password
*/
AS
	DECLARE 
        @previousPassword varbinary(20),
        @newPassword varbinary(20)

        SET @newPassword = HASHBYTES('SHA1', @pass)
	
	SELECT @previousPassword = [UserWeb].Password 
	FROM [UserWeb] 
	WHERE upper([Userid]) = upper(@userId)

	if @previousPassword = @newPassword
		BEGIN
			SELECT CAST(1 as BIT)
		END
	else 
		BEGIN
			UPDATE [UserWeb]
			SET [Password] = @newPassword,
				[PasswordValidUntilDate] = DATEADD(DAY,60,GETDATE())
			WHERE upper([Userid]) = upper(@userId)

			SELECT CAST(0 as BIT)
		END
GO
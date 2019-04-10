ALTER PROCEDURE [dbo].[SaveNewPassword]
	@pass VARCHAR(max), 
	@userId VARCHAR(max)
/*
  Modify in AR-11, 8/4/2019, Jonas Leander
  Change frm 90 to 60 days in DATEADD for PasswordValidUntilDate
*/
AS
	DECLARE @previousPassword VARCHAR(MAX)
	
	SELECT @previousPassword = [UserWeb].Password 
	FROM [UserWeb] 
	WHERE upper([Userid]) = upper(@userId)

	if @previousPassword = @pass
		BEGIN
			SELECT CAST(1 as BIT)
		END
	else 
		BEGIN
			UPDATE [UserWeb]
			SET [Password] = @pass,
				[PasswordValidUntilDate] = DATEADD(DAY,60,GETDATE())
			WHERE upper([Userid]) = upper(@userId)

			SELECT CAST(0 as BIT)
		END
GO



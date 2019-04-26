ALTER PROCEDURE UpdateUser
/*
  Modify in AR-7, 5/4/2019, Jonas Leander
  Add update of column DisabledDate.

  Modify in AR-16, 9/4/2019, Jonas Leander
  Hash password.
*/
@UserId VARCHAR(max) = NULL,
@Name VARCHAR(70),
@IsEnabled BIT,
@DateOfBirth DATETIME = NULL,
@UserLevel INT,
@Password VARCHAR(max) = '',
@PreviousUserId VARCHAR(max) = NULL
AS
DECLARE
@IsEnabledPrevious bit,
@DisabledDate date,
@Lastacdt date,
@PasswordHashed varbinary(20),
@NewPasswordHashed varbinary(20),
@PasswordValidUntilDate datetime

SET @NewPasswordHashed = HASHBYTES('SHA1', ISNULL(@Password, ''))

IF (@UserId IS NULL)
BEGIN
  INSERT INTO [UserWeb](Fullname, [Enabled], DateOfBirth, Userid, [Password], [Level], PasswordValidUntilDate)
  VALUES(@Name, @IsEnabled, @DateOfBirth, @UserId, @NewPasswordHashed, @UserLevel, DATEADD(DAY, 60, GETDATE()));
END
ELSE
BEGIN
  SELECT
    @PasswordHashed = [Password],
    @Lastacdt = Lastacdt,
    @IsEnabledPrevious = ISNULL([Enabled], 0),
    @DisabledDate = DisabledDate,
	@PasswordValidUntilDate = PasswordValidUntilDate
  FROM
    [UserWeb]
  WHERE
    upper(Userid) = upper(@PreviousUserId)

IF (@IsEnabledPrevious = 0 AND @IsEnabled = 1) --Reactivate
BEGIN
  SET @Lastacdt = getdate()
  SET @DisabledDate = NULL
END
ELSE IF (@IsEnabledPrevious = 1 AND @IsEnabled = 0)
  SET @DisabledDate = getdate()

IF (@Password <> '!$&1(Ah)q6') --Password was updated ('!$&1(Ah)q6' is used to indicate no update)
BEGIN
  SET @PasswordHashed = @NewPasswordHashed
  SET @PasswordValidUntilDate = DATEADD(DAY, 60, GETDATE())
END

UPDATE
  [UserWeb]
SET
  Lastacdt = @Lastacdt,
  Fullname = @Name,
  [Enabled] = @IsEnabled,
  DateOfBirth = @DateOfBirth,
  Userid = @UserId,
  [Level] = @UserLevel,
  [Password] = @PasswordHashed,
  DisabledDate = @DisabledDate,
  PasswordValidUntilDate = @PasswordValidUntilDate
WHERE 
  upper(Userid) = upper(@PreviousUserId)
END
GO
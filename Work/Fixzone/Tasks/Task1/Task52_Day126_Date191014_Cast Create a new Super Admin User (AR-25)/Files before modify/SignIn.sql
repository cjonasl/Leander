ALTER PROCEDURE [dbo].[SignIn]
@UserId varchar(max),
@Password varchar(max)
AS
DECLARE @UsId varchar(25) = NULL
DECLARE @ClientId int = NULL
DECLARE @FullName varchar(max) = NULL
DECLARE @ClientName varchar(50) = NULL
DECLARE @ClientPriorityBooking bit = CAST(0 AS bit)
DECLARE @PasswordExpired bit = CAST(0 AS bit)
DECLARE @Enabled bit = CAST(0 AS bit)
DECLARE @IsPasswordEmpty bit = CAST(0 AS bit)
DECLARE @GroupID int = 0
DECLARE @ReminderQuestion varchar(60) = NULL
DECLARE @ReminderAnswer varchar(20) = NULL
DECLARE @DateOfBirth datetime = NULL
DECLARE @Lastacdt datetime = NULL
DECLARE @NumberOfLogInFailures int = NULL
DECLARE @PasswordHashed varbinary(20) = CASE WHEN @Password IS NOT NULL THEN HASHBYTES('SHA1', @Password) ELSE HASHBYTES('SHA1', '') END
DECLARE @DateToday datetime = getdate()
DECLARE @HashedPasswordInDB varbinary(20) = NULL
DECLARE @NumberOfLogInFailuresUpdateValue int = 0

SELECT
  @UsId = u.Userid,
  @ClientId = u.ClientID,
  @FullName = u.Fullname,
  @ClientName = c.ClientName,
  @ClientPriorityBooking  = c.ClientPriorityBooking,
  @PasswordExpired = CASE WHEN PasswordValidUntilDate > @DateToday THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END,
  @Enabled = u.[Enabled],
  @IsPasswordEmpty = CASE WHEN [Password] IS NULL OR [Password] = HASHBYTES('SHA1', '') THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
  @GroupID = CAST(c.GroupID AS int),
  @ReminderQuestion = u.ReminderQuestion,
  @ReminderAnswer = u.ReminderAnswer,
  @DateOfBirth = u.DateOfBirth,
  @Lastacdt = u.Lastacdt,
  @NumberOfLogInFailures = u.NumberOfLogInFailures,
  @HashedPasswordInDB = u.[Password]
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  UPPER(u.Userid) = UPPER(@UserId)


IF (@Enabled = 1 AND (@IsPasswordEmpty = 1 OR (@PasswordHashed = @HashedPasswordInDB))) --Log in succcess
BEGIN



  UPDATE
    UserWeb
  SET
    Lastacdt = getdate(),
    NumberOfLogInFailures = 0
  WHERE
    UPPER(Userid) = UPPER(@UserId)
    
    
INSERT INTO [UserLogTrack]
           ([Userid]           ,[ClientID]           ,[Fullname]           ,[ClientName]           ,[ClientPriorityBooking]           ,[PasswordExpired]
           ,[Enabled]           ,[IsPasswordEmpty]           ,[GroupID]
           ,[ReminderQuestion]           ,[ReminderAnswer]           ,[DateOfBirth]           ,[Lastacdt]           ,[NumberOfLogInFailures]           ,[PasswordInDB]
           ,[UserPassword]           ,[UserPasswordHash]           ,[LoginSuccess]           ,[Comment])
           
           select @UsId ,  @ClientId ,  @FullName ,  @ClientName ,  @ClientPriorityBooking  ,  @PasswordExpired ,  @Enabled ,
  @IsPasswordEmpty ,  @GroupID ,  @ReminderQuestion ,  @ReminderAnswer,  @DateOfBirth ,  @Lastacdt,
  @NumberOfLogInFailures ,  @HashedPasswordInDB,  @Password,  @PasswordHashed,  cast(1 as bit),  'Login success:' + 'Lastacdt updated with today date'

END

IF (@Enabled = 1 AND @IsPasswordEmpty = 0 AND @PasswordHashed <> @HashedPasswordInDB) --Handle NumberOfLogInFailures
BEGIN
  SET @NumberOfLogInFailures = ISNULL(@NumberOfLogInFailures, 0) + 1
			
  IF (@NumberOfLogInFailures = 3) --Disable user and reset NumberOfLogInFailures to 0
  BEGIN
  
  
    exec dbo.DisableUser @UserId
    SET @NumberOfLogInFailuresUpdateValue = 0
	SET @Enabled = CAST(0 AS bit)
	
	INSERT INTO [UserLogTrack]
           ([Userid]           ,[ClientID]           ,[Fullname]           ,[ClientName]           ,[ClientPriorityBooking]
           ,[PasswordExpired]           ,[Enabled]           ,[IsPasswordEmpty]           ,[GroupID]           ,[ReminderQuestion]
           ,[ReminderAnswer]           ,[DateOfBirth]           ,[Lastacdt]           ,[NumberOfLogInFailures]
           ,[PasswordInDB]           ,[UserPassword]           ,[UserPasswordHash]           ,[LoginSuccess]           ,[Comment])
           
           select @UsId ,  @ClientId ,  @FullName ,  @ClientName ,  @ClientPriorityBooking  ,  @PasswordExpired ,
  @Enabled ,  @IsPasswordEmpty ,  @GroupID ,  @ReminderQuestion ,  @ReminderAnswer,
  @DateOfBirth ,  @Lastacdt,  @NumberOfLogInFailures ,  @HashedPasswordInDB,  @Password,  @PasswordHashed,
  cast(0 as bit),  'Login failure:' + 'NumberOfLogInFailures-3 . So user is deactivated'

	
	
  END
  ELSE
  begin
    SET @NumberOfLogInFailuresUpdateValue = @NumberOfLogInFailures
    
	INSERT INTO [UserLogTrack]
           ([Userid]           ,[ClientID]           ,[Fullname]           ,[ClientName]           ,[ClientPriorityBooking]
           ,[PasswordExpired]           ,[Enabled]           ,[IsPasswordEmpty]           ,[GroupID]           ,[ReminderQuestion]
           ,[ReminderAnswer]           ,[DateOfBirth]           ,[Lastacdt]           ,[NumberOfLogInFailures]
           ,[PasswordInDB]           ,[UserPassword]           ,[UserPasswordHash]           ,[LoginSuccess]           ,[Comment])
           
           select @UsId ,  @ClientId ,  @FullName ,  @ClientName ,  @ClientPriorityBooking  ,  @PasswordExpired ,
  @Enabled ,  @IsPasswordEmpty ,  @GroupID ,  @ReminderQuestion ,  @ReminderAnswer,
  @DateOfBirth ,  @Lastacdt,  @NumberOfLogInFailures ,  @HashedPasswordInDB,  @Password,  @PasswordHashed,
  cast(0 as bit),  'Login failure:' + 'NumberOfLogInFailure' + CAST( @NumberOfLogInFailuresUpdateValue as varchar(1))
    
end
  UPDATE UserWeb
  SET NumberOfLogInFailures = @NumberOfLogInFailuresUpdateValue
  WHERE UPPER(Userid) = UPPER(@UserId)
END
ELSE
  SET @NumberOfLogInFailures = NULL

SELECT
  @ClientId AS 'UserStoreID',
  @UsId AS 'UserId',
  @FullName AS 'UserName',
  @ClientName AS 'UserStoreName',
  @ClientPriorityBooking AS 'ClientPriorityBooking',
  @PasswordExpired AS 'PasswordExpired',
  @Enabled AS 'Enabled',
  @IsPasswordEmpty AS 'IsPasswordEmpty',
  @GroupID AS 'GroupID',
  @ReminderQuestion AS 'ReminderQuestion', 
  @ReminderAnswer AS 'ReminderAnswer',
  @DateOfBirth AS 'DateOfBirth',
  @Lastacdt AS 'Lastacdt',
  @NumberOfLogInFailures AS 'NumberOfLogInFailures'

GO



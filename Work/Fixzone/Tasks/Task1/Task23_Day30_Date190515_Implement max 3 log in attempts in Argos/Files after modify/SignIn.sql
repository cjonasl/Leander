ALTER PROCEDURE [dbo].[SignIn]
@UserId VARCHAR(MAX),
@Password VARCHAR(MAX)
AS 
	DECLARE @ClientId INT = null     
   
	DECLARE @ClientIdInDB INT = null     
     
	DECLARE @FullName VARCHAR(MAX) = null    
        
	DECLARE @PasswordExpired BIT    
   
	DECLARE @Enabled BIT = 1    
  
	DECLARE @PasswordInDB varbinary(20)    
     
	declare @clientpriorityB bit
	
	DECLARE @Lastacdt date   

	DECLARE @PasswordHashed varbinary(20) = NULL
	declare @reminderQuestion varchar(30) = NULL
    declare @reminderAnswer varchar(30) = NULL
	declare @DateOfBirth datetime = NULL
	declare @NumberOfLogInFailures int = NULL
	declare @NumberOfLogInFailuresUpdateValue int
 -- check is password empty
     
	 IF (@Password IS NOT NULL)
         SET @PasswordHashed = HASHBYTES('SHA1', @Password)     
     
	SELECT TOP 1
	  @PasswordInDB = [Password],
	  @ClientIdInDB = [UserWeb].ClientID,
	  @NumberOfLogInFailures = NumberOfLogInFailures,
	  @Enabled = [Enabled]
	FROM
	  [UserWeb]
	WHERE
	  UPPER(Userid) = UPPER(@UserId)    
  

   -- if password empty but user exist    
    
	IF ((@PasswordInDB is null or @PasswordInDB = HASHBYTES('SHA1', '')) AND @ClientIdInDB is not null)
	BEGIN    
   
  -- if password empty    
		UPDATE
		  [UserWeb]
		SET
		  Lastacdt = getdate(),
		  NumberOfLogInFailures = 0
		WHERE
		  UPPER(Userid) = UPPER(@UserId) AND
		  [Enabled] = 1
		
		select @ClientId = ClientID, @FullName = Fullname, @Enabled = [Enabled],    @PasswordExpired = case
        when PasswordValidUntilDate > GETDATE() then CAST(0 as bit)  else CAST(1 as bit)  end,
		@Lastacdt = Lastacdt,@reminderQuestion=ReminderQuestion,@reminderAnswer=ReminderAnswer,@DateOfBirth=DateOfBirth
    	from [UserWeb]  where upper(UserId) = upper(@UserId)      
    
		SELECT @ClientId as 'UserStoreID', @UserId as 'UserId', @FullName as 'UserName', ClientName as 'UserStoreName',     
		CAST(ClientPriorityBooking as BIT) as 'ClientPriorityBooking', @PasswordExpired as 'PasswordExpired', CAST(@Enabled as int) as 'Enabled',    
        CAST(1 as int) as 'IsPasswordEmpty',Cast(GroupID as int) as GroupID,
		@Lastacdt AS 'Lastacdt',@reminderQuestion as ReminderQuestion,@reminderAnswer as ReminderAnswer,@DateOfBirth AS DateOfBirth, NULL AS 'NumberOfLogInFailures'
		FROM Client  
		WHERE ClientID = @ClientId
	END 
	ELSE
	BEGIN  
		SELECT
		  @ClientId = ClientID,
		  @FullName = Fullname,
		  @Enabled = [Enabled],
		  @PasswordExpired = case when PasswordValidUntilDate > GETDATE() then CAST(0 as bit) else CAST(1 as bit) end,
		  @Lastacdt = Lastacdt,@reminderQuestion=ReminderQuestion,@reminderAnswer=ReminderAnswer,@DateOfBirth=DateOfBirth
	    FROM
		  [UserWeb]
		WHERE 
		  UPPER(Userid) = UPPER(@UserId) AND
		  [Password] = @PasswordHashed
     
        IF (@ClientId is null)    
	    BEGIN
		  IF (@NumberOfLogInFailures IS NOT NULL AND @Enabled = 1)
		  BEGIN
		    SET @NumberOfLogInFailures = @NumberOfLogInFailures + 1
			
			IF (@NumberOfLogInFailures = 3) --Disable user and reset NumberOfLogInFailures to 0
			BEGIN
		      exec dbo.DisableUser @UserId
			  SET @NumberOfLogInFailuresUpdateValue = 0
			END
			ELSE
			  SET @NumberOfLogInFailuresUpdateValue = @NumberOfLogInFailures

			UPDATE [UserWeb]
			SET NumberOfLogInFailures = @NumberOfLogInFailuresUpdateValue
			WHERE UPPER(Userid) = UPPER(@UserId)
		  END
		  ELSE IF (@NumberOfLogInFailures IS NOT NULL AND @Enabled = 0)
		    SET @NumberOfLogInFailures = NULL  --Only handle NumberOfLogInFailures when account is enabled
		  ELSE
		    SET @UserId = NULL --Indicate that user does not exist
	      
		  SELECT null as 'UserStoreID', @UserId as 'UserId', null as 'UserName', null as 'UserStoreName', CAST(0 as BIT) as 'ClientPriorityBooking',    
		  CAST(@Enabled as int) as 'Enabled', CAST(0 as int) as 'IsPasswordEmpty', CAST(0 as int ) as GroupID      ,'' as ReminderQuestion,'' as ReminderAnswer, NULL as DateOfBirth, @NumberOfLogInFailures as 'NumberOfLogInFailures'
	    END     
	    ELSE       
        BEGIN    
          --SetUp the LastLogin date    
		  UPDATE
		    [UserWeb]
		  SET
		    Lastacdt = getdate(),
		    NumberOfLogInFailures = 0
		  WHERE
		    UPPER(Userid) = UPPER(@UserId) AND
			[Password] = @PasswordHashed AND
			[Enabled] = 1
    
		  SELECT @ClientId as 'UserStoreID', @UserId as 'UserId', @FullName as 'UserName', ClientName as 'UserStoreName',     
		  CAST(ClientPriorityBooking as BIT) as 'ClientPriorityBooking', @PasswordExpired as 'PasswordExpired', CAST(@Enabled as int) as 'Enabled',    
 
		  CAST(0 as int) as 'IsPasswordEmpty' ,CAST(GroupID as int) as GroupID, @Lastacdt AS 'Lastacdt', @reminderQuestion as ReminderQuestion, @reminderAnswer as ReminderAnswer, @DateOfBirth AS DateOfBirth, NULL AS 'NumberOfLogInFailures' FROM Client 
    
		  WHERE ClientID = @ClientId  
        END
    END
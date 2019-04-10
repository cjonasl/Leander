SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
ALTER PROCEDURE [dbo].[SignIn]    
  
 @UserId VARCHAR(MAX),    
     
 @Password VARCHAR(max)    
  
    
AS    
       
	DECLARE @ClientId INT = null     
   
	DECLARE @ClientIdInDB INT = null     
     
	DECLARE @FullName VARCHAR(MAX) = null    
        
	DECLARE @PasswordExpired BIT    
   
	DECLARE @Enabled BIT = 1    
  
	DECLARE @PasswordInDB varbinary(20)    
     
	declare @clientpriorityB bit
	
	DECLARE @DisabledDate date   

	DECLARE @PasswordHashed varbinary(20) = HASHBYTES('SHA1', '')
     
 -- check is password empty
     
	 IF (@Password IS NOT NULL)
         SET @PasswordHashed = HASHBYTES('SHA1', @Password)     
     
	SELECT TOP 1 @PasswordInDB = [UserWeb].Password,   @ClientIdInDB = [UserWeb].ClientID    
    
	FROM [UserWeb] WHERE upper([UserWeb].Userid) = upper(@UserId)    
  
  
   -- if password empty but user exist    
    
	IF (@PasswordInDB is null AND @ClientIdInDB is not null)
	BEGIN    
   
  -- if password empty    
		UPDATE [UserWeb] SET [UserWeb].Lastacdt = GETDATE()     where upper(UserId) = upper(@UserId)   and Password = @PasswordHashed     
		
		select @ClientId = ClientID, @FullName = Fullname, @Enabled = Enabled,    @PasswordExpired = case     
        when PasswordValidUntilDate > GETDATE() then CAST(0 as bit)  else CAST(1 as bit)  end,
		@DisabledDate = DisabledDate
    	from [UserWeb]  where upper(UserId) = upper(@UserId)      
    
		SELECT @ClientId as 'UserStoreID', @UserId as 'UserId', @FullName as 'UserName', ClientName as 'UserStoreName',     
		CAST(ClientPriorityBooking as BIT) as 'ClientPriorityBooking', @PasswordExpired as 'PasswordExpired', CAST(@Enabled as int) as 'Enabled',    
        CAST(1 as int) as 'IsPasswordEmpty',Cast(GroupID as int) as GroupID,
		@DisabledDate AS 'DisabledDate'
		FROM Client  
		WHERE ClientID = @ClientId
	END 
	ELSE
	BEGIN  
		select @ClientId = ClientID, @FullName = Fullname, @Enabled = Enabled,  @PasswordExpired = case     
        when PasswordValidUntilDate > GETDATE()    then CAST(0 as bit)       
        else CAST(1 as bit) end, @DisabledDate = DisabledDate
	    from [UserWeb] where upper(UserId) = upper(@UserId) and Password = @PasswordHashed
     
        IF (@ClientId is null)    
	    BEGIN      
		  SELECT null as 'UserStoreID', null as 'UserId', null as 'UserName', null as 'UserStoreName', CAST(0 as BIT) as 'ClientPriorityBooking',    
		  CAST(@Enabled as int) as 'Enabled', CAST(0 as int) as 'IsPasswordEmpty', CAST(0 as int ) as GroupID      
	    END     
	    ELSE       
        BEGIN    
          --SetUp the LastLogin date    
		  UPDATE [UserWeb]     SET [UserWeb].Lastacdt = GETDATE()   where upper(UserId) = upper(@UserId) and Password = @PasswordHashed   
    
		  SELECT @ClientId as 'UserStoreID', @UserId as 'UserId', @FullName as 'UserName', ClientName as 'UserStoreName',     
		  CAST(ClientPriorityBooking as BIT) as 'ClientPriorityBooking', @PasswordExpired as 'PasswordExpired', CAST(@Enabled as int) as 'Enabled',    
     
		  CAST(0 as int) as 'IsPasswordEmpty' ,CAST(GroupID as int) as GroupID, @DisabledDate AS 'DisabledDate' FROM Client 
    
		  WHERE ClientID = @ClientId  
        END
    END
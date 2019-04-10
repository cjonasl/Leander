USE [Cast]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SignIn]
@UserId VARCHAR(max),
@Password VARCHAR(max)
AS
DECLARE
@PasswordInDB varchar(max),
@ClientIdInDB int,
@ClientpriorityB bit,
@Case int,
@PasswordInt int,
@FullName varchar(max),
@Enabled bit,
@PasswordExpired bit,
@IsPasswordEmpty int,
@DisabledDate date


-- Set default values --
SET @PasswordInDB = ''
SET @ClientIdInDB = NULL
SET @IsPasswordEmpty = 0
SET @ClientpriorityB = 0

SELECT TOP 1
  @PasswordInDB = ISNULL([Password], ''),
  @ClientIdInDB = [ClientID],
  @FullName = Fullname,
  @Enabled = [Enabled],
  @PasswordExpired = CASE WHEN PasswordValidUntilDate > GETDATE() THEN CAST(0 as bit) ELSE CAST(1 as bit) END,
  @DisabledDate = DisabledDate
 FROM 
  [UserWeb]
WHERE
  UPPER(Userid) = UPPER(@UserId)

SELECT @ClientpriorityB = clientprioritybooking 
FROM client 
WHERE clientid = @ClientIdInDB

IF (ISNUMERIC(@Password) = 1)
  SET @PasswordInt = CAST(@Password AS int)
ELSE
  SET @PasswordInt = NULL

/*
  Different cases:
  Case 1: User does not exist
  Case 2: User exists, @ClientpriorityB = 0 and @PasswordInDB is null or empty
  Case 3: User exists, @ClientpriorityB = 0 and @PasswordInDB is not null and not empty and @PasswordInDB = @Password
  Case 4: User exists, @ClientpriorityB = 0 and @PasswordInDB is not null and not empty and @PasswordInDB <> @Password
  Case 5: User exists, @ClientpriorityB = 1 and @Password = UserWeb.ClientID
  Case 6: User exists, @ClientpriorityB = 1 and @Password <> UserWeb.ClientID
*/

IF (@ClientIdInDB IS NULL)
  SET @Case = 1
ELSE IF (@ClientpriorityB = 0 AND @PasswordInDB = '')
BEGIN
  SET @IsPasswordEmpty = 1
  SET @Case = 2
END
ELSE IF (@ClientpriorityB = 0 AND @PasswordInDB <> '' AND @PasswordInDB = @Password)
  SET @Case = 3
ELSE IF (@ClientpriorityB = 0 AND @PasswordInDB <> '' AND @PasswordInDB <> @Password)
  SET @Case = 4
ELSE IF (@ClientpriorityB = 1 AND @ClientIdInDB = @PasswordInt)
  SET @Case = 5
ELSE
  SET @Case = 6

IF (@Case = 1 OR @Case = 4 OR @Case = 6)
BEGIN
  SELECT
    NULL AS 'UserStoreID',
    NULL AS 'UserId',
    NULL AS 'UserName',
    NULL AS 'UserStoreName',
    @ClientpriorityB AS 'ClientPriorityBooking',
    CAST(1 AS int) AS 'Enabled',
    @IsPasswordEmpty AS 'IsPasswordEmpty',
    CAST(0 AS int ) AS 'GroupID',
    @DisabledDate AS 'DisabledDate'
END
ELSE
BEGIN
  SELECT 
    @ClientIdInDB AS 'UserStoreID',
    @UserId AS 'UserId',
    @FullName AS 'UserName',
    ClientName AS 'UserStoreName', 
    @ClientpriorityB AS 'ClientPriorityBooking',
    @PasswordExpired AS 'PasswordExpired',
    CAST(@Enabled AS int) AS 'Enabled',
    @IsPasswordEmpty AS 'IsPasswordEmpty',
    Cast(GroupID AS int) AS 'GroupID',
    @DisabledDate AS 'DisabledDate'
  FROM
    Client
  WHERE
    ClientID = @ClientIdInDB

  IF (@Case <> 2)
  BEGIN
    UPDATE [UserWeb]
    SET Lastacdt = GETDATE()
    WHERE UPPER(Userid) = UPPER(@UserId)
  END
END


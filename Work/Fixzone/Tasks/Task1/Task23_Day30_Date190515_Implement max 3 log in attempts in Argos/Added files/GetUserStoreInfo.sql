CREATE PROCEDURE GetUserStoreInfo
@UserId varchar(max),       
@Password varchar(max)
AS
DECLARE @ClientId int = NULL
DECLARE @PasswordInDB varbinary(20) = NULL
DECLARE @Enabled bit = NULL
DECLARE @PasswordHashed varbinary(20) = NULL
DECLARE @UserCanLogIn bit = CAST(0 AS bit) --Default

IF (@Password IS NOT NULL)
  SET @PasswordHashed = HASHBYTES('SHA1', @Password) 

SELECT TOP 1
  @PasswordInDB = [Password],
  @ClientId = ClientID,
  @Enabled = [Enabled]
FROM
  [UserWeb]
WHERE
  UPPER(Userid) = UPPER(@UserId) 

IF (@ClientId IS NOT NULL AND @Enabled = 1 AND (@PasswordInDB IS NULL OR @PasswordInDB = HASHBYTES('SHA1', '')))
  SET @UserCanLogIn = CAST(1 AS bit)
ELSE IF (@ClientId IS NOT NULL AND @Enabled = 1)
BEGIN
  SET @ClientId = NULL

  SELECT
    @ClientId = ClientID
  FROM
    [UserWeb]
  WHERE 
    UPPER(Userid) = UPPER(@UserId) AND
	[Password] = @PasswordHashed

  IF (@ClientId IS NOT NULL)
    SET @UserCanLogIn = CAST(1 AS bit)
END

IF (@UserCanLogIn = 1)
BEGIN
  SELECT @UserCanLogIn AS 'UserCanLogIn', @ClientId AS 'UserStoreID', ClientName AS 'UserStoreName'
  FROM Client
  WHERE ClientID = @ClientId
END
ELSE
BEGIN
  SELECT @UserCanLogIn AS 'UserCanLogIn', NULL AS 'UserStoreID', NULL AS 'UserStoreName'
END
GO
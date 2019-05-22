CREATE PROCEDURE GetUserStoreInfo
@UserId varchar(max),       
@Password varchar(max)
AS
DECLARE @ClientId int = NULL
DECLARE @ClientName varchar(50) = NULL
DECLARE @Enabled bit = CAST(0 AS bit)
DECLARE @IsPasswordEmpty int = 0
DECLARE @UserCanLogIn bit = CAST(0 AS bit) --Default
DECLARE @PasswordHashed varbinary(20) = CASE WHEN @Password IS NOT NULL THEN HASHBYTES('SHA1', @Password) ELSE HASHBYTES('SHA1', '') END
DECLARE @HashedPasswordInDB varbinary(20) = NULL

SELECT
  @ClientId = u.ClientID,
  @ClientName = c.ClientName,
  @Enabled = u.[Enabled],
  @IsPasswordEmpty = CASE WHEN [Password] IS NULL OR [Password] = HASHBYTES('SHA1', '') THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
  @HashedPasswordInDB = u.[Password]
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  UPPER(Userid) = UPPER(@UserId)

IF (@Enabled = 1 AND (@IsPasswordEmpty = 1 OR (@PasswordHashed = @HashedPasswordInDB))) --Log in succcess
BEGIN
  SELECT CAST(1 AS bit) AS 'UserCanLogIn', @ClientId AS 'UserStoreID', @ClientName AS 'UserStoreName'
END
ELSE
BEGIN
  SELECT CAST(0 AS bit) AS 'UserCanLogIn', NULL AS 'UserStoreID', NULL AS 'UserStoreName'
END
GO
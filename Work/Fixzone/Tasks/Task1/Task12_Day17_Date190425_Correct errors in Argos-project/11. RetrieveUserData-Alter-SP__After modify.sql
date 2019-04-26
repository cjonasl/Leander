ALTER PROCEDURE [dbo].[RetrieveUserData]
@UserID VARCHAR(max)
/*
  Modify 15/4/2019, Jonas Leander
  Change the way password is handled now when it is hashed.
*/
AS
    SELECT U.Fullname       AS Name,
        U.Title             AS JobRole,
        CAST(U.[Enabled] AS BIT) AS IsEnabled,      
        CAST(U.DateOfBirth as DATETIME) AS  DateOfBirth,
        CASE
		  WHEN  (U.[Password] IS NULL OR U.[Password] = HASHBYTES('SHA1', '')) THEN ''
		  ELSE '!$&1(Ah)q6' --Code to use when indicate that the password is set
		 END
		 AS Password,
        U.Level             AS UserLevel,
        U.Userid            AS  UserId  
                
    FROM [UserWeb] U
    WHERE upper(U.Userid) = upper(@UserID)

GO









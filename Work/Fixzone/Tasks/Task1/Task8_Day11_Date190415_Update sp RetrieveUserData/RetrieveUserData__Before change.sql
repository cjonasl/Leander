ALTER PROCEDURE [dbo].[RetrieveUserData]
    @UserID VARCHAR(max)
AS
    SELECT U.Fullname       AS Name,
        U.Title             AS JobRole,
        CAST(U.[Enabled] AS BIT) AS IsEnabled,      
        CAST(U.DateOfBirth as DATETIME) AS  DateOfBirth,
        U.Password          AS Password,
        U.Level             AS UserLevel,
        U.Userid            AS  UserId  
                
    FROM [UserWeb] U
    WHERE upper(U.Userid) = upper(@UserID)

GO









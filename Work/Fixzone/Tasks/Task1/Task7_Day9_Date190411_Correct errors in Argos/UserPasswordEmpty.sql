ALTER PROCEDURE [dbo].[UserPasswordEmpty]
@UserId VARCHAR (MAX)
/*
  Modify 11/4/2019, Jonas Leander
  Handle hash of password
*/
AS
DECLARE @Empty bit = 1
	
SELECT @Empty = CAST(CASE WHEN [Password] IS NOT NULL THEN 0 ELSE 1 END AS bit)
FROM [UserWeb]
WHERE UPPER(Userid) = UPPER(@UserId)

SELECT @Empty
ALTER PROCEDURE [dbo].[AddNewUser]
@UserId varchar(MAX),
@Name varchar(MAX),
@UserLevel INT,
@StoreNumber INT,
@DateOfBirth DATETIME= null,
@CreatedBy varchar(MAX)
/*
  Modify 11/4/2019, Jonas Leander
  Handle hash of password
*/
AS
DECLARE @pass varbinary(20) --Chnage from varchar(10) to varbinar(20)
SET @pass = NULL --Change from '' to NULL

INSERT INTO [dbo].[UserWeb] (Userid, Fullname, [Level], ClientID, [Password], DateOfBirth, CreatedBy, Created)
VALUES (@UserId, @Name, @UserLevel, @StoreNumber,@pass,@DateOfBirth, @CreatedBy, GETDATE())



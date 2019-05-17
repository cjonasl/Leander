USE [CAST_ClientDemo]
GO

BEGIN TRAN

CREATE TABLE [dbo].[UserWeb_Bak13May2019](
	[_id] int NOT NULL,
	[Userid] [varchar](25) NULL,
	[Password] [varchar](20) NULL,
	[Fullname] [varchar](70) NULL,
	[Title] [varchar](40) NULL,
	[Level] [int] NULL,
	[Lastacdt] [date] NULL,
	[Address] [varchar](max) NULL,
	[EmailAddr] [varchar](64) NULL,
	[Fax] [varchar](40) NULL,
	[SendBy] [smallint] NULL,
	[NxLetter] [int] NULL,
	[AccLogin] [varchar](20) NULL,
	[AccPassword] [varchar](20) NULL,
	[StockWarehouse] [varchar](10) NULL,
	[CostCentre] [varchar](10) NULL,
	[Department] [varchar](10) NULL,
	[LoginType] [varchar](10) NULL,
	[BenchEngineerID] [int] NULL,
	[TelNo] [varchar](20) NULL,
	[Mobile] [varchar](20) NULL,
	[Created] [datetime] NULL,
	[CreatedBy] [varchar](11) NULL,
	[Enabled] [bit] NULL,
	[DateOfBirth] [datetime] NULL,
	[ClientID] [int] NULL,
	[ReminderQuestion] [varchar](60) NULL,
	[ReminderAnswer] [varchar](20) NULL,
	[PasswordValidUntilDate] [datetime] NULL
)
GO

INSERT INTO UserWeb_Bak13May2019
(
	[_id],
	[Userid],
	[Password],
	[Fullname],
	[Title],
	[Level],
	[Lastacdt],
	[Address],
	[EmailAddr],
	[Fax],
	[SendBy],
	[NxLetter],
	[AccLogin],
	[AccPassword],
	[StockWarehouse],
	[CostCentre],
	[Department],
	[LoginType],
	[BenchEngineerID],
	[TelNo],
	[Mobile],
	[Created],
	[CreatedBy],
	[Enabled],
	[DateOfBirth],
	[ClientID],
	[ReminderQuestion],
	[ReminderAnswer],
	[PasswordValidUntilDate]
)
SELECT
	[_id],
	[Userid],
	[Password],
	[Fullname],
	[Title],
	[Level],
	[Lastacdt],
	[Address],
	[EmailAddr],
	[Fax],
	[SendBy],
	[NxLetter],
	[AccLogin],
	[AccPassword],
	[StockWarehouse],
	[CostCentre],
	[Department],
	[LoginType],
	[BenchEngineerID],
	[TelNo],
	[Mobile],
	[Created],
	[CreatedBy],
	[Enabled],
	[DateOfBirth],
	[ClientID],
	[ReminderQuestion],
	[ReminderAnswer],
	[PasswordValidUntilDate]
FROM
   [dbo].[UserWeb]
GO

----------------------------- Number 1 -----------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'DeletedUserWebRecordsSinceLastReport')
DROP TABLE DeletedUserWebRecordsSinceLastReport
GO

CREATE TABLE DeletedUserWebRecordsSinceLastReport
(
  [Userid] [varchar](25) COLLATE Latin1_General_CI_AS NULL,
  [Fullname] [varchar](70) COLLATE Latin1_General_CI_AS NULL,
  [Store] [varchar](50) COLLATE Latin1_General_CI_AS NULL,
  [LastLoggedInDate] [date] NULL,
  [DisabledDate] [date] NULL,
  [DeletedDate] [date] NULL
)
GO

----------------------------- Number 2 -----------------------------------------------------------------
ALTER TABLE UserWeb
ADD DisabledDate date NULL
GO


----------------------------- Number 3 -----------------------------------------------------------------
DECLARE
@DateToday datetime

SET @DateYesterday = dateadd(dd, -1, getdate()) 

update userweb set [password] = clientid, passwordvaliduntildate = @DateYesterday
where clientid in (select clientid from client where clientprioritybooking=1)
GO

DROP INDEX idxPwd ON UserWeb
GO

ALTER TABLE UserWeb
ALTER COLUMN [Password] varchar(20) NULL
GO

ALTER TABLE UserWeb
ADD tmpPassword varchar(20) NULL
GO

UPDATE UserWeb
SET tmpPassword = [Password]

UPDATE UserWeb
SET tmpPassword = NULL
WHERE LTRIM(RTRIM(tmpPassword)) = ''

UPDATE UserWeb
SET [Password] = NULL

ALTER TABLE UserWeb
ALTER COLUMN [Password] decimal(1,0)

ALTER TABLE UserWeb
ALTER COLUMN [Password] varbinary(20) NULL

UPDATE UserWeb
SET [Password] = HASHBYTES('SHA1', tmpPassword)
WHERE tmpPassword IS NOT NULL

ALTER TABLE UserWeb
DROP COLUMN tmpPassword
GO


----------------------------- Number 4 -----------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'DisableUser')
DROP PROCEDURE DisableUser
GO

CREATE PROCEDURE DisableUser
/*
  Author: Jonas Leander
  Date: 5/4/2019
  Description: Disable a user (Jira A-12)
*/
@Userid varchar(25)
AS
UPDATE
  UserWeb
SET
  [Enabled] = 0,
  DisabledDate = getdate()
WHERE
  Userid = @Userid
GO


----------------------------- Number 5 -----------------------------------------------------------------
ALTER PROCEDURE [dbo].[SaveNewPassword]
	@pass VARCHAR(max), 
	@userId VARCHAR(max)
/*
  Modify in AR-11, 8/4/2019, Jonas Leander
  Change frm 90 to 60 days in DATEADD for PasswordValidUntilDate

  Modify in AR-16, 9/4/2019, Jonas Leander
  Hash password
*/
AS
	DECLARE 
        @previousPassword varbinary(20),
        @newPassword varbinary(20)

        SET @newPassword = HASHBYTES('SHA1', @pass)
	
	SELECT @previousPassword = [UserWeb].Password 
	FROM [UserWeb] 
	WHERE upper([Userid]) = upper(@userId)

	if @previousPassword = @newPassword
		BEGIN
			SELECT CAST(1 as BIT)
		END
	else 
		BEGIN
			UPDATE [UserWeb]
			SET [Password] = @newPassword,
				[PasswordValidUntilDate] = DATEADD(DAY, 60,GETDATE())
			WHERE upper([Userid]) = upper(@userId)

			SELECT CAST(0 as BIT)
		END
GO


----------------------------- Number 6 -----------------------------------------------------------------
ALTER PROCEDURE [dbo].[UserPasswordEmpty]
@UserId VARCHAR (MAX)
/*
  Modify 11/4/2019, Jonas Leander
  Handle hash of password
*/
AS
DECLARE @Empty bit = 1
	
SELECT
  @Empty = 
  CAST(
    CASE
	  WHEN [Password] IS NULL OR [Password] = HASHBYTES('SHA1', '') THEN 1
	  ELSE 0
	END
  AS bit)
FROM
  [UserWeb]
WHERE
  UPPER(Userid) = UPPER(@UserId)

SELECT @Empty
GO


----------------------------- Number 7 -----------------------------------------------------------------
ALTER PROCEDURE [dbo].[ChangePassword]
@pass VARCHAR(max), 
@userId VARCHAR(max)
/*
  Modify 11/4/2019, Jonas Leander
  1. Handle hash of password
  2. Add update of column PasswordValidUntilDate
*/
AS
DECLARE
@passwordHashed varbinary(20)
	
SET @passwordHashed = HASHBYTES('SHA1', @pass)

UPDATE
  [UserWeb]
SET
  [Password] = @passwordHashed,
  [PasswordValidUntilDate] = DATEADD(DAY, 60, GETDATE()),
  NumberOfLogInFailures = 0
WHERE
  [Userid] = @userId
GO


----------------------------- Number 8 -----------------------------------------------------------------
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
DECLARE @pass varbinary(20) --Change from varchar(10) to varbinar(20)
SET @pass = NULL --Change from '' to NULL

INSERT INTO [dbo].[UserWeb] (Userid, Fullname, [Level], ClientID, [Password], DateOfBirth, CreatedBy, Created)
VALUES (@UserId, @Name, @UserLevel, @StoreNumber,@pass,@DateOfBirth, @CreatedBy, GETDATE())
GO


----------------------------- Number 9 -----------------------------------------------------------------
ALTER PROCEDURE [dbo].[AddDetails]
@pass VARCHAR(max), 
@question VARCHAR(max),
@answer VARCHAR(max),
@userId varchar(max)
/*
   Modify 11/4/2019, Jonas Leander
   1. Change from 90 to 60 days in DATEADD
   2. Handle hash of password
*/
AS
DECLARE
@passwordHashed varbinary(20)
	
SET @passwordHashed = HASHBYTES('SHA1', @pass)

UPDATE
  [UserWeb] 
SET
  [Password]= @passwordHashed,
  [ReminderQuestion]= @question,
  [ReminderAnswer]= @answer,
  [PasswordValidUntilDate] = DATEADD(DAY, 60, GETDATE())
WHERE 
  UPPER([Userid]) = UPPER(@userId)
GO


----------------------------- Number 10 -----------------------------------------------------------------
ALTER PROCEDURE UpdateUser
/*
  Modify in AR-7, 5/4/2019, Jonas Leander
  Add update of column DisabledDate.

  Modify in AR-16, 9/4/2019, Jonas Leander
  Hash password.
*/
@UserId VARCHAR(max) = NULL,
@Name VARCHAR(70),
@IsEnabled BIT,
@DateOfBirth DATETIME = NULL,
@UserLevel INT,
@Password VARCHAR(max) = '',
@PreviousUserId VARCHAR(max) = NULL
AS
DECLARE
@IsEnabledPrevious bit,
@DisabledDate date,
@Lastacdt date,
@PasswordHashed varbinary(20),
@NewPasswordHashed varbinary(20),
@PasswordValidUntilDate datetime

SET @NewPasswordHashed = HASHBYTES('SHA1', ISNULL(@Password, ''))

IF (@UserId IS NULL)
BEGIN
  INSERT INTO [UserWeb](Fullname, [Enabled], DateOfBirth, Userid, [Password], [Level], PasswordValidUntilDate)
  VALUES(@Name, @IsEnabled, @DateOfBirth, @UserId, @NewPasswordHashed, @UserLevel, DATEADD(DAY, 60, GETDATE()));
END
ELSE
BEGIN
  SELECT
    @PasswordHashed = [Password],
    @Lastacdt = Lastacdt,
    @IsEnabledPrevious = ISNULL([Enabled], 0),
    @DisabledDate = DisabledDate,
	@PasswordValidUntilDate = PasswordValidUntilDate
  FROM
    [UserWeb]
  WHERE
    upper(Userid) = upper(@PreviousUserId)

IF (@IsEnabledPrevious = 0 AND @IsEnabled = 1) --Reactivate
BEGIN
  SET @Lastacdt = getdate()
  SET @DisabledDate = NULL
END
ELSE IF (@IsEnabledPrevious = 1 AND @IsEnabled = 0)
  SET @DisabledDate = getdate()

IF (@Password <> '!$&1(Ah)q6') --Password was updated ('!$&1(Ah)q6' is used to indicate no update)
BEGIN
  SET @PasswordHashed = @NewPasswordHashed
  SET @PasswordValidUntilDate = DATEADD(DAY, 60, GETDATE())
END

UPDATE
  [UserWeb]
SET
  Lastacdt = @Lastacdt,
  Fullname = @Name,
  [Enabled] = @IsEnabled,
  DateOfBirth = @DateOfBirth,
  Userid = @UserId,
  [Level] = @UserLevel,
  [Password] = @PasswordHashed,
  DisabledDate = @DisabledDate,
  PasswordValidUntilDate = @PasswordValidUntilDate
WHERE 
  upper(Userid) = upper(@PreviousUserId)
END
GO


----------------------------- Number 11 -----------------------------------------------------------------
ALTER PROCEDURE [dbo].[RetrieveUserData]
@UserID VARCHAR(max)
/*
  Modify 15/4/2019, Jonas Leander
  Change the way password is handled now when it is hashed.
  Add ReminderQuestion and ReminderAnswer in the select list.
*/
AS
  SELECT
    U.Fullname AS Name,
    U.Title AS JobRole,
    CAST(U.[Enabled] AS BIT) AS IsEnabled,      
    CAST(U.DateOfBirth as DATETIME) AS  DateOfBirth,
    CASE
      WHEN  (U.[Password] IS NULL OR U.[Password] = HASHBYTES('SHA1', '')) THEN ''
      ELSE '!$&1(Ah)q6' --Code to use when indicate that the password is set
    END AS Password,
    U.Level AS UserLevel,
    U.Userid AS UserId,
    U.ReminderQuestion,
    U.ReminderAnswer              
  FROM
    [UserWeb] U
   WHERE
     upper(U.Userid) = upper(@UserID)
GO


----------------------------- Number 12 -----------------------------------------------------------------
--AR-14
DECLARE
@PageID int

SELECT @PageID = MAX(PageID)
FROM ProcessPage

SET @PageID = 1 + @PageID

INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(@PageID, '/Question/ConfirmYes', 'Consent for marketing question when booking a repair')

UPDATE ProcessDetail
SET ProcessStep = ProcessStep + 1
WHERE ProcessID = 4

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID)
VALUES(4, 1, 1, @PageID)
GO


----------------------------- Number 13 -----------------------------------------------------------------
UPDATE ProcessHeader
SET ProcessAccessLevel = 0
WHERE ProcessID = 18
GO


----------------------------- Number 14 -----------------------------------------------------------------
ALTER TABLE UserWeb
ADD NumberOfLogInFailures int NOT NULL DEFAULT(0)
GO


----------------------------- Number 15 -----------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'GetUserStoreInfo')
DROP PROCEDURE GetUserStoreInfo
GO

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

----------------------------- Number 16 -----------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'UserInformation')
DROP PROCEDURE UserInformation
GO

CREATE PROCEDURE [dbo].[UserInformation]
@UserId varchar(max)
AS
SELECT
  u.ClientID AS 'UserStoreID',
  u.Userid,
  u.Fullname AS 'UserName',
  c.ClientName AS 'UserStoreName',
  c.ClientPriorityBooking AS 'ClientPriorityBooking',
  CASE WHEN PasswordValidUntilDate > getdate() THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS 'PasswordExpired',
  u.[Enabled],
  CASE WHEN [Password] IS NULL OR [Password] = HASHBYTES('SHA1', '') THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS 'IsPasswordEmpty',
  CAST(c.GroupID AS int) AS 'GroupID',
  u.ReminderQuestion,
  u.ReminderAnswer,
  u.DateOfBirth,
  u.Lastacdt,
  NULL AS 'NumberOfLogInFailures'
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  UPPER(Userid) = UPPER(@UserId)
GO


----------------------------- Deployment script CheckDetails ---------------------------------------------
DECLARE
@PageID int

SELECT @PageID = MAX(PageID)
FROM ProcessPage

SET @PageID = 1 + @PageID

INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(@PageID, '/User/CheckDetails', 'Check that ReminderQuestion, ReminderAnswer and DateOfBirth are set')

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID)
VALUES(18, 2, 1, @PageID)
GO


---------------------------- Rep_DELETEUser --------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'DeleteTmpStoredDeletedUsers')
DROP PROCEDURE DeleteTmpStoredDeletedUsers
GO

CREATE PROCEDURE DeleteTmpStoredDeletedUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-6, to delete all rows in table DeletedUserWebRecordsSinceLastReport
  after a report has been successfully generated by stored procedure
  GenerateReportDisabledDeletedUsers.
*/
AS
TRUNCATE TABLE DeletedUserWebRecordsSinceLastReport
GO


---------------------------- Rep_DELETEUsers -------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'DeleteUsers')
DROP PROCEDURE DeleteUsers
GO

CREATE PROCEDURE DeleteUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-7, to delete users in table UserWeb and move them
  to table DeletedUserWebRecordsSinceLastReport if they have
  been disabled for more than 30 days.
*/
AS
DECLARE
@DateToday date

SET @DateToday = getdate()

INSERT INTO DeletedUserWebRecordsSinceLastReport
(
  Userid,
  Fullname,
  Store,
  LastLoggedInDate,
  DisabledDate,
  DeletedDate
)
SELECT
  u.Userid,
  u.Fullname,
  c.ClientName,
  u.Lastacdt,
  u.DisabledDate,
  @DateToday AS 'DeletedDate'
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  (Lastacdt IS NULL OR datediff(day, Lastacdt, @DateToday) > 120) AND
  u.DisabledDate IS NOT NULL AND 
  datediff(day, u.DisabledDate, @DateToday) > 30

DELETE FROM UserWeb
WHERE Userid IN(SELECT Userid FROM DeletedUserWebRecordsSinceLastReport)
GO


---------------------------- Rep_DELETEUsers -------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'GenerateReportDisabledDeletedUsers')
DROP PROCEDURE GenerateReportDisabledDeletedUsers
GO

CREATE PROCEDURE GenerateReportDisabledDeletedUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-6, to generate a report on a regular basis
  for disabled and deleted users.
*/
AS
SELECT
  u.Userid,
  u.Fullname,
  c.ClientName AS 'Store',
  u.Lastacdt AS 'LastLoggedInDate',
  u.DisabledDate,
  NULL AS 'DeletedDate'
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  u.[Enabled] = 0
UNION
SELECT
  Userid,
  Fullname,
  Store,
  LastLoggedInDate,
  DisabledDate,
  DeletedDate
FROM
  DeletedUserWebRecordsSinceLastReport
ORDER BY
  DeletedDate, Fullname
GO


---------------------------- Rep_DELETEUsers -------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'DisableUsers')
DROP PROCEDURE DisableUsers
GO

CREATE PROCEDURE DisableUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-7, to disable users if they been inactive
  for more than 90 days.
*/
AS
DECLARE
@DateToday date

SET @DateToday = getdate()

UPDATE
  [dbo].[UserWeb]
SET
  [Enabled] = 0,
  DisabledDate = @DateToday
WHERE
  DisabledDate IS NULL AND
  (Lastacdt IS NULL OR datediff(day, Lastacdt, @DateToday) > 90)
GO


---------------------------- Signin -------------------------------------------------------------
ALTER PROCEDURE [dbo].[SignIn]
@UserId varchar(max),
@Password varchar(max)
AS
DECLARE @ClientId int = NULL
DECLARE @FullName varchar(max) = NULL
DECLARE @ClientName varchar(50) = NULL
DECLARE @ClientPriorityBooking bit = CAST(0 AS bit)
DECLARE @PasswordExpired bit = CAST(0 AS bit)
DECLARE @Enabled bit = CAST(0 AS bit)
DECLARE @IsPasswordEmpty int = 0
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
  UPPER(Userid) = UPPER(@UserId)

IF (@Enabled = 1 AND (@IsPasswordEmpty = 1 OR (@PasswordHashed = @HashedPasswordInDB))) --Log in succcess
BEGIN
  UPDATE
    UserWeb
  SET
    Lastacdt = getdate(),
    NumberOfLogInFailures = 0
  WHERE
    UPPER(Userid) = UPPER(@UserId)
END

IF (@Enabled = 1 AND @IsPasswordEmpty = 0 AND @PasswordHashed <> @HashedPasswordInDB) --Handle NumberOfLogInFailures
BEGIN
  SET @NumberOfLogInFailures = @NumberOfLogInFailures + 1
			
  IF (@NumberOfLogInFailures = 3) --Disable user and reset NumberOfLogInFailures to 0
  BEGIN
    exec dbo.DisableUser @UserId
    SET @NumberOfLogInFailuresUpdateValue = 0
	SET @Enabled = CAST(0 AS bit)
  END
  ELSE
    SET @NumberOfLogInFailuresUpdateValue = @NumberOfLogInFailures

  UPDATE UserWeb
  SET NumberOfLogInFailures = @NumberOfLogInFailuresUpdateValue
  WHERE UPPER(Userid) = UPPER(@UserId)
END
ELSE
  SET @NumberOfLogInFailures = NULL

SELECT
  @ClientId AS 'UserStoreID',
  @UserId AS 'UserId',
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


---------------------------- UpdateUserDetails -------------------------------------------------------------
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'UpdateUserDetails')
DROP PROCEDURE UpdateUserDetails
GO

CREATE PROCEDURE UpdateUserDetails 
 
@question VARCHAR(max),
@answer VARCHAR(max),
@userId varchar(max),@DateOfBirth DATETIME= null
AS
  
Update userweb set ReminderQuestion=@question,ReminderAnswer =@answer ,DateOfBirth=@DateOfBirth where userid=@userId
GO


IF (@@ERROR = 0)
  COMMIT TRAN
ELSE
  ROLLBACK TRAN
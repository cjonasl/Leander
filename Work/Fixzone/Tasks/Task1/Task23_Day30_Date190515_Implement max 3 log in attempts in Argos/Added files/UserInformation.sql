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
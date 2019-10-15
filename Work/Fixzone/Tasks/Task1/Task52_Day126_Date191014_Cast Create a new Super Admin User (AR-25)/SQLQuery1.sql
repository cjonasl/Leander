SELECT
  aaa._id,
  aaa.Userid,
  bbb.[ClientPriorityBooking],
  bbb.[ClientID],
  bbb.[ClientName],
  bbb.[ClientShortname],
  aaa.[Password],
  aaa.Fullname,
  aaa.Lastacdt,
  aaa.[Level],
  aaa.[Enabled],
  aaa.[ReminderQuestion],
  aaa.[ReminderAnswer],
  aaa.[PasswordValidUntilDate],
  aaa.[DisabledDate],
  aaa.[NumberOfLogInFailures]
FROM
  [dbo].[UserWeb] aaa
  INNER JOIN Client bbb ON aaa.ClientId = bbb.ClientId
ORDER BY
  bbb.[ClientPriorityBooking],
  bbb.[ClientID]
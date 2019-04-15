SELECT * FROM [UserWeb]
WHERE [Level] = 1 AND PasswordValidUntilDate IS NOT NULL AND PasswordValidUntilDate > GETDATE() AND [Enabled] = 1 AND
ClientID IN(SELECT ClientID FROM Client WHERE [ClientPriorityBooking] = 0)


-- [ClientPriorityBooking] = 0
UPDATE [UserWeb]
SET [Password] = HASHBYTES('SHA1', 'abc')
WHERE UserId = 'AD6907'

-- [ClientPriorityBooking] = 1
UPDATE [UserWeb]
SET [Password] = HASHBYTES('SHA1', 'abc')
WHERE UserId = '2149366'


SELECT * FROM [UserWeb]
WHERE UserId = '11111'

SELECT * FROM [UserWeb]
WHERE UserId = '987654321'



SELECT
  UserId,
  [DateOfBirth],
  [ReminderQuestion],
  [ReminderAnswer],
  [PasswordValidUntilDate],
  [Enabled]
FROM
  UserWeb
WHERE
  [Password] IS NULL AND
  ClientID IN(SELECT ClientID FROM Client WHERE [ClientPriorityBooking] = 1) AND
  UserId = '4212513'

UPDATE UserWeb
SET PasswordValidUntilDate = '2019-05-01'
WHERE UserId = '4212513'


exec [dbo].[UserPasswordEmpty] '42550'

SELECT case when PasswordValidUntilDate > GETDATE() then CAST(0 as bit)  else CAST(1 as bit)  end
FROM UserWeb
WHERE UserId = '4212513'
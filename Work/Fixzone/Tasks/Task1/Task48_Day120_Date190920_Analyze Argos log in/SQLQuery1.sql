SELECT * FROM AnalyzeLogIn
WHERE UserIdExists = 1 AND LogInSuccess = 0
ORDER BY ID

SELECT * FROM UserWeb
WHERE UserId = '7742327'

SELECT * FROM AnalyzeLogIn
WHERE UserIdExists = 1 AND LogInSuccess = 0 AND NumberOfLogInFailures = 3
ORDER BY ID


SELECT * FROM AnalyzeLogIn
WHERE UserId = '2455986'
ORDER BY ID

SELECT * FROM AnalyzeLogIn2
WHERE UserId = '2455986'
ORDER BY ID

SELECT [Lastacdt], [Enabled], [NumberOfLogInFailures]
FROM [dbo].[UserWeb]
WHERE UserId = '2455986'



SELECT [Lastacdt], [NumberOfLogInFailures] FROM UserWeb
WHERE [Lastacdt] IS NOT NULL
ORDER BY [Lastacdt] desc


SELECT [Lastacdt], [Enabled], [NumberOfLogInFailures]
FROM [dbo].[UserWeb]
WHERE [Userid] IN('9015720', '4317866', '8444770', '4466352')


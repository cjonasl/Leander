--Database server: fzserver

USE [PASClientconnect]

SELECT * FROM [dbo].[UserWeb]
WHERE [Userid] = 'FIXPAUL'

SELECT ClientId,* FROM [dbo].[service]


select * from client

UPDATE [dbo].[service]
SET ClientId = 11
WHERE [SERVICEID] = 2888252

SELECT * FROM [dbo].[service]
WHERE [SERVICEID] = 2888252

SELECT * FROM [dbo].[Customer]
WHERE [CUSTOMERID] = 3545743
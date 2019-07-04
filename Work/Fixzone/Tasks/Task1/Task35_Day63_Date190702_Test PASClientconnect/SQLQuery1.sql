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

SELECT * FROM Client
ORDER BY ClientName

SELECT * FROM Client
WHERE ClientID = 11

SELECT * FROM Client
WHERE ClientName = '0800 Repair Chargeable Spares'

SELECT * FROM Client
WHERE [ClientName] lIKE '%Electrolux%'

SELECT * FROM [dbo].[Customer]

SELECT ClientId, * FROM [dbo].[service]

SELECT COUNT(*)
FROM [dbo].[UserWeb]

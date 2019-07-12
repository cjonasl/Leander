INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(32, 'CustAdvShr', 1)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(32, 1, 1, 73, 0, NULL)

INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(34, 'CstDetails', 1)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(34, 1, 1, 70, 0, NULL)

SELECT * FROM [dbo].[ProcessHeader]

UPDATE [dbo].[ProcessHeader]
SET [ProcessName] = 'CstDetails'
WHERE ProcessID = 33

UPDATE [dbo].[ProcessHeader]
SET [ProcessName] = 'Dummy'
WHERE ProcessID = 34

SELECT * FROM ProcessDetail
WHERE [ProcessID] IN(33, 34)

UPDATE ProcessDetail
SET ProcessID = 34
WHERE [ProcessDetailID] IN(1553, 1556, 1568)

UPDATE ProcessDetail
SET ProcessID = 33
WHERE [ProcessDetailID] = 1569
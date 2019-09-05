INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowJobSearch', 1, 'true', 39)

INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowCustomerSearch', 1, 'true', 39)

INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowJobStatuses', 1, 'true',39)

INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowProductSearch', 1, 'true', 0)


INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowJobSearch', 1, 'true', 0)






SELECT * FROM ProcessDetail
WHERE ClientId = 39

SELECT * FROM [dbo].[ProcessPage]
WHERE [PageID] = 2

SELECT ClientId FROM [service]

UPDATE [service]
SET ClientId = 39
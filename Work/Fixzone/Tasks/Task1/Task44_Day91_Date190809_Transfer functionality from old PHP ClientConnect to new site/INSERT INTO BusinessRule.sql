INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowProductSearch', 0, 'false', 39)

INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowCustomerSearch', 0, 'false', 0)

INSERT INTO BusinessRule([Key], [Checked], [Value], [Clientid])
VALUES('ShowJobStatuses', 0, 'false', 0)


SELECT * FROM ProcessDetail
WHERE ClientId = 39

SELECT * FROM [dbo].[ProcessPage]
WHERE [PageID] = 2

SELECT ClientId FROM [service]

UPDATE [service]
SET ClientId = 39
--Job/AdvSearch
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(5, 1, 1, 72, 0, 39)

--Job/JobDetails
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(5, 2, 1, 16, 0, 39)

--Account/ChangePassword
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(18, 1, 1, 38, 0, 39)

--JobStatus/JobStatusList
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(25, 1, 1, 67, 0, 39)

--Job/JobDetails
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(6, 1, 1, 16, 0, 39)

--Customer/Search
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(28, 1, 1, 69, 0, 39)

--Customer/AdvSearch
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(32, 1, 1, 73, 0, 39)


SELECT * FROM ProcessPage
SELECT * FROM ProcessHeader

SELECT * FROM [dbo].[ProcessDetail]
WHERE [ProcessID] = 32

SELECT * FROM ProcessPage
WHERE [PageID] = 73

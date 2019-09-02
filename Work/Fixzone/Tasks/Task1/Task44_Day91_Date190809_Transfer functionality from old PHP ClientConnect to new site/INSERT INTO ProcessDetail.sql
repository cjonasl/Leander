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

--Administration/UserList
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(11, 1, 1, 30, 0, 39)

--Administration/AccountInfo
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(16, 1, 1, 36, 0, 39)

--Administration/EditUser
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(16, 2, 1, 39, 0, 39)

--Administration/AddColleague
INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(17, 1, 1, 37, 0, 39)


SELECT * FROM ProcessPage
SELECT * FROM ProcessHeader

SELECT * FROM [dbo].[ProcessDetail]
WHERE [ProcessID] = 18

SELECT * FROM ProcessPage
WHERE [PageID] = 38

SELECT * FROM [dbo].[ProcessDetail]
WHERE ClientId = 39

--Ran successfully on prod database (Use SSMS on "Terminal server": user name: PAS_CC, password: QchK8HZEn4g7rU9u and database name is PAS) 2019-08-07 12:25
INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(32, 'CustAdvShr', 1)

INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(33, 'CstDetails', 0)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(32, 1, 1, 73, 0, NULL)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(33, 1, 1, 70, 0, NULL)

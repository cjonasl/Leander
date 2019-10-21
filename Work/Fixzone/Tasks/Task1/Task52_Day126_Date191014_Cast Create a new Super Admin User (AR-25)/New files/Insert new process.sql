INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(68, '/Client/ClientSearch', 'Client search page')

INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(25, 'ClientSrch', 1)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID)
VALUES(25, 1, 1, 68)
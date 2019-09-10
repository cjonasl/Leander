INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(78, '/CustProd/ApplianceDetails', 'Customer search appliance details')

INSERT INTO ProcessHeader(ProcessID, ProcessName, ProcessAccessLevel)
VALUES(38, 'Appliance details', 1)

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID, CustomerProcess, ClientId)
VALUES(38, 1, 1, 78, 0, 39)
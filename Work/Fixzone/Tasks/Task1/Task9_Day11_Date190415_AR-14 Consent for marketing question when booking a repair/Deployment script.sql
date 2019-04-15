--AR-14
DECLARE
@PageID int

SELECT @PageID = MAX(PageID)
FROM ProcessPage

SET @PageID = 1 + @PageID

INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(@PageID, '/Question/ConfirmYes', 'Consent for marketing question when booking a repair')

UPDATE ProcessDetail
SET ProcessStep = ProcessStep + 1
WHERE ProcessID = 4

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID)
VALUES(4, 1, 1, @PageID)
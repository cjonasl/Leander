DECLARE
@PageID int

SELECT @PageID = MAX(PageID)
FROM ProcessPage

SET @PageID = 1 + @PageID

INSERT INTO ProcessPage(PageID, PageURL, PageDescription)
VALUES(@PageID, '/User/CheckDetails', 'Check that ReminderQuestion, ReminderAnswer and DateOfBirth are set')

INSERT INTO ProcessDetail(ProcessID, ProcessStep, ExecuteLinkType, ExecuteLinkID)
VALUES(18, 2, 1, @PageID)
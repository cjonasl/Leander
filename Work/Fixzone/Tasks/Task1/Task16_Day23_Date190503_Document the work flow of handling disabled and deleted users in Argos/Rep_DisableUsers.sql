CREATE PROCEDURE DisableUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-7, to disable users if they been inactive
  for more than 90 days.
*/
AS
DECLARE
@DateToday date

SET @DateToday = getdate()

UPDATE
  [dbo].[UserWeb]
SET
  [Enabled] = 0,
  DisabledDate = @DateToday
WHERE
  DisabledDate IS NULL AND
  (Lastacdt IS NULL OR datediff(day, Lastacdt, @DateToday) > 90)
GO
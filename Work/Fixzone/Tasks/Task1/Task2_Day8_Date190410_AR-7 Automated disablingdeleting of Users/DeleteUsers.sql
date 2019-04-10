CREATE PROCEDURE DeleteUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-7, to delete users in table UserWeb and move them
  to table DeletedUserWebRecordsSinceLastReport if they have
  been disabled for more than 30 days.
*/
AS
DECLARE
@DateToday date

SET @DateToday = getdate()

INSERT INTO DeletedUserWebRecordsSinceLastReport
(
  Userid,
  Fullname,
  Store,
  LastLoggedInDate,
  DisabledDate,
  DeletedDate
)
SELECT
  u.Userid,
  u.Fullname,
  c.ClientName,
  u.Lastacdt,
  u.DisabledDate,
  @DateToday AS 'DeletedDate'
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
   u.DisabledDate IS NOT NULL AND
   datediff(day, u.DisabledDate, @DateToday) >= 30

DELETE FROM UserWeb
WHERE Userid IN(SELECT Userid FROM DeletedUserWebRecordsSinceLastReport)
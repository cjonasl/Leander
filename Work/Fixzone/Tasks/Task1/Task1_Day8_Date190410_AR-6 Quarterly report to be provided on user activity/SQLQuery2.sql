CREATE PROCEDURE GenerateReportDisabledDeletedUsers
/*
  Author: Jonas Leander
  Date: 5/4/2019
  Description:
*/
AS
SELECT
  u.Userid,
  u.Fullname,
  c.ClientName AS 'Store',
  u.Lastacdt AS 'LastLoggedInDate',
  u.DisabledDate,
  NULL AS 'DeletedDate'
FROM
  UserWeb u
  INNER JOIN Client c ON u.ClientID = c.ClientID
WHERE
  u.[Enabled] = 0
UNION
SELECT
  Userid,
  Fullname,
  Store,
  LastLoggedInDate,
  DisabledDate,
  DeletedDate
FROM
  DeletedUserWebRecordsSinceLastReport
ORDER BY
  DeletedDate, Fullname
GO



CREATE PROCEDURE DeleteTmpStoredDeletedUsers
/*
  Author: Jonas Leander

  Date: 5/4/2019

  Description: 
  Jira AR-6, to delete all rows in table DeletedUserWebRecordsSinceLastReport
  after a report has been successfully generated by stored procedure
  GenerateReportDisabledDeletedUsers.
*/
AS
TRUNCATE TABLE DeletedUserWebRecordsSinceLastReport
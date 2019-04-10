CREATE PROCEDURE DisableUser
/*
  Author: Jonas Leander
  Date: 5/4/2019
  Description: Disable a user (Jira A-12)
*/
@Userid varchar(25)
AS
UPDATE
  UserWeb
SET
  [Enabled] = 0,
  DisabledDate = getdate()
WHERE
  Userid = @Userid
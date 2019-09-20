CREATE TRIGGER [dbo].[dfhdsfhshf] ON [dbo].[UserWeb] AFTER UPDATE
AS
BEGIN
  INSERT INTO AnalyzeLogIn2(DeleteOrUpdate, _id, Userid, LogInDate, [Enabled], NumberOfLogInFailures)
  SELECT 'Delete', _id, Userid, getdate(), [Enabled], NumberOfLogInFailures
  FROM deleted 

  INSERT INTO AnalyzeLogIn2(DeleteOrUpdate, _id, Userid, LogInDate, [Enabled], NumberOfLogInFailures)
  SELECT 'Insert', _id, Userid, getdate(), [Enabled], NumberOfLogInFailures
  FROM inserted 
END
GO
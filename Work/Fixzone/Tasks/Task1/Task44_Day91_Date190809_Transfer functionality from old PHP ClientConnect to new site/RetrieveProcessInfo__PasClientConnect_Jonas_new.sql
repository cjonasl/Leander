ALTER PROCEDURE [dbo].[RetrieveProcessInfo]
AS	
SELECT
  pd.ProcessID AS 'ProcessId',
  CAST(pd.ProcessStep as INT) AS 'Step',
  ph.ProcessAccessLevel AS 'AccessLevel',		
  CASE
    WHEN pd.ExecuteLinkType = 0 THEN pd.ExecuteLinkID
    ELSE NULL
  END AS 'ChildProcessId',
  pd.CustomerProcess,
  pp.PageURL AS 'PageURL',
  pd.ClientId
FROM
  ProcessDetail pd
  INNER JOIN ProcessHeader ph ON pd.ProcessID = ph.ProcessID
  INNER JOIN ProcessPage pp ON pd.ExecuteLinkID = pp.PageID AND pd.ExecuteLinkType = 1
CREATE PROCEDURE [dbo].[Report_GetJobsCountByStatus]
@ClientId int
AS
BEGIN
SELECT
  sv.STATUSID AS 'StatusId',
  st.[Status],
  CASE WHEN st.StatusWIP = 1 THEN 'Yes' ELSE 'No' END AS 'Work in progress',
  COUNT(*) AS 'Count'
FROM
  [service] sv
  INNER JOIN [status] st ON sv.STATUSID = st.StatusID
WHERE
  sv.CLIENTID = @ClientId
GROUP BY
  sv.STATUSID, st.[Status], CASE WHEN st.StatusWIP = 1 THEN 'Yes' ELSE 'No' END 
ORDER BY
  sv.STATUSID

OPTION(RECOMPILE)
END
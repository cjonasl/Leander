CREATE PROCEDURE usp_JobsByStatus
@ClientId int
AS
BEGIN
SELECT st.[Status], COUNT(*) AS 'Count'
  FROM [dbo].[service] se INNER JOIN [dbo].[status] st ON se.STATUSID = st.StatusID
  WHERE se.CLIENTID = @ClientId
  GROUP BY st.[Status]
  ORDER BY st.[Status]
END
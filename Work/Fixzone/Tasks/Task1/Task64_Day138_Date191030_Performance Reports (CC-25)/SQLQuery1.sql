SELECT st.[Status], COUNT(*) AS 'Count'
FROM [dbo].[service] se INNER JOIN [dbo].[status] st ON se.STATUSID = st.StatusID
WHERE se.CLIENTID = 818
GROUP BY st.[Status]
ORDER BY st.[Status]
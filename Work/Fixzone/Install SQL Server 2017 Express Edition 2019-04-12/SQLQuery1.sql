SELECT
  e.FirstName,
  e.LastName,
  d.[Name]
INTO #AAA
FROM
  Employee e
  INNER JOIN Department d ON e.DepartmentID = d.ID



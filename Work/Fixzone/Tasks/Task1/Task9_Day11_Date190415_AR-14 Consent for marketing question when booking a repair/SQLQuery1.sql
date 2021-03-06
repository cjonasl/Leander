SELECT
  pd.ProcessStep,
  pp.PageURL,
  pp.PageDescription
FROM
  ProcessDetail pd
  INNER JOIN ProcessPage pp ON pd.ExecuteLinkID = pp.PageID
WHERE
  pd.ProcessID = 4
ORDER BY
  pd.ProcessStep
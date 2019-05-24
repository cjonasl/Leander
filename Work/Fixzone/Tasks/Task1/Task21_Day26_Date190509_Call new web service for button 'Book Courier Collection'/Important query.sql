SELECT
  P.[ReturnReference],
  R.RmaId,
  P.CodeID,
  P.[SAEDIFromID],
  P.[SAEDICallRef]
FROM
  [dbo].[SAEDIParts] P
  INNER JOIN [dbo].[SONYRMA] R ON P.SAEDIFromID = R.SaediFromID AND P.SAEDICallRef = R.ClientRef AND
  R.INPUT_partNumberReceived  = P.CodeID  AND (R.INPUT_externalMatId = CAST(P.SAEDICallRef AS VARCHAR(20)) + CAST(P.OrderNumber AS VARCHAR(20)) OR  R.RmaId = P.ReturnReference)
WHERE P.[ReturnReference] IS NOT NULL AND P.[ReturnReference] <> ''

--E2F7KNM8	

SELECT * FROM [dbo].[SONYRMA]
WHERE INPUT_partNumberReceived = '181135021'
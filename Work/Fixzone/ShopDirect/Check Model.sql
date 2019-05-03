SELECT APPLIANCECD, MFR, MODEL, COUNT(*)
FROM Model
GROUP BY APPLIANCECD, MFR, MODEL
HAVING COUNT(*) > 1

SELECT cap.CUSTAPLID, cap.APPLIANCECD, cap.MFR, cap.MODEL
FROM Custapl cap
WHERE
dbo.fnFilter_WithinDateRange(cap.CONTRACTDT, '2018-01-29', getdate()) = 1
AND dbo.fnFilter_NotContractStatus(cap.CONTRACTSTATUS, 60) = 1
AND EXISTS
(
  SELECT 1
  FROM Model mdl
  WHERE (cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL)
  GROUP BY mdl.APPLIANCECD, mdl.MFR, mdl.MODEL
  HAVING COUNT(*) > 1
)

SELECT * FROM
Custapl cap
LEFT JOIN Model mdl ON cap.APPLIANCECD = mdl.APPLIANCECD AND cap.MFR = mdl.MFR AND cap.MODEL = mdl.MODEL
WHERE cap.CUSTAPLID = 4181392
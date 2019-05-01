SELECT
  l.[GroupCode] + '_' + l.[LetterCode] + '_' + l.[LetterType] AS Msg,
  l.[TriggerId],
  l.[GroupCode],
  l.[LetterCode],
  l.[LetterType],
  l.[LetterName],
  l.[LetterDesc],
  l.[LetterBody],
  t.[TRIGGERNAME],
  t.[TRIGGERSQL],
  t.[TRIGGERFIELDNAME]
FROM
  Letter l
  INNER JOIN Triggers t ON l.TriggerId = t.Triggerid
WHERE
  l.[TriggerId] NOT IN(1, 2, 3, 4, 5, 9, 10, 11, 12, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24, 25, 26, 29, 31, 33, 34, 35, 37, 39, 40, 41, 42, 43, 45, 46, 47, 48, 50, 51, 52, 54, 56, 57, 58, 59, 60, 61,  62, 63) AND
  l.[TriggerId] NOT IN(65, 66) --Not used according to Angela in mail 2019-04-26
ORDER BY
  l.[TriggerId]
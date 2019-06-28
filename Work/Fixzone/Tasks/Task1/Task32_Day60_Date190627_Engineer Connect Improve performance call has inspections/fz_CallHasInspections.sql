--Installed this stored procedure on live database 2019-06-27 kl. 11:38
CREATE PROCEDURE fz_CallHasInspections
@FromId nvarchar(22),
@ToId nvarchar(22),
@ClientRef nvarchar(22)
AS

IF EXISTS
(
  SELECT
    1
  FROM
    Inspection
  WHERE
   clientRef = @clientRef AND
   ((fromId = @FromId AND toId = @ToId) OR (fromId = @ToId AND toId = @FromId))
)
  return 1
ELSE
  return 0
--Installed this stored procedure on live database 2019-06-27 kl. 16:50
CREATE PROCEDURE fz_GetInspections
@FromId nvarchar(22),
@ToId nvarchar(22),
@ClientRef nvarchar(22),
@Reference int = NULL
AS
IF (@Reference IS NULL)
BEGIN
  SELECT
    _id,
    reference,
    inspectionId,
    fromId,
	toId,
    clientRef,
    name,
	[status],
	[sent],
    DateInserted
  FROM
    Inspection
  WHERE
    clientRef = @clientRef AND
    ((fromId = @FromId AND toId = @ToId) OR (fromId = @ToId AND toId = @FromId))
   ORDER BY 
    reference
END
ELSE
BEGIN
    SELECT
    _id,
    reference,
    inspectionId,
    fromId,
	toId,
    clientRef,
    name,
	[status],
	[sent],
    DateInserted
  FROM
    Inspection
  WHERE
    fromId = @fromId AND
	toId = @toId AND
	clientRef = @clientRef AND
	reference = @Reference
END
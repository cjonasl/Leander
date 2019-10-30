ALTER PROCEDURE ProductNumberCodeIsMandatory
@Model varchar(25)
AS
BEGIN
DECLARE
@Mandatory bit

IF EXISTS
(
  SELECT
    1
  FROM
    Model mdl
	INNER JOIN PNCMandatory pnc ON mdl.MFR = pnc.MFR
   WHERE
     mdl.MODEL = @Model AND
	 pnc.Active = 1
)
  SET @Mandatory = 1
ELSE
  SET @Mandatory = 0

SELECT @Mandatory
END
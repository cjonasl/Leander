CREATE PROCEDURE [dbo].[ProductNumberCodeIsMandatory]
@ModelId int
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
     mdl.MODELID = @ModelId AND
	 pnc.Active = 1
)
  SET @Mandatory = 1
ELSE
  SET @Mandatory = 0

SELECT @Mandatory
END
GO
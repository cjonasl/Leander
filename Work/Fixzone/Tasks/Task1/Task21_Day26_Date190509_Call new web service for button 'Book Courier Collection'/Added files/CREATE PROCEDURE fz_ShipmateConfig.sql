CREATE PROCEDURE fz_ShipmateConfig
@SAEDIID varchar(11),
@IsGet bit,
@ShipmateConfig varchar(500) = NULL
AS
DECLARE 
@ID int = NULL,
@Value varchar(500)

SELECT @ID = _id
FROM SAEDIClient
WHERE (SAEDIID = @SAEDIID AND ClientType = 'Transfer')

IF (@ID IS NOT NULL AND @IsGet = 1)
BEGIN
  SELECT @Value = ISNULL(ShipmateConfig, '')
  FROM SAEDIClient
  WHERE _id = @ID

  IF (@Value = '')
    SELECT 'Error in fz_ShipmateConfig! Shipmate is not configured for client ' + @SAEDIID
  ELSE
    SELECT @Value
END
ELSE IF (@ID IS NOT NULL AND @IsGet = 0)
BEGIN
  UPDATE SAEDIClient
  SET ShipmateConfig = @ShipmateConfig
  WHERE _id = @ID

  SELECT 'Success'
END
ELSE
  SELECT 'Error in fz_ShipmateConfig! The client ' + @SAEDIID + ' does not exist in table SAEDIClient as expected.'

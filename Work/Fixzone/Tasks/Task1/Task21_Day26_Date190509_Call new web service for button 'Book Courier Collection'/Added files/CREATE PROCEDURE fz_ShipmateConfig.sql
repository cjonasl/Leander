CREATE PROCEDURE fz_ShipmateConfig
@SAEDIID varchar(11),
@IsGet bit,
@ShipmateConfig varchar(1000) = NULL
AS
DECLARE 
@ID int = NULL,
@Value varchar(500)

SELECT @ID = _id
FROM SAEDIClient
WHERE SAEDIID = @SAEDIID AND ClientType = 'Transfer'

IF (@ID IS NOT NULL AND @IsGet = 1)
BEGIN
  SELECT ISNULL(ShipmateConfig, '')
  FROM SAEDIClient
  WHERE _id = @ID
END
ELSE IF (@ID IS NOT NULL AND @IsGet = 0)
BEGIN
  UPDATE SAEDIClient
  SET ShipmateConfig = @ShipmateConfig
  WHERE _id = @ID

  SELECT 'Success'
END
ELSE
  SELECT 'ClientId does not exist!'

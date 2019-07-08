CREATE PROCEDURE fz_ShipmateConfig
@SAEDIID varchar(11) = NULL,
@ShipmateConfig varchar(1000) = NULL,
@ResTrackingReference varchar(100) = NULL
AS
DECLARE 
@ID int = NULL

IF (@SAEDIID IS NULL)
BEGIN
  SELECT TOP 1
    ISNULL(bbb.ShipmateConfig, '')
  FROM
    ShipmateConsignmentCreation aaa
    INNER JOIN SAEDIClient bbb ON aaa.ClientId = bbb.SAEDIID COLLATE Database_Default
  WHERE
    aaa.ResTrackingReference = @ResTrackingReference COLLATE Database_Default
END
ELSE
BEGIN
  SELECT @ID = _id
  FROM SAEDIClient
  WHERE SAEDIID = @SAEDIID COLLATE Database_Default AND ClientType = 'Transfer'

  IF (@ID IS NULL)
  BEGIN
    SELECT 'ClientId does not exist!'
  END
  ELSE IF (@ShipmateConfig IS NOT NULL)
  BEGIN
    UPDATE SAEDIClient
    SET ShipmateConfig = @ShipmateConfig
    WHERE _id = @ID

    SELECT 'Success'
  END
  ELSE
  BEGIN
    SELECT ISNULL(ShipmateConfig, '')
    FROM SAEDIClient
    WHERE _id = @ID
  END
END
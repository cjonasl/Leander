ALTER PROCEDURE [dbo].[fz_GetPartsRMADetails] 
@SAEDIFromID varchar(15),
@SON  varchar(20)
AS
BEGIN
  SELECT
    ISNULL(D.rmaId, R.RmaId) AS rmaId,
	ISNULL(D.rmaDocumentUrl, R.RmaDocumentUrl) AS rmaDocumentUrl,
	R.input_Sonnumber,
	R.input_partnumber,
	R.clientref,
    case ISNULL(D.shipmentStatus, R.ShipmentStatus) when 'NOSHIPMENTNEEDED' then '' else 'Return Required.' end as shipmentStatus,
    R.Collectionref,
    R.CollectionDate,
    R.CollectionAddedOn,
    SCC.ResMediaURL AS ShipmateMediaURL
FROM
  sonyRma R
  LEFT JOIN SonySearchReturnData D on D.son = R.INPUT_sonNumber AND R.Success <> '1'
  LEFT JOIN ShipmateConsignmentCreation SCC ON R.ShipmateConsignmentCreationId = SCC.ID
WHERE
   SaediFromID = @SAEDIFromID AND
   R.INPUT_sonNumber = @SON AND
   R.Success = '1' and
   @SON <> ''
END
GO
USE [Saedi_PRD]
GO

/****** Object:  StoredProcedure [dbo].[fz_GetPartsRMADetails]    Script Date: 06/14/2019 19:19:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[fz_GetPartsRMADetails] 
@SAEDIFromID varchar(15),
@SON  varchar(20)
AS
BEGIN
  SELECT
    ISNULL(D.rmaId, R.RmaId) AS rmaId,
	ISNULL(D.rmaDocumentUrl, R.RmaDocumentUrl) AS rmaDocumentUrl,
	input_Sonnumber, input_partnumber,clientref,
   case ISNULL(D.shipmentStatus, R.ShipmentStatus) when 'NOSHIPMENTNEEDED' then '' else 'Return Required.' end as shipmentStatus,
   R.Collectionref,
   R.CollectionDate,
   R.CollectionAddedOn,
   SCC.ResMediaURL AS ShipmateMediaURL
FROM
  sonyRma R
  LEFT JOIN SonySearchReturnData D on D.son = R.INPUT_sonNumber and R.Success<>'1'
  LEFT JOIN ShipmateConsignmentCreation SCC ON R.ShipmateConsignmentCreationId = SCC.ID
WHERE
   SaediFromID = @SAEDIFromID and
   R.INPUT_sonNumber = @SON and
   R.Success='1' and
   @SON<>''
END
GO
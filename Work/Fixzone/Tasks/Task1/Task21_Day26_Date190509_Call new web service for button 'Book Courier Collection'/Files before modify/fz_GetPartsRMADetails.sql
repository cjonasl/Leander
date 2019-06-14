USE [Saedi_PRD]
GO

/****** Object:  StoredProcedure [dbo].[fz_GetPartsRMADetails]    Script Date: 06/14/2019 19:19:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[fz_GetPartsRMADetails] 

@SAEDIFromID varchar(15),
@SON  varchar(20)

as
 begin
 
 select ISNULL(D.rmaId,R.RmaId)as rmaId,ISNULL(D.rmaDocumentUrl,R.RmaDocumentUrl) as rmaDocumentUrl ,input_Sonnumber,input_partnumber,clientref,
 case  ISNULL(D.shipmentStatus,R.ShipmentStatus) when 'NOSHIPMENTNEEDED' then '' else 'Return Required.' end as shipmentStatus, Collectionref, CollectionDate ,CollectionAddedOn
  from sonyRma R left join SonySearchReturnData 
   D on D.son=R.INPUT_sonNumber and R.Success<>'1'
 
 where SaediFromID=@SAEDIFromID  and R.INPUT_sonNumber=@SON   and R.Success='1' and @SON<>''
 
 end
GO
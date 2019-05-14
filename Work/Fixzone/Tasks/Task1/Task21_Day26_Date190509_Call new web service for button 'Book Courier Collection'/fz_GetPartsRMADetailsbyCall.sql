USE [SAEDI_PRD]
GO

/****** Object:  StoredProcedure [dbo].[fz_GetPartsRMADetailsbyCall]    Script Date: 13/05/2019 16:45:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER  proc [dbo].[fz_GetPartsRMADetailsbyCall]

@clientref varchar(12),
@SAEDIFromID varchar(12)

as
 begin
  
select SaediFromID, clientref,input_Sonnumber,input_partnumber,ISNULL(D.rmaId,R.RmaId)as rmaId,ISNULL(D.rmaDocumentUrl,R.RmaDocumentUrl) as rmaDocumentUrl , ISNULL(D.shipmentStatus,R.ShipmentStatus) as shipmentStatus, Collectionref, CollectionDate ,CollectionAddedOn
  from sonyRma R left join SonySearchReturnData 
   D on cast(D.son AS varchar(20))=R.INPUT_sonNumber and R.Success<>'1'
 
 where SaediFromID=@SAEDIFromID  and R.Clientref=@clientref   and R.Success='1'  
 
 end

GO


exec dbo.fz_GetPartsRMADetailsbyCall '1076', '3CCH23DE'
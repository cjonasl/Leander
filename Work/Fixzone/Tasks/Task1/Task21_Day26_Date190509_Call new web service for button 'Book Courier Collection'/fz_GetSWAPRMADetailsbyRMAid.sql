USE [SAEDI_PRD]
GO

/****** Object:  StoredProcedure [dbo].[fz_GetSWAPRMADetailsbyRMAid]    Script Date: 10/05/2019 17:01:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER  proc [dbo].[fz_GetSWAPRMADetailsbyRMAid] 

@RMAID varchar(15),
@SAEDIFromID  varchar(20)=''

as
 begin
 
 select R.SaediFromID,clientref,RmaId,R.RmaDocumentUrl , R.ShipmentStatus shipmentStatus, Collectionref, CollectionDate ,CollectionAddedOn
  from SONYSwapRMA R
 where-- SaediFromID=@SAEDIFromID  and 
 R.RmaId=@RMAID   and R.Success='1' 
 
 end

GO



USE [Saedi_PRD]
GO

/****** Object:  StoredProcedure [dbo].[UpdateCollectionjob]    Script Date: 06/14/2019 15:49:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER  proc [dbo].[UpdateCollectionjob]

@RMA varchar(12),
@jobref nvarchar(50),
@collectiondate varchar(20),
@SwapclaimCollection bit = 0,
@ShipmateConsignmentCreationId int = NULL
as
 begin
  if(@SwapclaimCollection=0)
 
  begin
    update
	  sonyrma
	 set
	   CollectionAddedOn = GETDATE(),
	   CollectionDate = @collectiondate,
	   Collectionref= @jobref,
	   ShipmateConsignmentCreationId = @ShipmateConsignmentCreationId
	 where
	   RmaId=@RMA 
  end
  else

  begin
	update SONYSwapRMA set CollectionAddedOn=GETDATE(), CollectionDate = @collectiondate  , Collectionref= @jobref   where RmaId=@RMA 
  end
 
 end
GO
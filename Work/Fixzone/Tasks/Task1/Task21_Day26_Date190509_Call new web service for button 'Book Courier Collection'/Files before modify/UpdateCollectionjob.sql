USE [SAEDI_PRD]
GO

/****** Object:  StoredProcedure [dbo].[UpdateCollectionjob]    Script Date: 23/05/2019 15:25:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER  proc [dbo].[UpdateCollectionjob]

@RMA varchar(12),
@jobref varchar(12),
@collectiondate varchar(20),
@SwapclaimCollection bit =false
as
 begin
  if(@SwapclaimCollection=0)
 
  begin
  update sonyrma set CollectionAddedOn=GETDATE(), CollectionDate = @collectiondate  , Collectionref= @jobref   where RmaId=@RMA 
  end
  else

  begin
	update SONYSwapRMA set CollectionAddedOn=GETDATE(), CollectionDate = @collectiondate  , Collectionref= @jobref   where RmaId=@RMA 
  end
 
 end

GO



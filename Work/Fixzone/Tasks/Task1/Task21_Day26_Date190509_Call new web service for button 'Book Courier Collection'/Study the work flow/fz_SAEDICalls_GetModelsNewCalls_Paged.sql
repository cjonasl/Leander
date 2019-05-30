USE [SAEDI_PRD]
GO

/****** Object:  StoredProcedure [dbo].[fz_SAEDICalls_GetModelsNewCalls_Paged]    Script Date: 29/05/2019 10:48:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-----------------------------
-- Author: Peter Potocnik
-- Created: 02.05.2013
-----------------------------

-- TEST -----------------------------------------------------------------
-- exec [dbo].[fz_SAEDICalls_GetModelsNewCalls_Paged] 'djhenry1', 0, 10
-------------------------------------------------------------------------

ALTER PROCEDURE [dbo].[fz_SAEDICalls_GetModelsNewCalls_Paged] 
	@saediId  varchar(11),
	@startRowIndex int,
	@maxNumRows int	
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @MaxNoOfDays int
	
	


	SELECT @MaxNoOfDays = MaxNoOfDays 
	FROM [SAEDIClient] sc2 
	WITH (nolock) 
	WHERE sc2.SAEDIID=@saediId
		
	set @startRowIndex = @startRowIndex + 1 -- GridView pager start with row index "0"
	
	SELECT * from
	(SELECT
	ROW_NUMBER() OVER (ORDER By cClientRef, cCallDateTime Desc) as RowNum,   -- for paging Peter
    * FROM [dbo].[view_SAEDICalls_GetModels]
    
	WHERE-- cSAEDIToID = @saediId
(cSAEDIToID=@saediId  or cSAEDIToID in (select saediid  from saediclient  where subgroupid=@saediId))
	AND cSAEDIInstruction = 1 
	AND cLastTransfer IS NULL
	AND c2ClientRef IS NULL
	AND 
		(
			(cEventDate IS NOT NULL AND cEventDate >= DateAdd(DAY,- cnMaxNoOfDays, GETDATE()))
			OR
			(cCallDateTime IS NOT NULL AND cCallDateTime >= DateAdd(DAY,- cnMaxNoOfDays, GETDATE()))
		)
	
	) sSAEDICalls
	
	-- Paging
	WHERE RowNum >= @startRowIndex AND RowNum < (@startRowIndex + @maxNumRows)
END






GO



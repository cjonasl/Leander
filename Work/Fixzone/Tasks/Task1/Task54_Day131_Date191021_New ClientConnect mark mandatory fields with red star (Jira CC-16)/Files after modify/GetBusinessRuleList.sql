ALTER PROCEDURE [dbo].[GetBusinessRuleList]
@ClientId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (@ClientId = -1) --Callcenter
	BEGIN
	  SELECT [Key], [Checked], [Value]
	  FROM [dbo].[BusinessRule]
	  WHERE [Clientid] = -1
	END
	ELSE
	BEGIN
	  CREATE TABLE #TmpTableBusinessRuleList
	  (
	    [Key] nvarchar(50) NOT NULL,
        [Checked] bit NULL,
        [Value] nvarchar(50) NULL
	  )

	  INSERT INTO #TmpTableBusinessRuleList([Key])
	  SELECT DISTINCT [Key]
	  FROM [dbo].[BusinessRule]
	  WHERE [Clientid] = @ClientId OR [Clientid] = -1 OR [Clientid] = 0

	  UPDATE
	    #TmpTableBusinessRuleList
	  SET
	    [Checked] = 
	    (
	      SELECT TOP 1 tmp.[Checked]
		  FROM [dbo].[BusinessRule] tmp
		  WHERE (#TmpTableBusinessRuleList.[Key] = tmp.[Key] COLLATE Database_Default AND (tmp.[Clientid] = @ClientId OR tmp.[Clientid] = -1 OR tmp.[Clientid] = 0))
		  ORDER BY tmp.[Clientid] desc
	    ),
	    [Value] = 
	    (
	      SELECT TOP 1 tmp.[Value]
		  FROM [dbo].[BusinessRule] tmp
		  WHERE (#TmpTableBusinessRuleList.[Key] = tmp.[Key] COLLATE Database_Default AND (tmp.[Clientid] = @ClientId OR tmp.[Clientid] = -1 OR tmp.[Clientid] = 0))
		  ORDER BY tmp.[Clientid] desc
	    )

	  SELECT [Key], [Checked], [Value]
	  FROM #TmpTableBusinessRuleList
	END
END
GO



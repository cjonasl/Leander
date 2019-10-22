ALTER PROCEDURE [dbo].[GetBusinessRuleList]
@ClientId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Key],value,checked from businessrule  where clientid=@ClientId
END
GO
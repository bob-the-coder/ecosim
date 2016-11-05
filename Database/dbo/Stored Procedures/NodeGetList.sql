
CREATE PROCEDURE [dbo].[NodeGetList]
	-- Add the parameters for the stored procedure here
	@NodeId uniqueidentifier = NULL,
	@ProductId uniqueidentifier = NULL,
	@NeedId uniqueidentifier = NULL,
	@Error bit = 0 OUT,
	@ErrorCode int = 0 OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (@NodeId IS NULL AND @ProductId IS NULL AND @NeedId IS NULL)
	RETURN

	IF @NodeId IS NOT NULL
	BEGIN
		SELECT * FROM Nodes WHERE EXISTS 
		(SELECT * FROM NodeLinks WHERE NodeId = Nodes.Id AND LinkId = @NodeId
		 OR NodeId = @NodeId AND LinkId = Nodes.Id) ORDER BY Name;
		RETURN;
	END

	IF @ProductId IS NOT NULL
	BEGIN
		SELECT * FROM Nodes WHERE EXISTS 
		(SELECT * FROM Productions WHERE NodeId = Nodes.Id AND ProductId = @ProductId)
		ORDER BY Name;
		RETURN;
	END

	IF @NeedId IS NOT NULL
	SELECT * FROM Nodes WHERE EXISTS 
	(SELECT * FROM Needs WHERE NodeId = Nodes.Id AND ProductId = @NeedId)
	ORDER BY Name;
	RETURN
END
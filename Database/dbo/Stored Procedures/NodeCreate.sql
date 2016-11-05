
CREATE PROCEDURE [dbo].[NodeCreate]
	-- Add the parameters for the stored procedure here
	@Id UNIQUEIDENTIFIER = NULL,
	@Name nvarchar(200),
	@SpendingLimit float,
	@Error bit = 1 OUT,
	@ErrorCode int = 0 OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @Id IS NULL
	BEGIN
		SET @Error = 1
		SET @ErrorCode = 404
		RETURN
	END

	--IF (SELECT COUNT(*) FROM Nodes) > 0
	--BEGIN
	--	DECLARE @LinkId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nodes ORDER BY NEWID())
	--	DECLARE @LinkId2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nodes WHERE Id != @LinkId ORDER BY NEWID())
	--	DECLARE @LinkId3 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nodes WHERE Id != @LinkId 
	--										AND Id != @LinkId2	ORDER BY NEWID())
	--END

	INSERT INTO Nodes VALUES (@Id, (SELECT COUNT(*) FROM Nodes), @SpendingLimit)

	--IF (SELECT COUNT(*) FROM Nodes) > 1
	--	INSERT INTO NodeLinks VALUES (@Id, @LinkId)

	--IF (SELECT COUNT(*) FROM Nodes) > 2 AND @LinkId2 IS NOT NULL AND @LinkId2 > @LinkId
	--	INSERT INTO NodeLinks VALUES (@Id, @LinkId2)

	--IF (SELECT COUNT(*) FROM Nodes) > 3 AND @LinkId3 IS NOT NULL AND @LinkId3 > @LinkId2 AND @LinkId3 < @LinkId
	--	INSERT INTO NodeLinks VALUES (@Id, @LinkId3)
    -- Insert statements for procedure here
	SELECT * FROM Nodes WHERE Id = @Id
	SET @Error = 0
END
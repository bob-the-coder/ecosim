
CREATE PROCEDURE [dbo].[NodeLinkCreate]
	-- Add the parameters for the stored procedure here
	@NodeId UNIQUEIDENTIFIER,
	@LinkId UNIQUEIDENTIFIER,
	@Error bit = 1 OUT,
	@ErrorCode int = 0 OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @NodeId IS NULL OR @LinkId IS NULL
	BEGIN
		SET @Error = 1
		SET @ErrorCode = 404
		RETURN
	END

	INSERT INTO NodeLinks VALUES (@NodeId, @LinkId)

	SET @Error = 0
END
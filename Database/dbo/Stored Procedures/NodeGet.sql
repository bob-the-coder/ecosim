
CREATE PROCEDURE NodeGet
	-- Add the parameters for the stored procedure here
	@Id UNIQUEIDENTIFIER = NULL,
	@Error bit = 0 OUT,
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

    -- Insert statements for procedure here
	SELECT * FROM Nodes WHERE Id = @Id
END
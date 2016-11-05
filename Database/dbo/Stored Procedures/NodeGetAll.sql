
CREATE PROCEDURE [dbo].[NodeGetAll]
	-- Add the parameters for the stored procedure here
	@Error bit = 0 OUT,
	@ErrorCode int = 0 OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT * FROM Nodes ORDER BY Name;
END
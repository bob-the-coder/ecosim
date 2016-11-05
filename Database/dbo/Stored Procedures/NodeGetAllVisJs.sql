
CREATE PROCEDURE [dbo].[NodeGetAllVisJs]
	-- Add the parameters for the stored procedure here
	@Error bit = 0 OUT,
	@ErrorCode int = 0 OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		Id AS id, 
		Name AS label,
		(SELECT COUNT(*) FROM NodeLinks WHERE NodeId = Id OR LinkId = Id) AS 'group',
		(SELECT COUNT(*) FROM NodeLinks WHERE NodeId = Id OR LinkId = Id) AS 'value' 
	FROM Nodes
END
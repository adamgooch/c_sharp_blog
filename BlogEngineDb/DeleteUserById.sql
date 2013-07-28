CREATE PROCEDURE [dbo].[DeleteUserById]
	@Id uniqueidentifier
AS
	DELETE FROM [dbo].[Users]
	WHERE Id = @Id
RETURN 0

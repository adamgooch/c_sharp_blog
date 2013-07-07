CREATE PROCEDURE [dbo].[DeleteUserByEmail]
	@Email nvarchar(50)
AS
	DELETE FROM [dbo].[Users]
	WHERE Email = @Email
RETURN 0

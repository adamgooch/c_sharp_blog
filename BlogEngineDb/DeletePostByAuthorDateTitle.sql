CREATE PROCEDURE [dbo].[DeletePostByAuthorDateTitle]
	@Author nvarchar(50),
	@Date date,
	@Title nvarchar(50)
AS
	DELETE FROM [dbo].[Posts]
	WHERE Author = @Author
	AND CreatedDateTime = @Date
	AND Title = @Title
RETURN 0

CREATE PROCEDURE [dbo].[GetAllPostsByAuthor]
	@Author nvarchar(50)
AS
	SELECT *
	FROM [dbo].[Posts] AS Post
	WHERE Author = @Author

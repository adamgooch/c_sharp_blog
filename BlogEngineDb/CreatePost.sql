CREATE PROCEDURE [dbo].[CreatePost]
	@CreatedDateTime date,
	@Title nvarchar(max),
	@Body nvarchar(max),
	@Tags nvarchar(max),
	@Author nvarchar(max)
AS
	DECLARE @Id uniqueidentifier
	SET @Id = NEWID()
	INSERT INTO [dbo].[Posts] ( Id, CreatedDateTime, ModifiedDateTime, Title, Body, Tags, Author )
	VALUES ( @Id, @CreatedDateTime, @CreatedDateTime, @Title, @Body, @Tags, @Author )
	SELECT @Id AS Id

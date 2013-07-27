CREATE PROCEDURE [dbo].[CreateUser]
	@Email nvarchar(max),
	@Salt varbinary(max),
	@PasswordDigest varbinary(max),
	@CreatedDateTime datetime,
	@Role int,
	@VerifiedToken uniqueidentifier
AS
	DECLARE @Id uniqueidentifier
	SET @Id = NEWID()
	INSERT INTO [dbo].[Users] ( Id, CreatedDateTime, ModifiedDateTime, Email, Salt, PasswordDigest, UserRole, VerifiedToken )
	VALUES ( @Id, @CreatedDateTime, @CreatedDateTime, @Email, @Salt, @PasswordDigest, @Role, @VerifiedToken )
	SELECT @Id AS Id

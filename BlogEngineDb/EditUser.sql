CREATE PROCEDURE [dbo].[EditUser]
	@Id uniqueidentifier,
	@Email nvarchar(max),
	@Salt varbinary(max),
	@PasswordDigest varbinary(max),
	@ModifiedDateTime datetime,
	@Role int,
	@VerifiedToken uniqueidentifier
AS
	UPDATE [dbo].[Users]
		SET ModifiedDateTime = @ModifiedDateTime
		, Email = @Email
		, Salt = @Salt
		, PasswordDigest = @PasswordDigest
		, UserRole = @Role
		, VerifiedToken = @VerifiedToken
		WHERE Id = @Id
	SELECT @Id AS Id

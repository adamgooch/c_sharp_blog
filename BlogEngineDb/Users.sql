CREATE TABLE [dbo].[Users]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Email] NVARCHAR(50) NOT NULL, 
    [Salt] VARBINARY(50) NOT NULL, 
    [PasswordDigest] VARBINARY(MAX) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [Role] INT NOT NULL, 
    [VerifiedToken] UNIQUEIDENTIFIER NOT NULL
)

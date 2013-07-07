CREATE TABLE [dbo].[Users]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Email] NVARCHAR(50) NOT NULL, 
    [Salt] VARBINARY(50) NOT NULL, 
    [PasswordDigest] VARBINARY(MAX) NOT NULL, 
    [CreatedDateTime] DATETIME NOT NULL, 
    [UserRole] INT NOT NULL, 
    [VerifiedToken] UNIQUEIDENTIFIER NOT NULL, 
    [ModifiedDateTime] DATETIME NOT NULL
)

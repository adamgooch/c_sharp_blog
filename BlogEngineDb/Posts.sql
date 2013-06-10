CREATE TABLE [dbo].[Posts]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [CreatedDateTime] DATETIME NOT NULL, 
    [ModifiedDateTime] DATETIME NOT NULL, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Body] NVARCHAR(MAX) NOT NULL, 
    [Tags] NVARCHAR(50) NOT NULL, 
    [Author] NVARCHAR(50) NOT NULL
)

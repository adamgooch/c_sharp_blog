CREATE TABLE [dbo].[Roles](
	[id] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[created_date] [datetime] NOT NULL,
	[modified_date] [datetime] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
)

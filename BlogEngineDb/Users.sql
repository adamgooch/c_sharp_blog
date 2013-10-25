CREATE TABLE [dbo].[Users](
	[id] [uniqueidentifier] NOT NULL,
	[email] [nvarchar](254) NOT NULL,
	[pass_digest] [varbinary](256) NOT NULL,
	[salt] [varbinary](64) NOT NULL,
	[verify_token] [uniqueidentifier] NOT NULL,
	[active] [bit] NOT NULL,
	[created_date] [datetime] NOT NULL,
	[modified_date] [datetime] NOT NULL,
	[reset_pass_token] [varbinary](64) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
)
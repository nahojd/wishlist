CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NotifyOnChange] [bit] NOT NULL,
	[ApprovalTicket] [uniqueidentifier] NULL
)
GO

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_NotifyOnChange]  DEFAULT ((0)) FOR [NotifyOnChange]
GO

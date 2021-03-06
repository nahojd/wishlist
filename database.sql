/****** Object:  Table [dbo].[Wish]    Script Date: 2019-11-21 21:01:21 ******/
DROP TABLE IF EXISTS [dbo].[Wish]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2019-11-21 21:01:21 ******/
DROP TABLE IF EXISTS [dbo].[User]
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 2019-11-21 21:01:21 ******/
DROP TABLE IF EXISTS [dbo].[Friend]
GO
/****** Object:  Table [dbo].[Friend]    Script Date: 2019-11-21 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friend](
	[UserId] [int] NOT NULL,
	[FriendId] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2019-11-21 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ApprovalTicket] [uniqueidentifier] NULL,
	[NotifyOnChange] [bit] NOT NULL,
	[PasswordHash] [nvarchar](128) NULL,
	[Salt] [nvarchar](128) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wish]    Script Date: 2019-11-21 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wish](
	[WishId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[TjingedById] [int] NULL,
	[LinkUrl] [nvarchar](255) NULL,
	[Created] [datetime] NULL,
	[Changed] [datetime] NULL
) ON [PRIMARY]
GO

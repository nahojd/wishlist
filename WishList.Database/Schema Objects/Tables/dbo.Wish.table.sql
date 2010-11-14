CREATE TABLE [dbo].[Wish] (
    [WishId]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100)  NOT NULL,
    [Description] NVARCHAR (500) NOT NULL,
    [OwnerId]     INT            NOT NULL,
    [TjingedById] INT            NULL,
    [LinkUrl]     NVARCHAR (255) NULL,
	[Created]	  DATETIME		 NOT NULL,
	[Changed]     DATETIME	 	 NULL
);


CREATE TABLE [dbo].[Nodes] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (200)   NOT NULL,
    [SpendingLimit] FLOAT (53)       NULL,
    CONSTRAINT [PK_Nodes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


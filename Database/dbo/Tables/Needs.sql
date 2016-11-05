CREATE TABLE [dbo].[Needs] (
    [NodeId]    UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity]  INT              NOT NULL,
    [Priority]  INT              NOT NULL,
    CONSTRAINT [FK_NodeNeed_Node] FOREIGN KEY ([NodeId]) REFERENCES [dbo].[Nodes] ([Id]),
    CONSTRAINT [FK_NodeNeed_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
);


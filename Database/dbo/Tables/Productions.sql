CREATE TABLE [dbo].[Productions] (
    [NodeId]    UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity]  INT              NOT NULL,
    [Quality]   INT              NOT NULL,
    [Price]     FLOAT (53)       NOT NULL,
    CONSTRAINT [FK_NodeProduction_Node] FOREIGN KEY ([NodeId]) REFERENCES [dbo].[Nodes] ([Id]),
    CONSTRAINT [FK_NodeProduction_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
);


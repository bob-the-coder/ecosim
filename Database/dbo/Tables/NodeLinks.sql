CREATE TABLE [dbo].[NodeLinks] (
    [NodeId] UNIQUEIDENTIFIER NOT NULL,
    [LinkId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_NodeNeighbour_Neighbour] FOREIGN KEY ([LinkId]) REFERENCES [dbo].[Nodes] ([Id]),
    CONSTRAINT [FK_NodeNeighbour_Node] FOREIGN KEY ([NodeId]) REFERENCES [dbo].[Nodes] ([Id])
);


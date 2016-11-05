CREATE TABLE [dbo].[ProductIngredients] (
    [ProductId]    UNIQUEIDENTIFIER NOT NULL,
    [IngredientId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_ProductIngredient_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT [FK_ProductsIngredient_Ingredient] FOREIGN KEY ([IngredientId]) REFERENCES [dbo].[Products] ([Id])
);


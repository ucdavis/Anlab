CREATE TABLE [dbo].[SavedOrders] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]  NVARCHAR (450) NOT NULL,
    [OrderId] INT            NOT NULL,
    CONSTRAINT [PK_SavedOrders] PRIMARY KEY CLUSTERED ([Id] ASC)
);



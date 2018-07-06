CREATE TABLE [dbo].[SavedOrders]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserId] NVARCHAR(450) NOT NULL, 
    [OrderId] INT NOT NULL
)

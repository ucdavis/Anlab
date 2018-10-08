CREATE TABLE [dbo].[History] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [OrderId]        INT            NOT NULL,
    [ActionDateTime] DATETIME2 (7)  NOT NULL,
    [Action]         NVARCHAR (50)  NULL,
    [ActorId]        NVARCHAR (50)  NULL,
    [ActorName]      NVARCHAR (250) NULL,
    [Notes]          NVARCHAR (MAX) NULL,
    [JsonDetails]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_History_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders]([Id])
);



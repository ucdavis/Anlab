CREATE TABLE [dbo].[MailMessages] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Body]          NVARCHAR (MAX) NOT NULL,
    [CreatedAt]     DATETIME2 (7)  NOT NULL,
    [FailureReason] NVARCHAR (MAX) NULL,
    [OrderId]       INT            NULL,
    [SendTo]        NVARCHAR (MAX) NOT NULL,
    [Sent]          BIT            NULL,
    [SentAt]        DATETIME2 (7)  NULL,
    [Subject]       NVARCHAR (256) NOT NULL,
    [UserId]        NVARCHAR (450) NULL,
    CONSTRAINT [PK_MailMessages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MailMessages_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_MailMessages_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MailMessages_UserId]
    ON [dbo].[MailMessages]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MailMessages_OrderId]
    ON [dbo].[MailMessages]([OrderId] ASC);


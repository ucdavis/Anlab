CREATE TABLE [dbo].[Orders] (
    [Id]                            INT              IDENTITY (1, 1) NOT NULL,
    [AdditionalEmails]              NVARCHAR (MAX)   NULL,
    [ApprovedPaymentTransaction_Id] NVARCHAR (450)   NULL,
    [ClientId]                      NVARCHAR (16)    NULL,
    [Created]                       DATETIME2 (7)    NOT NULL,
    [CreatorId]                     NVARCHAR (450)   NOT NULL,
    [JsonDetails]                   NVARCHAR (MAX)   NULL,
    [KfsTrackingNumber]             NVARCHAR (20)   NULL,
    [LabId]                         NVARCHAR (16)    NULL,
    [Paid]                          BIT              NOT NULL,
    [PaymentType]                   NVARCHAR (50)   NULL,
    [Project]                       NVARCHAR (256)   NOT NULL,
    [RequestNum]                    NVARCHAR (50)   NULL,
    [ResultsFileIdentifier]         NVARCHAR (MAX)   NULL,
    [SavedTestDetails]              NVARCHAR (MAX)   NULL,
    [ShareIdentifier]               UNIQUEIDENTIFIER NOT NULL,
    [SignedEnvelopeId]              NVARCHAR (50)   NULL,
    [SlothTransactionId]            UNIQUEIDENTIFIER   NULL,
    [Status]                        NVARCHAR (50)   NULL,
    [Updated]                       DATETIME2 (7)    NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT ((0)),
    [ClientName] VARCHAR(512) NULL,
    [BackedupTestDetails] NVARCHAR(MAX) NULL,
    [DateFinalized] DATETIME2 NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_AspNetUsers_CreatorId] FOREIGN KEY ([CreatorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_PaymentEvents_ApprovedPaymentTransaction_Id] FOREIGN KEY ([ApprovedPaymentTransaction_Id]) REFERENCES [dbo].[PaymentEvents] ([Transaction_Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Orders_CreatorId]
    ON [dbo].[Orders]([CreatorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Orders_ApprovedPaymentTransaction_Id]
    ON [dbo].[Orders]([ApprovedPaymentTransaction_Id] ASC);


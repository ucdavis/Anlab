CREATE TABLE [dbo].[TestItems] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [AdditionalInfoPrompt] NVARCHAR (MAX) NULL,
    [Analysis]             NVARCHAR (512) NOT NULL,
    [Category]             NVARCHAR (64)  NOT NULL,
    [Group]                NVARCHAR (MAX) NOT NULL,
    [Notes]                NVARCHAR (MAX) NULL,
    [Public]               BIT            NOT NULL,
    CONSTRAINT [PK_TestItems] PRIMARY KEY CLUSTERED ([Id] ASC)
);


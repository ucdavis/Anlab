CREATE TABLE [dbo].[AnalysisMethods] (
    [Id]       INT            NOT NULL,
    [Category] NVARCHAR (MAX) NULL,
    [Content]  NVARCHAR (MAX) NULL,
    [Title]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AnalysisMethods] PRIMARY KEY CLUSTERED ([Id] ASC)
);


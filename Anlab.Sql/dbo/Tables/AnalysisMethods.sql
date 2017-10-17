CREATE TABLE [dbo].[AnalysisMethods] (
    [Id]       INT            NOT NULL,
    [Category] NVARCHAR (256) NULL,
    [Content]  NVARCHAR (MAX) NOT NULL,
    [Title]    NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_AnalysisMethods] PRIMARY KEY CLUSTERED ([Id] ASC)
);


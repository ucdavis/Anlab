CREATE TABLE [dbo].[SystemAlerts] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Created]     DATETIME      NOT NULL,
    [Updated]     DATETIME      NOT NULL,
    [IsActive]    BIT           CONSTRAINT [DF_SystemAlert_IsActive] DEFAULT ((0)) NOT NULL,
    [Content]     VARCHAR (MAX) NOT NULL,
    [Markdown]    VARCHAR (MAX) NOT NULL,
    [Danger]      BIT           CONSTRAINT [DF_SystemAlerts_Danger] DEFAULT ((0)) NOT NULL,
    [Description] VARCHAR (50)  NOT NULL
);


CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [Account]              NVARCHAR (50)      NULL,
    [ClientId]             NVARCHAR (16)      NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [FirstName]            NVARCHAR (50)      NULL,
    [LastName]             NVARCHAR (50)      NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [Name]                 NVARCHAR (256)     NOT NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [Phone]                NVARCHAR (256)     NULL,
    [PhoneNumber]          NVARCHAR (256)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [CompanyName]          NVARCHAR (1000)    NULL,
    [BillingContactName]   NVARCHAR (250)     NULL,
    [BillingContactAddress]NVARCHAR (2000)    NULL,
    [BillingContactPhone]  NVARCHAR (50)      NULL,
    [BillingContactEmail]  NVARCHAR (250)     NULL,
    [Created]              DATETIME           CONSTRAINT [DF_AspNetUsers_Created] DEFAULT (getdate()) NOT NULL,
    [Updated]              DATETIME           CONSTRAINT [DF_AspNetUsers_Updated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


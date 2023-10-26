IF NOT EXISTS (SELECT name FROM [sys].[tables] WHERE name = 'Change')
BEGIN

    SET ANSI_NULLS ON
    SET QUOTED_IDENTIFIER ON
    CREATE TABLE [dbo].[Change]
    (
        [Guid] [uniqueidentifier] NOT NULL,
        [ChangeWrapperGuid] [uniqueidentifier] NOT NULL,
        [ActionType] [nvarchar](500) NULL,
        [EntityName] [nvarchar](500) NULL,
        [OldData] [text] NULL,
        [NewData] [text] NULL,
        [TransactionDate] [datetime] NULL,
        CONSTRAINT [PK_Change]
            PRIMARY KEY CLUSTERED ([Guid] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
                  ALLOW_PAGE_LOCKS = ON
                 ) ON [PRIMARY]
    ) ON [PRIMARY]

END

IF NOT EXISTS
(
    SELECT name
    FROM [sys].[tables]
    WHERE name = 'ChangeWrapper'
)
BEGIN
    SET ANSI_NULLS ON
    SET QUOTED_IDENTIFIER ON
    CREATE TABLE [dbo].[ChangeWrapper]
    (
        [Guid] [uniqueidentifier] NOT NULL,
        [Username] [nvarchar](50) NULL,
        [IpAddress] [nvarchar](50) NULL,
        [TargetName] [nvarchar](500) NULL,
        [MethodName] [nvarchar](500) NULL,
        [TransactionDate] [datetime] NULL,
        CONSTRAINT [PK_ChangeWrapper]
            PRIMARY KEY CLUSTERED ([Guid] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
                  ALLOW_PAGE_LOCKS = ON
                 ) ON [PRIMARY]
    ) ON [PRIMARY]

END

IF NOT EXISTS
(
    SELECT name
    FROM [sys].[foreign_keys]
    WHERE name = 'FK_Change_ChangeWrapper'
)
BEGIN

    ALTER TABLE [dbo].[Change] WITH CHECK
    ADD CONSTRAINT [FK_Change_ChangeWrapper]
        FOREIGN KEY ([ChangeWrapperGuid])
        REFERENCES [dbo].[ChangeWrapper] ([Guid])

    ALTER TABLE [dbo].[Change] CHECK CONSTRAINT [FK_Change_ChangeWrapper]
END




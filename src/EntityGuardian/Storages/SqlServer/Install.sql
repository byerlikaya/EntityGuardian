DECLARE @SchemaLockResult INT;
EXEC @SchemaLockResult = sp_getapplock @Resource = '$(EntityGuardiaonSchemaName):SchemaLock', @LockMode = 'Exclusive'

IF NOT EXISTS (SELECT [schema_id] FROM [sys].[schemas] WHERE [name] = '$(EntityGuardiaonSchemaName)')
BEGIN
    EXEC (N'CREATE SCHEMA [$(EntityGuardiaonSchemaName)]');
    PRINT 'Created database schema [$(EntityGuardiaonSchemaName)]';
END

DECLARE @SCHEMA_ID int;
SELECT @SCHEMA_ID = [schema_id] FROM [sys].[schemas] WHERE [name] = '$(EntityGuardiaonSchemaName)';

IF NOT EXISTS (SELECT name FROM [sys].[tables] WHERE name = 'Change' AND [schema_id] = @SCHEMA_ID)
BEGIN

    SET ANSI_NULLS ON
    SET QUOTED_IDENTIFIER ON
    CREATE TABLE [$(EntityGuardiaonSchemaName)].[Change]
    (
        [Guid] [uniqueidentifier] NOT NULL,
        [ChangeWrapperGuid] [uniqueidentifier] NOT NULL,
        [Order] [int] NULL,
        [TransactionType] [nvarchar](500) NULL,
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
    WHERE name = 'ChangeWrapper' AND [schema_id] = @SCHEMA_ID
)
BEGIN
    SET ANSI_NULLS ON
    SET QUOTED_IDENTIFIER ON
    CREATE TABLE [$(EntityGuardiaonSchemaName)].[ChangeWrapper]
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

    ALTER TABLE [$(EntityGuardiaonSchemaName)].[Change] WITH CHECK
    ADD CONSTRAINT [FK_Change_ChangeWrapper]
        FOREIGN KEY ([ChangeWrapperGuid])
        REFERENCES [$(EntityGuardiaonSchemaName)].[ChangeWrapper] ([Guid])

    ALTER TABLE [$(EntityGuardiaonSchemaName)].[Change] CHECK CONSTRAINT [FK_Change_ChangeWrapper]
END




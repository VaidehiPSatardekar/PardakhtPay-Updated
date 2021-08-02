CREATE TABLE [ManualTransferSourceCardDetails] (
    [Id] int NOT NULL IDENTITY,
    [ManualTransferId] int NOT NULL,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NULL,
    [CardToCardAccountId] int NOT NULL,
    [AccountGuid] nvarchar(max) NULL,
    CONSTRAINT [PK_ManualTransferSourceCardDetails] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210201133408_ManualTransferAccountGuidLength', N'3.1.10');

GO


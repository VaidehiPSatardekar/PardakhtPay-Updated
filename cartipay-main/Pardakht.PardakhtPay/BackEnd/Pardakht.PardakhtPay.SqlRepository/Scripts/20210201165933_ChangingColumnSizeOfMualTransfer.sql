DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ManualTransfers]') AND [c].[name] = N'AccountGuid');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ManualTransfers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [ManualTransfers] ALTER COLUMN [AccountGuid] nvarchar(4000) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210201165933_ChangingColumnSizeOfMualTransfer', N'3.1.10');

GO


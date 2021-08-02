ALTER TABLE [OwnerBankLogins] ADD [LastPasswordChangeDate] datetime2 NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200122165032_20200122_1650_LastPasswordChangeDate', N'2.2.6-servicing-10079');

GO


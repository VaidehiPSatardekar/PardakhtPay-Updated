ALTER TABLE [Tenants] ADD [TenantType] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190130125042_TenantTypeAdded', N'2.1.4-rtm-31024');

GO


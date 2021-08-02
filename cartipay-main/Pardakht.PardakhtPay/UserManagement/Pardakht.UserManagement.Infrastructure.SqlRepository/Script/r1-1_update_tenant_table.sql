ALTER TABLE [Tenants] ADD [PlatformGuid] nvarchar(max) NULL;

GO

ALTER TABLE [Tenants] ADD [PlatformName] nvarchar(max) NULL;

GO

ALTER TABLE [Tenants] ADD [TenantName] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190129174435_UpdateTenantTable', N'2.1.4-rtm-31024');

GO


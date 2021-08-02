using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository
{
    public class ParadakhtUserManagementDbContextFactory : DbContextFactory, IDesignTimeDbContextFactory<ParadakhtUserManagementDbContext>
    {
        public ParadakhtUserManagementDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ParadakhtUserManagementDbContext>();
            var connectionString = GetConnectionStringforOperationalDb();
            builder.UseSqlServer(connectionString);
            return new ParadakhtUserManagementDbContext(builder.Options);
        }

        //public async Task CreateDbContextForTenant(string connectionString)
        //{
        //    var builder = new DbContextOptionsBuilder<AccountRepository>();
        //    builder.UseSqlServer(connectionString);
        //    var newDb = new AccountRepository(builder.Options, new TenantConfig());
        //    await newDb.Database.EnsureCreatedAsync();
        //    await newDb.Database.MigrateAsync();
        //}


    }
}

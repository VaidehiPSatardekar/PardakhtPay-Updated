using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account
{
    public class AccountRepositoryFactory : DbContextFactory, IDesignTimeDbContextFactory<AccountRepository>
    {
        public AccountRepository CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AccountRepository>();

            var connectionString = GetConnectionStringforAccountDb();

            builder.UseSqlServer(connectionString);

            return new AccountRepository(builder.Options, null);
        }

        //public async Task CreateDbContextForTenant(string connectionString)
        //{
        //    var builder = new DbContextOptionsBuilder<AccountRepository>();
        //    builder.UseSqlServer(connectionString);
        //    var newDb = new AccountRepository(builder.Options, new TenantConfig { ConnectionString = connectionString });
        //    await newDb.Database.MigrateAsync();
        //}
    }
}

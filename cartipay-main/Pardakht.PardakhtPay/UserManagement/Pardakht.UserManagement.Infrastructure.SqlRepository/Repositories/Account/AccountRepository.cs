using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account
{
      public class AccountRepository : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        // this is the current tenant context
        //private readonly TenantConfig _tenantConfig;
        private readonly IConnectionStringManager _connectionStringManager;

        // the connection string in tenantConfig will overwrite whatever is in options
        public AccountRepository(DbContextOptions<AccountRepository> options, 
            IConnectionStringManager connectionStringManager = null)
            : base(options)
        {
            //_tenantConfig = tenantConfig;
            _connectionStringManager = connectionStringManager;
        }
        public AccountRepository(string connectionString) : base(GetOptions(connectionString)) { }

        private static DbContextOptions GetOptions(string connectionString)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<AccountRepository>();
            dbOptionsBuilder.UseSqlServer(connectionString);
            dbOptionsBuilder.EnableSensitiveDataLogging();
            return dbOptionsBuilder.Options;
        }

        //public new DbSet<UserRole> UserRoles { get; set; }
        //public new DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // this should only be when running migrations from the console
            var factory = new AccountRepositoryFactory();
            optionsBuilder.UseSqlServer(factory.GetConnectionStringforAccountDb());
            base.OnConfiguring(optionsBuilder);
        }

       
    }
}

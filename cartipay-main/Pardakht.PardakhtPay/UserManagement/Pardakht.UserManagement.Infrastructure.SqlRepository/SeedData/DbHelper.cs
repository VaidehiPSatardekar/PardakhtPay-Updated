using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData

{
    public class DbHelper
    {
        public IConnectionStringManager ConnectionStringManager
        {
            get { return _connectionStringManager; }
        }

        private readonly IConnectionStringManager _connectionStringManager;

        internal string ConnectionString
        {
            get { return _connectionStringManager.MainConnectionString; }
        }

        public DbHelper(IConnectionStringManager connectionStringManager)
        {
            _connectionStringManager = connectionStringManager;
        }

        internal void CreateDb(DbContext context, bool deleteFirst)
        {
            if (deleteFirst)
            {
                context.Database.EnsureDeleted();
            }
            // this will create (if not already there) and run migrations on the db (we can't use EnsureCreated() as migrations will fail)
            context.Database.Migrate();
        }

        internal DbContextOptions<T> GetOptionsBuilder<T>() where T : DbContext
        {
            return GetOptionsBuilder<T>(ConnectionString);
        }

        internal DbContextOptions<T> GetOptionsBuilder<T>(string connectionString) where T : DbContext
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<T>();
            dbOptionsBuilder.UseSqlServer(connectionString);
            return dbOptionsBuilder.Options;
        }

      
    }
}

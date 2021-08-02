using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pardakht.PardakhtPay.SqlRepository
{
    public class PardakhtPayDbContextFactory : DbContextFactory, IDesignTimeDbContextFactory<PardakhtPayDbContext>
    {
        public PardakhtPayDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PardakhtPayDbContext>();
            var connectionString = GetConnectionString();

            builder.UseSqlServer(connectionString);

            return new PardakhtPayDbContext(connectionString);
        }
    }
}

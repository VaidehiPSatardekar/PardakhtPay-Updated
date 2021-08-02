using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.SqlRepository.SeedData
{
    public class SeedDataInvoker
    {
        private SeedDataSettings _SeedDataSettings = null;
        private ILogger<SeedDataInvoker> _Logger = null;
        private IConnectionStringManager _ConnectionStringManager;

        public SeedDataInvoker(IOptions<SeedDataSettings> seedDataSettings,
            IConnectionStringManager connectionStringManager,
            ILogger<SeedDataInvoker> logger)
        {
            _SeedDataSettings = seedDataSettings.Value;
            _ConnectionStringManager = connectionStringManager;
            _Logger = logger;
        }

        public void Seed()
        {
            _Logger.LogInformation($"Database seeding was started. Configuration : {JsonConvert.SerializeObject(_SeedDataSettings)}");

            if (_SeedDataSettings.Enabled)
            {
                ConfigureMainDb();
            }
            else
            {
                _Logger.LogInformation($"Log information is disable");
            }
        }

        private void ConfigureMainDb()
        {
            //var context = new PardakhtPayDbContext(_ConnectionStringManager.MainConnectionString);

            //if (_SeedDataSettings.DropAndRecreateMainDb)
            //{
            //    context.Database.EnsureDeleted();
            //}

            //context.Database.Migrate();
        }
    }
}

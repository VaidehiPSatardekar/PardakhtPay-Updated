using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account;
using Pardakht.UserManagement.Shared.Models.Configuration;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class SeedDataInvoker
    {
        private readonly SeedDataSettings _settings;
        private readonly IConfiguration _configuration;
        private IServiceProvider _services;
        private readonly ILogger<SeedDataInvoker> _logger;
        private readonly IConnectionStringManager _connectionStringManager;


        public SeedDataInvoker(IOptions<SeedDataSettings> settings,
            IServiceProvider services,
            ILogger<SeedDataInvoker> logger, IConfiguration configuration, IConnectionStringManager connectionStringManager)
        {
            _settings = settings.Value;
            _services = services;
            _logger = logger;
            _configuration = configuration;
            _connectionStringManager = connectionStringManager;
        }


        public async void Seed()
        {
            _logger.LogInformation($"data seeding enabled = {_settings.Enabled}");
            List<ISeedData> list = new List<ISeedData>();
            // TODO: could possibly use a strongly-typed sub-section in config file and use a factory to resolve
            if (_settings.Enabled)
            {
                _logger.LogInformation("starting data seeding");
                try
                {
                    var helper = new DbHelper(_connectionStringManager);
                    if (_settings.CreateDatabaseOnly)
                    {
                        ConfigureMainDb(helper);
                    }

                    list.Add(new PlatformSeedData(_services));
                    list.Add(new PermissionGroupSeedData(_services));
                    list.Add(new PermissionSeedData(_services));
                    list.Add(new RoleSeedData(_services));
                    list.Add(new RolePermissionSeedData(_services));


                    list.Add(new ApiKeySeedData(helper.GetOptionsBuilder<AccountRepository>(), helper.GetOptionsBuilder<ParadakhtUserManagementDbContext>(), _connectionStringManager));
                    list.Add(new PlatformUsersSeedData(helper.GetOptionsBuilder<AccountRepository>(), helper.GetOptionsBuilder<ParadakhtUserManagementDbContext>(), _connectionStringManager));


                    foreach (var item in list)
                    {
                        await item.Seed(_settings.ClearExistingData);
                    }

                    _logger.LogInformation("data seeding complete");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error occurred seeding data");
                }
            }
        }

        private void ConfigureMainDb(DbHelper helper)
        {
            // main tables
            var connectionString = "Server=" + _configuration.GetSection("ConnectionStringOperational")["Server"] +
                                   ";User=" + _configuration.GetSection("ConnectionStringOperational")["User"] +
                                   ";Password=" + _configuration.GetSection("ConnectionStringOperational")["Password"] +
                                   ";Database=" + _configuration.GetSection("ConnectionStringOperational")["Database"];
            connectionString += ";MultipleActiveResultSets=True";

            _logger.LogInformation("configuring main db");
            var appDbContext = new ParadakhtUserManagementDbContext(connectionString);
            helper.CreateDb(appDbContext, _settings.DropAndRecreateDb);


            connectionString = "Server=" + _configuration.GetSection("ConnectionStringAccount")["Server"] +
                               ";User=" + _configuration.GetSection("ConnectionStringAccount")["User"] +
                               ";Password=" + _configuration.GetSection("ConnectionStringAccount")["Password"] +
                               ";Database=" + _configuration.GetSection("ConnectionStringAccount")["Database"];
            connectionString += ";MultipleActiveResultSets=True";

            // add the identity tables
            var identityDbContext = new AccountRepository(connectionString);
            helper.CreateDb(identityDbContext, _settings.DropAndRecreateDb);
        }
    }
}

using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository
{
    public abstract class DbContextFactory
    {
        internal virtual string GetConnectionStringforAccountDb()
        {
            // this should only be called when running migrations from the console - use IConnectionStringManager instead
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            var connectionString = "Server=" + configuration.GetSection("ConnectionStringAccount")["Server"].ToString() +
                       ";User=" + configuration.GetSection("ConnectionStringAccount")["User"].ToString() +
                       ";Password=" + configuration.GetSection("ConnectionStringAccount")["Password"].ToString() +
                       ";Database=" + configuration.GetSection("ConnectionStringAccount")["Database"].ToString();

            return connectionString;
        }

        internal virtual string GetConnectionStringforOperationalDb()
        {
            // this should only be called when running migrations from the console - use IConnectionStringManager instead
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            var connectionString = "Server=" + configuration.GetSection("ConnectionStringOperational")["Server"].ToString() +
                                   ";User=" + configuration.GetSection("ConnectionStringOperational")["User"].ToString() +
                                   ";Password=" + configuration.GetSection("ConnectionStringOperational")["Password"].ToString() +
                                   ";Database=" + configuration.GetSection("ConnectionStringOperational")["Database"].ToString();

            return connectionString;
        }
    }
}

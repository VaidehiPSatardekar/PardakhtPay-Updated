using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Pardakht.PardakhtPay.SqlRepository
{
    public abstract class DbContextFactory
    {
        internal virtual string GetConnectionString()
        {
            // this should only be called when running migrations from the console - use IConnectionStringManager instead
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            var connectionString = "Server=" + configuration.GetSection("ConnectionString")["Server"].ToString() +
                       ";User=" + configuration.GetSection("ConnectionString")["User"].ToString() +
                       ";Password=" + configuration.GetSection("ConnectionString")["Password"].ToString() +
                       ";Database=" + configuration.GetSection("ConnectionString")["Database"].ToString();

            return connectionString;
        }
    }
}

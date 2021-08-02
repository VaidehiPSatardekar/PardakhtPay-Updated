using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.SqlRepository;

namespace Pardakht.PardakhtPay.BotAutoTransferService
{
    class Program
    {
        public static string ConnectionString = string.Empty;

        private static async Task Main(string[] args)
        {
            await Run(args);
        }

        private static async Task Run(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var builder = new HostBuilder()
            .UseEnvironment(environment)
            .ConfigureAppConfiguration((hostingContext, b) =>
            {
                var config = b.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables().Build();
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = "Server=" + context.Configuration.GetSection("ConnectionString")["Server"].ToString() +
                                   ";User=" + context.Configuration.GetSection("ConnectionString")["User"].ToString() +
                                   ";Password=" + context.Configuration.GetSection("ConnectionString")["Password"].ToString() +
                                   ";Database=" + context.Configuration.GetSection("ConnectionString")["Database"].ToString();

                ConnectionString = connectionString;

                services.AddDbContext<PardakhtPayDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                services.SetupConfigurations(context.Configuration);
                services.AddDependencyInjections(context.Configuration);
                services.AddCacheService();

                services.AddLogging(configure => configure.AddSerilog());
            });

            if (isService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        }
    }
}

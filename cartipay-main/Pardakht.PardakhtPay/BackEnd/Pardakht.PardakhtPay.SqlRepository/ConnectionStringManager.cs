using Microsoft.Extensions.Configuration;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;

namespace Pardakht.PardakhtPay.SqlRepository
{
    public class ConnectionStringManager : IConnectionStringManager
    {
        public string Database { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string User { get; set; }

        public string MainConnectionString
        {
            get
            {
                return $"Server={Server};User={User};Password={Password};Database={Database}";
            }
        }

        public ConnectionStringManager(IConfiguration config)
        {

            Server = config.GetSection("ConnectionString")["Server"].ToString();
            User = config.GetSection("ConnectionString")["User"].ToString();
            Password = config.GetSection("ConnectionString")["Password"].ToString();
            Database = config.GetSection("ConnectionString")["Database"].ToString();
        }
    }
}

using Microsoft.Extensions.Configuration;
using Pardakht.UserManagement.Infrastructure.Interfaces;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository
{
    public class ConnectionStringManager : IConnectionStringManager
    {
        public string Server { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string Database { get; private set; }
        public string MainConnectionString
        {
            get { return $"Server={Server};User={User};Password={Password};Database={Database}"; }
        }

        public ConnectionStringManager(IConfiguration config)
        {
            Server = config.GetSection("ConnectionStringOperational")["Server"].ToString();
            User = config.GetSection("ConnectionStringOperational")["User"].ToString();
            Password = config.GetSection("ConnectionStringOperational")["Password"].ToString();
            Database = config.GetSection("ConnectionStringOperational")["Database"].ToString();
        }
    }
}

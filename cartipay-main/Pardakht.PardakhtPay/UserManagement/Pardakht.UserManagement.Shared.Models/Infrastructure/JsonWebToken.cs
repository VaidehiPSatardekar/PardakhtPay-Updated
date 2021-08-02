using System;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class JsonWebToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
    }
}

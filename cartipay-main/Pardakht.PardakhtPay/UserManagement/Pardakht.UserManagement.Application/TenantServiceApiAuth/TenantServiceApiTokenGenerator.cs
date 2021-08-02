using System;
using System.Threading.Tasks;

namespace Pardakht.UserManagement.Application.TenantServiceApiAuth
{
    public class TenantServiceApiTokenGenerator
    {
        private readonly TenantServiceApiAuth marketServiceApiAuth;
        private readonly TenantServiceInfo tenantServiceInfo;


        //public string Token { get; set; }

        public async Task<string> Token()
        {
            if (tenantServiceInfo.Expire < DateTime.UtcNow || string.IsNullOrEmpty(tenantServiceInfo.Token))
            {
                await marketServiceApiAuth.GenerateNewToken();
            }

            return tenantServiceInfo.Token;
        }

        public TenantServiceApiTokenGenerator(TenantServiceApiAuth marketServiceApiAuth, TenantServiceInfo tenantServiceInfo)
        {
            this.marketServiceApiAuth = marketServiceApiAuth;
            this.tenantServiceInfo = tenantServiceInfo;
            //if (marketServiceApiAuth.Expire < DateTime.UtcNow || string.IsNullOrEmpty(marketServiceApiAuth.Token))
            //{
            //    marketServiceApiAuth.GenerateNewToken();
            //}
            //Token = marketServiceApiAuth.Token;
        }
    }
}

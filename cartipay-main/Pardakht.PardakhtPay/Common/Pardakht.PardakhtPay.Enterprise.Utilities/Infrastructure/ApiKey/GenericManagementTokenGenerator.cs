using System;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey
{
    public class GenericManagementTokenGenerator<T> where T : ApiSettings, new()
    {
        private readonly GenericManagementAuth<T> genericManagementAuth;

        private readonly ILogger<GenericManagementTokenGenerator<T>> logger;

        public virtual string Token
        {
            get
            {
                try
                {
                    if (genericManagementAuth.Expire.ToUniversalTime() < DateTime.UtcNow || string.IsNullOrEmpty(genericManagementAuth.Token))
                    {
                        logger.LogInformation("GenericManagementTokenGenerator: generating new token");
                        genericManagementAuth.GenerateNewToken();
                    }

                    return genericManagementAuth.Token;
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, $"Generation token error {ex.Message}");
                    return null;
                }
            }
        }

        public DateTime TokenExpiryDate
        {
            get
            {
                return genericManagementAuth.Expire;
            }
        }

        public GenericManagementTokenGenerator(GenericManagementAuth<T> genericManagementAuth, ILogger<GenericManagementTokenGenerator<T>> logger)
        {
            this.genericManagementAuth = genericManagementAuth;
            this.logger = logger;
        }
    }
}

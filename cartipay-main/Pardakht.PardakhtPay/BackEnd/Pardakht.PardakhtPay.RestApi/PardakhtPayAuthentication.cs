using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Security.Claims;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Represents a class to manage PardakhtPay authentication
    /// </summary>
    public class PardakhtPayAuthentication
    {
        private readonly RequestDelegate _next;
        //private const string TenantKey = "account-context";

        /// <summary>
        /// Gets or sets default connection string of the application
        /// </summary>
        public static string CurrentConnectionString { get; set; }

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="next"></param>
        public PardakhtPayAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Handles invoking the current operation
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            try
            {
                if (context != null && context.Request != null && context.Request.Method != "OPTIONS")
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<PardakhtPayAuthentication>>();
                    var configuration = context.RequestServices.GetRequiredService<IOptions<BankBotConfiguration>>().Value;
                    var platformSettings = context.RequestServices.GetRequiredService<IOptions<PlatformInformationSettings>>().Value;

                    try
                    {
                        CurrentUser user = context.RequestServices.GetRequiredService<CurrentUser>();

                        if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated && context.Request != null && context.Request.Method != "OPTIONS")
                        {
                            user.CallbackUrl = configuration.CallbackUrl;

                            user.Name = context.User.Identity.Name;
                            var identifier = context.User.FindFirst(ClaimTypes.NameIdentifier);

                            if (identifier != null)
                            {
                                user.IdentifierGuid = identifier.Value;
                            }

                            Claim userData = context.User.FindFirst(ClaimTypes.UserData);

                            if (userData != null && !string.IsNullOrEmpty(userData.Value))
                            {
                                var tokenUserData = JsonConvert.DeserializeObject<TenantGuidArrayType>(userData.Value);

                                user.ParentAccountId = tokenUserData.ParentAccountId;
                            }

                            var roleDatas = context.User.FindAll(ClaimTypes.Role).ToList();

                            if (roleDatas != null && roleDatas.Count > 0)
                            {
                                user.Roles = roleDatas.Select(p => p.Value).ToList();
                            }

                            IMerchantRepository merchantRepository = context.RequestServices.GetRequiredService<IMerchantRepository>();
                            IOwnerBankLoginRepository ownerBankLoginRepository = context.RequestServices.GetRequiredService<IOwnerBankLoginRepository>();

                            var logins = await ownerBankLoginRepository.GetAllAsync();
                            user.LoginGuids = logins.Select(p => p.BankLoginGuid).ToList();
                        }
                        else
                        {
                            user.CallbackUrl = configuration.CallbackUrl;
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;

                        logger.LogError(ex.Message, ex);
                    }
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        ///// <summary>
        ///// Sets connection string to the current user
        ///// </summary>
        ///// <param name="user">CurrentUser instance</param>
        ///// <param name="context">Current http context instance</param>
        ///// <returns></returns>
        //private async Task SetConnectionString(CurrentUser user, HttpContext context)
        //{
        //    try
        //    {
        //        //if (!string.IsNullOrEmpty(user.CurrentTenantGuid))
        //        //{
        //        //    var service = context.RequestServices.GetRequiredService<ITenantCacheService>();

        //        //    var connectionString = await service.GetTenantConnectionString(user.CurrentTenantGuid);

        //        //    if (!string.IsNullOrEmpty(connectionString))
        //        //    {
        //        //        user.ConnectionString = connectionString;
        //        //    }
        //        //}

        //        //#if DEBUG
        //        user.ConnectionString = CurrentConnectionString;
        //        //#endif
        //    }
        //    catch (Exception ex)
        //    {
        //        context.RequestServices.GetRequiredService<ILogger<PardakhtPayAuthentication>>().LogError(ex, ex.Message);
        //    }

        //    user.ConnectionString = user.ConnectionString ?? CurrentConnectionString;
        //    //user.ConnectionString = CurrentConnectionString;
        //}
    }

    class TenantGuidArrayType
    {
        //public string[] TenantGuid { get; set; }

        //public string[] TenantDomainPlatformMapGuid { get; set; }

        public string TenantGuid { get; set; }

        public string PlatformGuid { get; set; }

        public string ParentAccountId { get; set; }
    }
}

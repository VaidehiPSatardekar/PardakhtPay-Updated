using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.User;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.User;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.User
{
    public class UserValidation : JwtBearerEvents
    {
        private readonly IUserResolverService userResolver;

        public UserValidation(IUserResolverService userResolver)
        {
            this.userResolver = userResolver;
        }

        public override async Task TokenValidated(TokenValidatedContext tokenContext)
        {
            var userContext = tokenContext.HttpContext.RequestServices.GetRequiredService<UserContext>();
            userContext.OriginalCallerContext = null;
            userContext.ProxyCallerContext = null;

            // embedded caller - the user who logged in
            userContext.OriginalCallerContext = await BuildOriginalCallerContext(tokenContext);
            if (userContext.OriginalCallerContext != null)
            {
                CheckIfBlocked(userContext.OriginalCallerContext.User, tokenContext);
                CheckIfDeleted(userContext.OriginalCallerContext.User, tokenContext);
            }

            // proxy account making call - api key login or user if not being called by proxy
            userContext.ProxyCallerContext = await BuildProxyCallerContext(tokenContext);
            if (userContext.ProxyCallerContext != null)
            {
                CheckIfBlocked(userContext.ProxyCallerContext.User, tokenContext);
                CheckIfDeleted(userContext.ProxyCallerContext.User, tokenContext);
            }
        }

        private void CheckIfBlocked(StaffUser user, TokenValidatedContext context)
        {
            // send a 401 back if blocked
            if (user != null)
            {
                if (user.IsBlocked)
                {
                    context.Fail($"User {user.Username} is blocked");
                }
            }
        }

        private void CheckIfDeleted(StaffUser user, TokenValidatedContext context)
        {
            // send a 401 back if blocked
            if (user != null)
            {
                if (user.IsDeleted)
                {
                    context.Fail($"User {user.Username} is deleted");
                }
            }
        }

        private async Task<SecurityContext> BuildOriginalCallerContext(TokenValidatedContext tokenContext)
        {

            var userData = Helper.HttpHeaderHelper.GetHeaderKeyValue(tokenContext.Request, "UserData");

            if (userData.Value.Count > 0)
            {
                var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userData.Value);
                UserData userDataObject = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(temp.ToString());
                var securityContext = new SecurityContext();
                securityContext.TokenInfo = new TokenInfo();
                securityContext.TokenInfo.UserData = userDataObject;
                var nameIdentifier = Helper.HttpHeaderHelper.GetHeaderKeyValue(tokenContext.Request, "NameIdentifier");
                securityContext.TokenInfo.AccountId = nameIdentifier.Value;
                var username = Helper.HttpHeaderHelper.GetHeaderKeyValue(tokenContext.Request, "UserName");
                securityContext.TokenInfo.Username = username.Value;
                var roles = Helper.HttpHeaderHelper.GetHeaderKeyValue(tokenContext.Request, "Roles");
                if (!string.IsNullOrEmpty(roles.Value))
                {
                    securityContext.TokenInfo.Roles = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(roles.Value);
                }
                if (securityContext.TokenInfo.UserData == null || securityContext.TokenInfo.UserData.UserType != (int)UserType.ApiUser)
                {
                    securityContext.User = await userResolver.GetUser(securityContext.AccountId, securityContext.TenantGuid, securityContext.PlatformGuid);
                }
                else
                {
                    securityContext.User = new StaffUser()
                    {
                        AccountId = securityContext.AccountId,
                        TenantGuid = securityContext.TenantGuid,
                        Username = securityContext.Username,
                        Permissions = tokenContext.Principal.Claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).ToList(),
                        UserType = UserType.ApiUser
                    };
                }

                return securityContext;
            }

            return null;
        }

        private async Task<SecurityContext> BuildProxyCallerContext(TokenValidatedContext tokenContext)
        {
            if (tokenContext.Principal != null && tokenContext.Principal.Claims != null)
            {
                var securityContext = new SecurityContext();
                securityContext.TokenInfo = new TokenInfo();
                var userData = tokenContext.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData)?.Value;
                if (!string.IsNullOrEmpty(userData))
                {
                    securityContext.TokenInfo.UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(userData);
                } // TODO: else??
                securityContext.TokenInfo.AccountId = tokenContext.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
                securityContext.TokenInfo.Username = tokenContext.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;

                if (securityContext.TokenInfo.UserData == null || securityContext.TokenInfo.UserData.UserType != (int)UserType.ApiUser)
                {
                    securityContext.User = await userResolver.GetUser(securityContext.AccountId, securityContext.TenantGuid, securityContext.PlatformGuid);
                }
                else
                {
                    securityContext.User = new StaffUser()
                    {
                        AccountId = securityContext.AccountId,
                        TenantGuid = securityContext.TenantGuid,
                        Username = securityContext.Username,
                        Permissions = tokenContext.Principal.Claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).ToList(),
                        UserType = UserType.ApiUser
                    };
                }

                return securityContext;
            }

            return null;
        }
    }
}

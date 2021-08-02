using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.UserManagement.Shared.Models.Configuration;

namespace Pardakht.UserManagement.Web.RestService
{
    public class UserContextResolver
    {
        private readonly RequestDelegate next;

        public UserContextResolver(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userContext = context.RequestServices.GetRequiredService<UserContext>();

            var userData = context.Request.Headers.FirstOrDefault(p => p.Key == "UserData");
            if (userData.Value.Count == 0)
            {
                //If the user data is empty, it means there isn't any logged in user. For this case, we should resolve api key credentials
                if (context.User.Identity.IsAuthenticated)
                {
                    var userDataClaim = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData)?.Value;

                    if (!string.IsNullOrEmpty(userDataClaim))
                    {
                        var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userDataClaim);
                        userContext.UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(temp.ToString());
                        userContext.TenantGuid = userContext.UserData.TenantGuid == null ? string.Empty : userContext.UserData.TenantGuid.ToString();
                        userContext.PlatformGuid = userContext.UserData.PlatformGuid == null ? string.Empty : userContext.UserData.PlatformGuid.ToString();
                        userContext.ParentAccountId = userContext.UserData.ParentAccountId == null ? string.Empty : userContext.UserData.ParentAccountId.ToString();
                    }

                    var nameIdentifier = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier);
                    userContext.AccountId = nameIdentifier.Value;

                    var username = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name);
                    userContext.Username = username.Value;

                    userContext.Roles = context.User.Claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value).ToArray();
                }

                await next(context);
                //return;
            }
            else
            {
                var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userData.Value);
                userContext.UserData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(temp.ToString());
                userContext.TenantGuid = userContext.UserData.TenantGuid == null ? string.Empty : userContext.UserData.TenantGuid.ToString();
                userContext.PlatformGuid = userContext.UserData.PlatformGuid == null ? string.Empty : userContext.UserData.PlatformGuid.ToString();
                userContext.ParentAccountId = userContext.UserData.ParentAccountId == null ? string.Empty : userContext.UserData.ParentAccountId.ToString();

                var nameIdentifier = context.Request.Headers.FirstOrDefault(p => p.Key == "NameIdentifier");
                userContext.AccountId = nameIdentifier.Value;

                var username = context.Request.Headers.FirstOrDefault(p => p.Key == "UserName");
                userContext.Username = username.Value;

                var roles = context.Request.Headers.FirstOrDefault(p => p.Key == "Roles");
                if (!string.IsNullOrEmpty(roles.Value))
                {
                    userContext.Roles = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(roles.Value);
                }

                await next(context);
            }
        }
    }
}


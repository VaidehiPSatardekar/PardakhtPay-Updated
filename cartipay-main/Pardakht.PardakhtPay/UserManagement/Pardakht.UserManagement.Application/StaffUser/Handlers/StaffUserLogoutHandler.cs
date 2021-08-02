using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class StaffUserLogoutHandler : StaffUserHandlerBase
    {
        public StaffUserLogoutHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) {}

        public async Task<WebResponse> Handle()
        {
            var loggedOnUser = await staffUserManager.GetByAccountId(userContext.AccountId, userContext.TenantGuid, userContext.PlatformGuid);

            await staffUserManager.LogoutStaffUser(loggedOnUser, loggedOnUser, loggedOnUser.TenantGuid, userContext.PlatformGuid);
            return new WebResponse
            {
                Success = true,
                Message = string.Empty
            };
        }
    }
}

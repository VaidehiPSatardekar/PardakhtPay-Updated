using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class StaffUserUpdateTrackingTimeHandler : StaffUserHandlerBase
    {
        public StaffUserUpdateTrackingTimeHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) {}

        public async Task<WebResponse<StaffUserPerformanceTime>> Handle()
        {
            var loggedOnUser = await staffUserManager.GetByAccountId(userContext.AccountId, userContext.TenantGuid, userContext.PlatformGuid);
            
            var addedIdleMinutes = await staffUserManager.UpdateTrackingTimeStaffUser(loggedOnUser.Id, userContext.TenantGuid, userContext.PlatformGuid);
            return new WebResponse<StaffUserPerformanceTime>
            {
                Success = true,
                Message = string.Empty,
                Payload = addedIdleMinutes
            };
        }
    }
}

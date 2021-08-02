using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class StaffUserAddIdleMinutesHandler : StaffUserHandlerBase
    {
        public StaffUserAddIdleMinutesHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) {}

        public async Task<WebResponse<StaffUserPerformanceTime>> Handle(int addIdleMinutes)
        {
            var loggedOnUser = await staffUserManager.GetByAccountId(userContext.AccountId, userContext.TenantGuid, userContext.PlatformGuid);
            
            var addedIdleMinutes = await staffUserManager.AddIdleMinutesStaffUser(loggedOnUser.Id, addIdleMinutes, userContext.TenantGuid, userContext.PlatformGuid);
            return new WebResponse<StaffUserPerformanceTime>
            {
                Success = true,
                Message = string.Empty,
                Payload = addedIdleMinutes
            };
        }
    }
}

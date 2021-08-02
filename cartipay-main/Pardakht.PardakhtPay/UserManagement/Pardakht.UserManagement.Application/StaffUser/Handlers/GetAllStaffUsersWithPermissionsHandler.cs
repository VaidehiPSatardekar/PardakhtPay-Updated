using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class GetAllStaffUsersWithPermissionsHandler : StaffUserHandlerBase
    {
        public GetAllStaffUsersWithPermissionsHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) { }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> Handle(string platformGuid, IList<string> permissionCodes)
        {
            try
            {
                IEnumerable<Shared.Models.WebService.StaffUser> result;
                // trying to get all users for a platform
                if (userContext.IsProviderUser())
                {
                    result = await staffUserManager.GetAllUsersWithPermissions(platformGuid, permissionCodes);
                }
                else
                {
                    // tenant users cannot view provider user lists
                    // throw new Exception("You don't have permission for this operation");
                    return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>
                    {
                        Success = false,
                        Message = "You don't have permission for this operation"
                    };
                }

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetAllStaffUsersWithPermissionsHandler.Handle: an error occurred getting all staff users for platform {platformGuid}, permissions {String.Join(',', permissionCodes)} - {ex}");
                throw;
            }
        }
    }
}

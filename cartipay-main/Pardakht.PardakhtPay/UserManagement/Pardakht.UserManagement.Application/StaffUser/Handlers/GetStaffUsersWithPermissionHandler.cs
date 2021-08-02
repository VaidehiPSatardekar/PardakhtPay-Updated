using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class GetStaffUsersWithPermissionHandler : StaffUserHandlerBase
    {
        public GetStaffUsersWithPermissionHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) { }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> Handle(string platformGuid, string tenantGuid, string permissionCode)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserAccess))
                {
                    return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>> { Success = false, Message = "User not authorised to view users" };
                }

                int? tenantId = null;

                if (!string.IsNullOrEmpty(tenantGuid))
                {
                    var tenant = await GetTenant(tenantGuid);
                    tenantId = tenant.Id;
                    if (!userContext.IsProviderUser())
                    {
                        if (tenantGuid != userContext.TenantGuid)
                        {
                            return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>> { Success = false, Message = "Tenant users cannot view users from other tenancies" };
                        }
                    }
                }
                else
                {
                    if (!userContext.IsProviderUser())
                    {
                        return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>> { Success = false, Message = "Tenant users cannot view provider users" };
                    }
                }

                var result = await staffUserManager.GetUsersWithPermission(platformGuid, tenantId, permissionCode, tenantGuid);

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetStaffUsersWithPermissionHandler.Handle: an error occurred getting staff users for platform {platformGuid}, tenant {tenantGuid}, permission {permissionCode} - {ex}");
                throw;
            }
        }
    }
}

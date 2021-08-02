using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class GetAffiliateUsersHandler : StaffUserHandlerBase
    {
        public GetAffiliateUsersHandler(StaffUserHandlerArgs handlerArgs) : base(handlerArgs) { }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> Handle(string platformGuid, string tenantGuid)
        {
            try
            {
                IEnumerable<Shared.Models.WebService.StaffUser> result;
                if (string.IsNullOrEmpty(tenantGuid))
                {
                    // trying to get all provider users for a platform
                    if (userContext.IsProviderUser())
                    {
                        // TODO: check user has permission
                        result = await staffUserManager.GetProviderUsersByUserTypeAsync(platformGuid,UserType.AffiliateUser).ConfigureAwait(false);
                    }
                    else
                    {
                        // tenant users cannot view provider user lists
                        //throw new Exception("You don't have permission for this operation");
                        return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>
                        {
                            Success = false,
                            Message = "You don't have permission for this operation"
                        };
                    }
                }
                else
                {
                    var tenant = await GetTenant(tenantGuid).ConfigureAwait(false);

                    // getting list of tenant users
                    if (userContext.IsProviderUser())
                    {
                        // parent account doesn't apply to provider users
                        result = await staffUserManager.GetTenantUsersByUserTypeAsync(platformGuid, tenant.Id, true, null, tenantGuid, UserType.AffiliateUser).ConfigureAwait(false);
                    }
                    else
                    {
                        // this is a tenant user, so check if they can see all accounts, or just the ones that they own
                        //var parentAccountId = userData.ParentAccountId == null ? string.Empty : userData.ParentAccountId.ToString();
                        var parentAccountId = userContext.ParentAccountId;

                        if (string.IsNullOrEmpty(parentAccountId))
                        {
                            parentAccountId = userContext.AccountId;
                        }

                        var allOwners = userContext.HasRole(PermissionConstants.SeeAllOwners);

                        result = await staffUserManager.GetTenantUsersByUserTypeAsync(platformGuid, tenant.Id, allOwners, parentAccountId, tenantGuid, UserType.AffiliateUser).ConfigureAwait(false);
                    }
                }

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetAffiliateUsersHandler.Handle: an error occurred getting staff users for platform {platformGuid}, tenant {tenantGuid} - {ex}");
                throw;
            }
        }
    }
}

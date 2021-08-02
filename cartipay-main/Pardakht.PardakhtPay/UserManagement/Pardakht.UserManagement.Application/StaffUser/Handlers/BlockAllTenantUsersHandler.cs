using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class BlockAllTenantUsersHandler : StaffUserHandlerBase
    {
        private readonly IRoleManager _roleManager;
        private readonly StaffUserHandlerArgs _handlerArgs;

        public BlockAllTenantUsersHandler(IRoleManager roleManager, StaffUserHandlerArgs handlerArgs) : base(handlerArgs)
        {
            _roleManager = roleManager;
            _handlerArgs = handlerArgs;
        }

        public async Task<WebResponse> Handle(BlockAllUsersRequest request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserEdit))
                {
                    return new WebResponse { Success = false, Message = "User not authorised to update users" };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                if (request.UserType != UserType.ApiUser)
                {
                    return new WebResponse { Success = false, Message = "Not an API user!" };
                }

                var tenant = await GetTenant(request.TenantPlatformMapGuid);

                if (!userContext.IsProviderUser())
                {
                    throw new Exception("Tenant users cannot block users!");
                }

                var result = await staffUserManager.BlockAllTenantUsers(tenant.Id, actionedByUser, request);

                return new WebResponse
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"BlockAllTenantUsersHandler.Handle: an error occurred blocking staff users of tenant {request.TenantPlatformMapGuid} - {ex}");
                throw;
            }
        }
    }
}
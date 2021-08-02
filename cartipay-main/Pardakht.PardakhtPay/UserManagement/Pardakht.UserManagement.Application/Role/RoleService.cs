using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.UserManagement.Application.Role.Handlers;
using Pardakht.UserManagement.Application.TenantServiceApiAuth;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.Role
{
    public class RoleService : IRoleService
    {
        private readonly IRoleManager roleManager;
        private readonly Pardakht.UserManagement.Shared.Models.Configuration.UserContext userContext;
        private readonly IStaffUserManager staffUserManager;
        private readonly ILogger<RoleService> logger;
        private readonly TenantManagementSettings _tenantManagementSettings;
        private readonly TenantServiceApiTokenGenerator _tenantServiceApiTokenGenerator;
        private readonly IHttpClientFactory _httpClientFactory;

        public RoleService(IRoleManager roleManager, Pardakht.UserManagement.Shared.Models.Configuration.UserContext userContext, IStaffUserManager staffUserManager, ILogger<RoleService> logger,
                                    IOptions<TenantManagementSettings> tenantManagementSettings,
                                    TenantServiceApiTokenGenerator tenantServiceApiTokenGenerator,
                                    IHttpClientFactory httpClientFactory)
        {
            this.roleManager = roleManager;
            this.userContext = userContext;
            this.staffUserManager = staffUserManager;
            this.logger = logger;
            _tenantManagementSettings = tenantManagementSettings.Value;
            _tenantServiceApiTokenGenerator = tenantServiceApiTokenGenerator;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WebResponse<IEnumerable<RoleDto>>> GetRoles(string platformGuid, string tenantGuid)
        {
            var handler = new GetRolesHandler(roleManager, userContext, logger, _tenantManagementSettings,
                _tenantServiceApiTokenGenerator, _httpClientFactory);
            return await handler.Handle(platformGuid, tenantGuid);
        }

        public WebResponse<IEnumerable<PermissionGroupDto>> GetPermissionsGroups(string platformGuid)
        {
            var result = roleManager.GetPermissionsGroups(platformGuid);

            return new WebResponse<IEnumerable<PermissionGroupDto>>
            {
                Payload = result,
                Success = true
            };
        }

        public async Task<WebResponse<RoleDto>> UpdateRole(RoleDto request)
        {
            var handler = new UpdateRoleHandler(roleManager, userContext, staffUserManager, logger);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<RoleDto>> AddRole(RoleDto request)
        {
            var handler = new AddRoleHandler(roleManager, userContext, staffUserManager, logger);
            return await handler.Handle(request);
        }
    }
}

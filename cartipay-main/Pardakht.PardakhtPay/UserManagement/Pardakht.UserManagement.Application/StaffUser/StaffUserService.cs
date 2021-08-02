using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Application.StaffUser.Handlers;
using Pardakht.UserManagement.Application.TenantServiceApiAuth;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser
{
    public class StaffUserService : IStaffUserService
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly IServiceProvider serviceProvider;
        private readonly IRoleManager roleManager;
        private readonly IAuditLogManager auditLogManager;
        private readonly StaffUserHandlerArgs handlerArgs;
        private readonly ILogger logger;

        public StaffUserService(UserManager<ApplicationUser> identityUserManager,
                                UserContext userContext,
                                IStaffUserManager staffUserManager,
                                IJwtTokenService jwtTokenService,
                                IOptions<SendGridSettings> emailSettings,
                                IRoleManager roleManager,
                                IServiceProvider serviceProvider,
                                IOptions<TenantManagementSettings> tenantManagementSettings,
                                IOptions<RoleSettings> roleSettings,
                                TenantServiceApiTokenGenerator tenantServiceApiTokenGenerator,
                                IAuditLogManager auditLogManager,
                                ILogger<StaffUserService> logger,
                                IHttpClientFactory httpClientFactory,
                                IOptions<EmailNotification> emailNotification)
        {
            this.jwtTokenService = jwtTokenService;
            this.roleManager = roleManager;
            this.serviceProvider = serviceProvider;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
            handlerArgs = new StaffUserHandlerArgs(identityUserManager, staffUserManager, httpClientFactory, tenantManagementSettings.Value, 
                userContext, tenantServiceApiTokenGenerator, emailSettings.Value, logger, roleSettings.Value,emailNotification.Value);
        }

        public async Task<WebResponse<CreateStaffUserResponse>> CreateStaffUser(Shared.Models.WebService.StaffUser staffUserDto)
        {
            var handler = new CreateStaffUserHandler( handlerArgs);

            return await handler.Handle(staffUserDto);
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> UpdateStaffUser(Shared.Models.WebService.StaffUser staffUserDto)
        {
            var handler = new UpdateStaffUserHandler( handlerArgs);

            return await handler.Handle(staffUserDto);
        }

        public async Task<WebResponse<ConfigureTenantUsersResponse>> ConfigureTenantUsers(ConfigureTenantUsersRequest request)
        {
            var handler = new ConfigureTenantUsersHandler(roleManager, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetListAll(string platformGuid)
        {
            var handler = new GetAllStaffUsersHandler(handlerArgs);

            return await handler.Handle(platformGuid);
        }

        public Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetList(string platformGuid, string tenantGuid, int? brandId = null)
        {
            var handler = new GetStaffUsersHandler(handlerArgs);
            return handler.Handle(platformGuid, tenantGuid, brandId);
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetSystemUserList(string platformGuid, string tenantGuid)
        {
            var handler = new GetSystemUsersHandler(handlerArgs);
            return await handler.Handle(platformGuid, tenantGuid).ConfigureAwait(false);
        }


        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAffiliateList(string platformGuid, string tenantGuid)
        {
            var handler = new GetAffiliateUsersHandler(handlerArgs);
            return await handler.Handle(platformGuid, tenantGuid).ConfigureAwait(false);
        }
        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAllUsersWithPermission(string platformGuid, string permissionCode)
        {
            var handler = new GetAllStaffUsersWithPermissionHandler(handlerArgs);

            return await handler.Handle(platformGuid, permissionCode);
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetUsersWithPermission(string platformGuid, string tenantGuid, string permissionCode)
        {
            var handler = new GetStaffUsersWithPermissionHandler(handlerArgs);

            return await handler.Handle(platformGuid, tenantGuid, permissionCode);
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAllUsersWithPermissions(string platformGuid, IList<string> permissionCodes)
        {
            var handler = new GetAllStaffUsersWithPermissionsHandler(handlerArgs);

            return await handler.Handle(platformGuid, permissionCodes);
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetUsersWithPermissions(string platformGuid, string tenantGuid, IList<string> permissionCodes)
        {
            var handler = new GetStaffUsersWithPermissionsHandler(handlerArgs);

            return await handler.Handle(platformGuid, tenantGuid, permissionCodes);
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> GetByEmail(string email)
        {
            var result = await handlerArgs.staffUserManager.GetByEmail(email);

            return new WebResponse<Shared.Models.WebService.StaffUser>
            {
                Success = true,
                Payload = result
            };
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> GetUserDetails()
        {
            return await GetUserDetails(handlerArgs.userContext.AccountId, handlerArgs.userContext.TenantGuid, handlerArgs.userContext.PlatformGuid);
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> GetUserDetails(string accountId, string tenantGuid, string platformGuid, bool all = false)
        {
            var result = await handlerArgs.staffUserManager.GetByAccountId(accountId, tenantGuid, platformGuid, all);

            return new WebResponse<Shared.Models.WebService.StaffUser>
            {
                Success = (result != null),
                Payload = result
            };
        }

        public async Task<WebResponse<IEnumerable<StaffUserStatusLastLogin>>> GetStaffUsersStatusLastLogin(string tenantGuid, IEnumerable<int> staffUserIds, bool all = false,string timeZoneId=null)
        {
            var handler = new GetStaffUsersStatusLastLoginHandler(handlerArgs, jwtTokenService, serviceProvider);

            return await handler.Handle(tenantGuid, staffUserIds, all, timeZoneId);
        }

        public async Task<WebResponse<IEnumerable<StaffUserStatusLastLogin>>> GetStaffUsersStatusLastLoginWithPlatformGuid(string platformGuid,string tenantGuid, IEnumerable<int> staffUserIds, bool all = false, string timeZoneId = null)
        {
            var handler = new GetStaffUsersStatusLastLoginHandler(handlerArgs, jwtTokenService, serviceProvider);

            return await handler.Handle2(platformGuid,tenantGuid, staffUserIds, all, timeZoneId);
        }
        public async Task<WebResponse<IEnumerable<StaffUserPerformanceTime>>> GetStaffUsersPerformanceTime(string tenantGuid, DateTime dateFrom, DateTime dateTo, bool all = false,string timeZoneId=null)
        {
            var handler = new GetStaffUsersPerformanceTimeHandler(handlerArgs,jwtTokenService,serviceProvider);

            return await handler.Handle(tenantGuid, dateFrom, dateTo, all, timeZoneId);
        }

        public async Task<WebResponse<StaffUserPerformanceTime>> StaffUserAddIdleMinutes(int addIdleMinutes)
        {
            var handler = new StaffUserAddIdleMinutesHandler(handlerArgs);

            return await handler.Handle(addIdleMinutes);
        }

        public async Task<WebResponse<StaffUserPerformanceTime>> StaffUserUpdateTrackingTime()
        {
            var handler = new StaffUserUpdateTrackingTimeHandler(handlerArgs);

            return await handler.Handle();
        }

        public async Task<WebResponse> StaffUserLogout()
        {
            var handler = new StaffUserLogoutHandler(handlerArgs);

            return await handler.Handle();
        }

        public async Task<WebResponse<LoginResponse>> StaffUserLogin(LoginRequest request)
        {
            var handler = new StaffUserLoginHandler(jwtTokenService, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<LoginResponse>> LoginAs(LoginAsRequest request)
        {
            var handler = new StaffUserLoginAsHandler(jwtTokenService, logger,  handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<JsonWebToken>> ApiKeyLogin(ApiKeyLoginRequest request)
        {
            var handler = new ApiKeyLoginHandler(jwtTokenService, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<JsonWebToken>> RefreshToken()
        {
            var handler = new RefreshTokenHandler(jwtTokenService, handlerArgs);

            return await handler.Handle();
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> BlockStaffUser(BlockStaffUserRequest request)
        {
            var handler = new BlockStaffUserHandler( handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> DeleteStaffUser(DeleteStaffUserRequest request)
        {
            var handler = new DeleteStaffUserHandler( handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<PasswordChangeResponse>> ChangePassword(PasswordChangeRequest request)
        {
            var handler = new ChangePasswordHandler( auditLogManager, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse<PasswordResetResponse>> ResetStaffUserPassword(PasswordResetRequest request)
        {
            var handler = new ResetPasswordHandler( auditLogManager, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse> ForgotPassword(ForgotResetRequest request)
        {
            var handler = new ForgotPasswordHandler( auditLogManager, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse> ForgotPasswordBuUsername(ForgotResetByUsernameRequest request)
        {
            var handler = new ForgotPasswordByUsernameHandler( auditLogManager, handlerArgs);

            return await handler.Handle(request);
        }

        public async Task<WebResponse> BlockAllTenantUsers(BlockAllUsersRequest request)
        {
            var handler = new BlockAllTenantUsersHandler(roleManager, handlerArgs);

            return await handler.Handle(request);
        }

    }
}

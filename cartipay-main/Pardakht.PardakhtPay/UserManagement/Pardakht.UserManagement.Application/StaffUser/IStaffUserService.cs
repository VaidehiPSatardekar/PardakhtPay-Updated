using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser
{
    public interface IStaffUserService
    {

        Task<WebResponse<Shared.Models.WebService.StaffUser>> GetUserDetails();
        Task<WebResponse<Shared.Models.WebService.StaffUser>> GetUserDetails(string accountId, string tenantGuid, string platformGuid, bool all = false);
        Task<WebResponse<CreateStaffUserResponse>> CreateStaffUser(Shared.Models.WebService.StaffUser staffUserDto);
        Task<WebResponse<Shared.Models.WebService.StaffUser>> UpdateStaffUser(Shared.Models.WebService.StaffUser staffUserDto);
        Task<WebResponse<ConfigureTenantUsersResponse>> ConfigureTenantUsers(ConfigureTenantUsersRequest request);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetListAll(string platformGuid);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetList(string platformGuid, string tenantGuid, int? brandId = null);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAllUsersWithPermission(string platformGuid, string permissionCode);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetUsersWithPermission(string platformGuid, string tenantGuid, string permissionCode);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAllUsersWithPermissions(string platformGuid, IList<string> permissionCodes);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetUsersWithPermissions(string platformGuid, string tenantGuid, IList<string> permissionCodes);
        Task<WebResponse<Shared.Models.WebService.StaffUser>> GetByEmail(string email);
        Task<WebResponse<IEnumerable<StaffUserStatusLastLogin>>> GetStaffUsersStatusLastLogin(string tenantGuid, IEnumerable<int> staffUserIds, bool all = false,string timeZoneId=null);
        Task<WebResponse<IEnumerable<StaffUserStatusLastLogin>>> GetStaffUsersStatusLastLoginWithPlatformGuid(string platformGuid, string tenantGuid, IEnumerable<int> staffUserIds, bool all = false, string timeZoneId = null);
        Task<WebResponse<IEnumerable<StaffUserPerformanceTime>>> GetStaffUsersPerformanceTime(string tenantGuid, DateTime dateFrom, DateTime dateTo, bool all = false,string timeZoneId=null);
        Task<WebResponse<StaffUserPerformanceTime>> StaffUserAddIdleMinutes(int addIdleMinutes);
        Task<WebResponse<StaffUserPerformanceTime>> StaffUserUpdateTrackingTime();
        Task<WebResponse> StaffUserLogout();
        Task<WebResponse<LoginResponse>> StaffUserLogin(LoginRequest model);
        Task<WebResponse<LoginResponse>> LoginAs(LoginAsRequest request);
        Task<WebResponse<JsonWebToken>> ApiKeyLogin(ApiKeyLoginRequest model);
        Task<WebResponse<Shared.Models.WebService.StaffUser>> BlockStaffUser(BlockStaffUserRequest request);
        Task<WebResponse<Shared.Models.WebService.StaffUser>> DeleteStaffUser(DeleteStaffUserRequest request);
        Task<WebResponse<JsonWebToken>> RefreshToken();
        Task<WebResponse<PasswordChangeResponse>> ChangePassword(PasswordChangeRequest request);
        Task<WebResponse<PasswordResetResponse>> ResetStaffUserPassword(PasswordResetRequest request);
        Task<WebResponse> ForgotPassword(ForgotResetRequest request);
        Task<WebResponse> ForgotPasswordBuUsername(ForgotResetByUsernameRequest request);
        Task<WebResponse> BlockAllTenantUsers(BlockAllUsersRequest request);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetAffiliateList(string platformGuid, string tenantGuid);
        Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUser>>> GetSystemUserList(string platformGuid, string tenantGuid);
    }
}

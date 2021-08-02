using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Application.StaffUser;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Web.RestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffUserController : ControllerBase
    {
        private readonly IStaffUserService staffUserService;
        private readonly ILogger<StaffUserController> logger;
        public StaffUserController(IStaffUserService staffUserService, ILogger<StaffUserController> logger)
        {
            this.staffUserService = staffUserService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] StaffUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await staffUserService.CreateStaffUser(model);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.Post: an error occurred creating a new staff user for {model.Username} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] StaffUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await staffUserService.UpdateStaffUser(model);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.Put: an error occurred updating staff user for {model.Username} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("configure-tenant-users")]
        public async Task<IActionResult> ConfigureTenantUsers([FromBody] ConfigureTenantUsersRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await staffUserService.ConfigureTenantUsers(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ConfigureTenantUsers: an error occurred creating tenant database and user management data for {model.TenantPlatformMapGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("all/{platformGuid}")]
        public async Task<IActionResult> GetListAllByPlatformGuid(string platformGuid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetListAll(platformGuid);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListAllByPlatformGuid: an error occurred getting user list for {platformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{platformGuid}")]
        public async Task<IActionResult> GetListByPlatformGuid(string platformGuid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetList(platformGuid, string.Empty);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{platformGuid}/{tenantGuid}")]
        public async Task<IActionResult> GetListByPlatformGuid(string platformGuid, string tenantGuid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetList(platformGuid, tenantGuid);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} for tenant mapping {tenantGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{platformGuid}/{tenantGuid}/{brandId}")]
        public async Task<IActionResult> GetListByBrandId(string platformGuid, string tenantGuid, int brandId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetList(platformGuid, tenantGuid, brandId);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} for tenant mapping {tenantGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("statusLastLogin")]
        [Authorize]
        public async Task<IActionResult> GetStaffUsersStatusLastLogin([FromBody] StatusLastLoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetStaffUsersStatusLastLogin(model.TenantGuid, model.StaffUserIds, model.All, model.TimeZoneId);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetStaffUsersStatusLastLogin: an error occurred - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("statusLastLoginwithplatformguid")]
        [Authorize]
        public async Task<IActionResult> GetStaffUsersStatusLastLoginWithPlatformGuid([FromBody] StatusLastLoginWithPlatformGuid model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetStaffUsersStatusLastLoginWithPlatformGuid(model.PlatformGuid,model.TenantGuid, model.StaffUserIds, model.All, model.TimeZoneId);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetStaffUsersStatusLastLogin: an error occurred - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("performanceTime")]
        [Authorize]
        public async Task<IActionResult> GetStaffUsersPerformanceTime([FromBody] PerformanceTimeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetStaffUsersPerformanceTime(model.TenantGuid, model.DateFrom, model.DateTo, model.All, model.TimeZoneId);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetStaffUsersPerformanceTime: an error occurred - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("addIdleMinutes")]
        [Authorize]
        public async Task<IActionResult> AddIdleMinutes([FromBody] AddIdleMinutesRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.StaffUserAddIdleMinutes(model.AddIdleMinutes);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.AddIdleMinutes: an error occurred - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("updateTrackingTime")]
        [Authorize]
        public async Task<IActionResult> UpdateTrackingTime()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.StaffUserUpdateTrackingTime();
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.AddIdleMinutes: an error occurred - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("staff-user-logout")]
        [Authorize]
        public async Task<IActionResult> UserLogout()
        {
            try
            {
                var response = await staffUserService.StaffUserLogout();
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.UserLogout: an error occurred logging out - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("staff-user-login")]
        [Authorize]
        public async Task<IActionResult> UserLogin([FromBody] LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.StaffUserLogin(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.UserLogin: an error occurred logging in for {model.UserName} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("staff-user-login-as")]
        [Authorize]
        public async Task<IActionResult> UserLoginAs([FromBody] LoginAsRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.LoginAs(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.UserLoginAs: an error occurred logging in for {model.UserName} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api-key-login")]
        public async Task<IActionResult> ApiKeyLogin([FromBody] ApiKeyLoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.ApiKeyLogin(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ApiKeyLogin: an error occurred logging in for {model.ApiKey} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var response = await staffUserService.RefreshToken();
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.RefreshToken: an error occurred refreshing token - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await staffUserService.GetUserDetails();
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.Get: an error occurred getting user details - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-user-details/{platformGuid}/{accountId}/{tenantGuid}/{all?}")]
        public async Task<IActionResult> GetUserDetails(string platformGuid, string accountId, string tenantGuid, bool all = false)
        {
            try
            {
                var response = await staffUserService.GetUserDetails(accountId, tenantGuid, platformGuid, all);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetUserDetails: an error occurred getting user details for accountId {accountId} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-user-details/{platformGuid}/{accountId}/{all?}")]
        public async Task<IActionResult> GetUserDetails(string platformGuid, string accountId, bool all = false)
        {
            try
            {
                var response = await staffUserService.GetUserDetails(accountId, string.Empty, platformGuid, all);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetUserDetails: an error occurred getting user details for accountId {accountId} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var response = await staffUserService.GetByEmail(email);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                if (response.Payload == null)
                {
                    return NoContent();
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetByEmail: an error occurred getting user for email {email} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("block-staff-user")]
        [Authorize]
        public async Task<IActionResult> BlockStaffUser([FromBody] BlockStaffUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.BlockStaffUser(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.BlockStaffUser: an error occurred blocking staff user {model.UserId} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("delete-staff-user")]
        [Authorize]
        public async Task<IActionResult> DeleteStaffUser([FromBody] DeleteStaffUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.DeleteStaffUser(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.DeleteStaffUser: an error occurred deleting staff user {model.UserId} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("all/GetListByPermssionCode/{platformGuid}/{permissionCode}")]
        public async Task<IActionResult> GetListAllByPermissionCode(string platformGuid, string permissionCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetAllUsersWithPermission(platformGuid, permissionCode);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPermissionCode: an error occurred getting user list for {platformGuid} {permissionCode} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetListByPermssionCode/{platformGuid}/{permissionCode}")]
        public async Task<IActionResult> GetListByPermissionCode(string platformGuid, string permissionCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetUsersWithPermission(platformGuid, string.Empty, permissionCode);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPermissionCode: an error occurred getting user list for {platformGuid} {permissionCode} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetListByPermssionCode/{platformGuid}/{tenantGuid}/{permissionCode}")]
        public async Task<IActionResult> GetListByPermissionCode(string platformGuid, string tenantGuid, string permissionCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetUsersWithPermission(platformGuid, tenantGuid, permissionCode);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByPermissionCode: an error occurred getting user list for {platformGuid} {tenantGuid} {permissionCode} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("all/GetListByListPermssionCode/{platformGuid}")]
        public async Task<IActionResult> GetListAllByListPermissionCode(string platformGuid, [FromBody] IList<string> permissionCodes)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetAllUsersWithPermissions(platformGuid, permissionCodes);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListAllByListPermissionCode: an error occurred getting user list for {platformGuid} {String.Join(',', permissionCodes)} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("GetListByListPermssionCode/{platformGuid}")]
        public async Task<IActionResult> GetListByListPermissionCode(string platformGuid, [FromBody] IList<string> permissionCodes)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetUsersWithPermissions(platformGuid, string.Empty, permissionCodes);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByListPermissionCode: an error occurred getting user list for {platformGuid} {String.Join(',', permissionCodes)} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("GetListByListPermssionCode/{platformGuid}/{tenantGuid}")]
        public async Task<IActionResult> GetListByListPermissionCode(string platformGuid, string tenantGuid, [FromBody] IList<string> permissionCodes)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.GetUsersWithPermissions(platformGuid, tenantGuid, permissionCodes);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.GetListByListPermissionCode: an error occurred getting user list for {platformGuid} {tenantGuid} {String.Join(',', permissionCodes)} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Forbid();
                }

                var response = await staffUserService.ChangePassword(model);

                if (response.Success)
                {
                    return Ok();
                }

                return BadRequest(response.Message);

            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ChangePassword: an error occurred changing password - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await staffUserService.ResetStaffUserPassword(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ResetPassword: an error occurred resetting staff user password for {model.AccountId} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotResetRequest model)
        {
            try
            {
                var response = await staffUserService.ForgotPassword(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ForgotPassword: an error occurred processing forgotten staff user password for {model?.Email} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("forgot-password-by-username")]
        public async Task<IActionResult> ForgotPasswordByUsername(ForgotResetByUsernameRequest model)
        {
            try
            {
                var response = await staffUserService.ForgotPasswordBuUsername(model);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.ForgotPassword: an error occurred processing forgotten staff user password for {model?.Username} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("block-all")]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockAllTenantUsers([FromBody] BlockAllUsersRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await staffUserService.BlockAllTenantUsers(request);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"StaffUserController.BlockAllTenantUsers: an error occurred while blocking all tenant users for {request.TenantPlatformMapGuid} - {ex}");
                return StatusCode(500);
            }
        }
    }
}
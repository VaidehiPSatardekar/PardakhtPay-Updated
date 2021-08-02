using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Application.StaffUser;

namespace Pardakht.UserManagement.Web.RestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {
        private ILogger<SystemUserController> logger;
        private IStaffUserService staffUserService;
        public SystemUserController(ILogger<SystemUserController> logger, IStaffUserService staffUserService)
        {
            this.logger = logger;
            this.staffUserService = staffUserService;
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

                var response = await staffUserService.GetSystemUserList(platformGuid, string.Empty);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"SystemUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} - {ex}");
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

                var response = await staffUserService.GetSystemUserList(platformGuid, tenantGuid);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"SystemUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} for tenant mapping {tenantGuid} - {ex}");
                return StatusCode(500);
            }
        }
    }
}

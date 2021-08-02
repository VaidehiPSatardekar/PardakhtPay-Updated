using System;
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
    public class AffiliateUserController : ControllerBase
    {
        private readonly IStaffUserService staffUserService;
        private readonly ILogger<StaffUserController> logger;

        public AffiliateUserController(IStaffUserService staffUserService)
        {
            this.staffUserService = staffUserService;
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
                logger.LogError($"AffiliateUserController.Post: an error occurred creating a new affiliate user for {model.Username} - {ex}");
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

                var response = await staffUserService.GetAffiliateList(platformGuid, string.Empty);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"AffiliateUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} - {ex}");
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

                var response = await staffUserService.GetAffiliateList(platformGuid, tenantGuid);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                logger.LogError($"AffiliateUserController.GetListByPlatformGuid: an error occurred getting user list for {platformGuid} for tenant mapping {tenantGuid} - {ex}");
                return StatusCode(500);
            }
        }
    }
}

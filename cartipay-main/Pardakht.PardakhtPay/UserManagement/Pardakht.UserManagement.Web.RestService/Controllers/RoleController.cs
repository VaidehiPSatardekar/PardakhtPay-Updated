using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Application.Role;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Web.RestService.Controllers
{
    [Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly IRoleService roleService;
        private readonly ILogger<RoleController> logger;


        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            this.roleService = roleService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{platformGuid}")]
        public async Task<IActionResult> Get(string platformGuid)
        {
            try
            {
                var response = await roleService.GetRoles(platformGuid, string.Empty);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"RoleController.Get: an error occurred geting roles for platform {platformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{platformGuid}/{tenantGuid}")]
        public async Task<IActionResult> Get(string platformGuid, string tenantGuid)
        {
            try
            {
                var response = await roleService.GetRoles(platformGuid, tenantGuid);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"RoleController.Get: an error occurred geting roles for platform {platformGuid}, tenant {tenantGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("get-permission-groups/{platformGuid}")]
        public IActionResult GetPermissionGroups(string platformGuid)
        {
            try
            {
                var response = roleService.GetPermissionsGroups(platformGuid);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"RoleController.GetPermissionGroups: an error occurred geting permission groups for platform {platformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] RoleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await roleService.UpdateRole(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"RoleController.Put: an error occurred updating role {model.Name} for platform {model.PlatformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] RoleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }

                var response = await roleService.AddRole(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"RoleController.Post: an error occurred adding role {model.Name} for platform {model.PlatformGuid} - {ex}");
                return StatusCode(500);
            }
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> Get()
        //{
        //    try
        //    {
        //        var response = await _roleService.GetList();
        //        if (response.Success)
        //            return Ok(response.Payload);

        //        return BadRequest(response.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Get");
        //        return StatusCode(500);
        //    }

        //}

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var response = await _roleService.GetDetails(id);
        //        if (response.Success)
        //            return Ok(response.Payload);

        //        return BadRequest(response.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Get/id");
        //        return StatusCode(500);
        //    }

        //}

        //[HttpGet]
        //[Route("get-admin-role/{tenantGuid}")]
        //public async Task<IActionResult> GetAdminRole(string tenantGuid)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var response = await _roleService.GetDetails(id);
        //        if (response.Success)
        //            return Ok(response.Payload);

        //        return BadRequest(response.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Get/id");
        //        return StatusCode(500);
        //    }

        //}

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] RoleDto model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var response = await _roleService.Create(model);

        //        if (!response.Success)
        //            return BadRequest(response.Message);

        //        return Ok(response.Payload);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Post");
        //        return StatusCode(500);
        //    }

        //}


    }
}

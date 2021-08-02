using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages account operations
    /// </summary>
    //[Route("api/account")][
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : ControllerBase
    {
        //AuthenticationConfiguration _Configuration = null;
        //CurrentUser _CurrentUser = null;
        IAccountService _AccountService;
        private readonly IGenericManagementFunctions<UserManagementSettings> _UserManagementFunctions;
        ILogger _Logger = null;
        PlatformInformationSettings _PlatformInformationSettings = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="accountService"></param>
        public AccountController(
            //IOptions<AuthenticationConfiguration> configuration,
            IAccountService accountService,
                                        IGenericManagementFunctions<UserManagementSettings> userManagementFunctions,
            ILogger<AccountController> logger,
            IOptions<PlatformInformationSettings> platformInformationSettingOptions
            //CurrentUser currentUser
            )
        {
            //_Configuration = configuration.Value;
            //_CurrentUser = currentUser;
            _AccountService = accountService;
            _UserManagementFunctions = userManagementFunctions;
            _Logger = logger;
            _PlatformInformationSettings = platformInformationSettingOptions.Value;
        }

        ///// <summary>
        ///// Logins to the system.
        ///// </summary>
        ///// <description>
        ///// Logins to the system
        ///// </description>
        ///// <remarks>
        ///// Sample request
        ///// 
        ///// POST /api/account/login
        ///// {
        /////     "username": "username",
        /////     "password": "password"
        ///// }
        ///// </remarks>
        ///// <param name="model"></param>
        ///// <returns>Token</returns>
        ///// <response code="200">Returns access token</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpPost("login")]
        //[ProducesResponseType(200, Type = typeof(string))]
        //public async Task<IActionResult> Login(LoginModel model)
        //{
        //    try
        //    {
        //        var response = await _AccountService.Login(model);

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                access_token = response.Payload
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        [Route("api/staffuser/staff-user-login")]
        public async Task<IActionResult> UserLogin(object request)
        {
            try
            {
                Request.Headers.Add("UserData", JsonConvert.SerializeObject(new
                {
                    PlatformGuid = _PlatformInformationSettings.PlatformGuid
                }));
                var result =  await _UserManagementFunctions.GenericRequest(request, null, Request);

                return result;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"User Login Error - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [HttpDelete]
        [Authorize]
        [Route("api/staffuser/{*remaining}")]
        public async Task<IActionResult> StaffUserProxy()
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("StaffUserProxy Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet]
        [HttpDelete]
        [Authorize]
        [Route("api/Account/{*remaining}")]
        public async Task<IActionResult> HttpGetDelete()
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("HttpGetDelete Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [HttpPost]
        [Authorize]
        [Route("api/Account/{*remaining}")]
        public async Task<IActionResult> HttpPostPut([FromBody] object request)
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("HttpPostPut Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [HttpDelete]
        [Authorize]
        [Route("api/role/{*remaining}")]
        public async Task<IActionResult> RoleProxy()
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("RoleProxy Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [HttpPost]
        [Authorize]
        [Route("api/role/{*remaining}")]
        public async Task<IActionResult> RoleProxy([FromBody] object request)
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("HttpPostPut Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [HttpPost]
        [Authorize]
        [Route("api/staffuser/{*remaining}")]
        public async Task<IActionResult> StaffUserProxy([FromBody] object request)
        {
            try
            {
                return await _UserManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                _Logger.LogError("HttpPostPut Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        ///// <summary>
        ///// Changes password of a user
        ///// </summary>
        ///// <description>
        ///// Changes password of a user
        ///// </description>
        ///// <remarks>
        ///// Sample Request
        ///// 
        /////     POST/api/account/change-password
        /////     {
        /////         "oldPassword" : "abcd",
        /////         "newPassword" : "1234"
        /////     }
        ///// </remarks>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// <response code="200">Returns success message if the operation is success</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpPost("api/account/change-password")]
        //[Authorize]
        //public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        //{
        //    try
        //    {
        //        var response = await _AccountService.ChangePassword(Request.Headers["Authorization"].ToString(), Request.Headers["Origin"].ToString(), model);

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Gets users for current tenant
        ///// </summary>
        ///// <description>
        ///// Gets users for current tenant
        ///// </description>
        ///// <returns>Users</returns>
        ///// <response code="200">Returns user list</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpGet("api/account")]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<UserDTO>))]
        //[Authorize]
        //public async Task<IActionResult> GetUsers()
        //{
        //    try
        //    {

        //        var response = await _AccountService.GetUsers(Request.Headers["Authorization"].ToString(), Request.Headers["Origin"].ToString());

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok(response.Payload);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Returns a user with given id
        ///// </summary>
        ///// <description>
        ///// Returns a user with given id
        ///// </description>
        ///// <param name="id"></param>
        ///// <returns>User</returns>
        ///// <response code="200">Returns user</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpGet("api/account/{id}")]
        //[ProducesResponseType(200, Type = typeof(UserDTO))]
        //[Authorize]
        //public async Task<IActionResult> GetUser(int id)
        //{
        //    try
        //    {
        //        var response = await _AccountService.GetUser(Request.Headers["Authorization"].ToString(), Request.Headers["Origin"].ToString(), id);

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok(response.Payload);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Creates a new user
        ///// </summary>
        ///// <description>
        ///// Creates a new user
        ///// </description>
        ///// <remarks>
        ///// Sample Request
        ///// 
        /////     POST /api/account
        /////     {
        /////         "username": "username",
        /////         "password": "password",
        /////         "firstName": "firsatName",
        /////         "lastName": "lastName",
        /////         "email": "Email",
        /////         "isApiKey": false,
        /////         "tenantDomainPlatformMapGuid": "guid"
        /////     }
        ///// </remarks>
        ///// <param name="model"></param>
        ///// <returns>Returns created user</returns>
        ///// <response code="200">Returns the user which is created</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpPost("api/account")]
        //[ProducesResponseType(200, Type = typeof(UserDTO))]
        //[Authorize]
        //public async Task<IActionResult> Post([FromBody]UserCreateDTO model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var response = await _AccountService.Create(Request.Headers["Authorization"].ToString(), Request.Headers["Origin"].ToString(), model);

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok(response.Payload);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Updates current user
        ///// </summary>
        ///// <description>
        ///// Updates current user
        ///// </description>
        ///// <remarks>
        /////     PUT /api/account/id
        /////     {
        /////         "id": 1,
        /////         "username": "total",
        /////         "firstName": "firsatName",
        /////         "lastName": "lastName",
        /////         "email": "email",
        /////         "isApiKey": false,
        /////         "tenantDomainPlatformMapGuid": "guid"
        /////     }
        ///// </remarks>
        ///// <param name="id">Id of the user</param>
        ///// <param name="model"></param>
        ///// <returns>Returns updated user</returns>
        ///// <response code="200">Returns the user which is created</response>
        ///// <response code="400">If parameters are invalid</response>
        ///// <response code="500">if there is an internal server error</response>
        //[HttpPut("api/account/{id}")]
        //[ProducesResponseType(200, Type = typeof(UserDTO))]
        //[Authorize]
        //public async Task<IActionResult> Put(int id, [FromBody]UserUpdateDTO model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        if (id != model.Id)
        //        {
        //            return BadRequest("Id mismatch");
        //        }

        //        var response = await _AccountService.Update(Request.Headers["Authorization"].ToString(), Request.Headers["Origin"].ToString(), model);

        //        if (!response.Success)
        //        {
        //            return BadRequest(response.Message);
        //        }
        //        else
        //        {
        //            return Ok(response.Payload);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}
    }
}
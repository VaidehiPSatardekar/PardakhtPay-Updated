using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages bank login operations
    /// </summary>
    [Route("api/banklogin")]
    [ApiController]
  //  [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BankLoginController : ControllerBase
    {
        //IBankBotService _BankBotService = null;
        ILogger<BankLoginController> _Logger = null;
        IOwnerBankLoginService _OwnerBankLoginService = null;
        CurrentUser _CurrentUser = null;
        

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="ownerBankLoginService"></param>
        /// <param name="currentUser"></param>
        /// <param name="logger"></param>
        public BankLoginController(
            IOwnerBankLoginService ownerBankLoginService,
            CurrentUser currentUser,
            ILogger<BankLoginController> logger
            )
        {
            _OwnerBankLoginService = ownerBankLoginService;
            _CurrentUser = currentUser;
            _Logger = logger;           
        }

        /// <summary>
        /// Returns login list
        /// </summary>
        /// <description>
        /// Returns login list
        /// </description>
        /// <remarks>
        /// 
        ///     Sample Request
        ///     
        ///         GET /api/banklogin/getloginlist
        /// </remarks>
        /// <returns>Login list. List of <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getloginlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotLoginSelect>))]
        [Authorize]
        public async Task<IActionResult> GetLoginList(bool includeDeleteds = false)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetLoginSelect(includeDeleteds);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns login list (Including login requests)
        /// </summary>
        /// <description>
        /// Returns login list (Including login requests)
        /// </description>
        /// <remarks>
        /// 
        ///     Sample Request
        ///     
        ///         GET /api/banklogin/getownerloginlist
        /// </remarks>
        /// <returns>Login list. List of <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getownerloginlist")]
        [Authorize]
        public async Task<IActionResult> GetOwnerLoginList()
        {
            try
            {
                var response = await _OwnerBankLoginService.GetOwnerLoginList();

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns account list with given username
        /// </summary>
        /// <description>
        /// Returns account list with given username
        /// </description>
        /// <remarks>
        ///  Sample Request
        ///     GET /api/banklogin/getaccountlistbyusername
        ///     {
        ///         "username": "total"
        ///     }
        /// </remarks>
        /// <param name="username">Username of a spesific login information</param>
        /// <returns>List of <see cref="BotAccountInformation"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getaccountlistbyusername")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotAccountInformation>))]
        public async Task<IActionResult> GetAccountListByUsername(string username)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetAccountsByUsernameAsync(username);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns all of the acccounts
        /// </summary>
        /// <description>
        /// Returns all of the accounts
        /// </description>
        /// <remarks>
        /// Sample Request
        ///     GET /api/banklogin/getaccountlist
        /// </remarks>
        /// <returns></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getaccountlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotAccountInformation>))]
        [Authorize]
        public async Task<IActionResult> GetAccountList(bool includeDeleteds)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetAccountsAsync(includeDeleteds);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns accounts by the login guid
        /// </summary>
        /// <description>
        /// Returns accounts by the login guid
        /// </description>
        /// <param name="loginGuid">Guid of the login</param>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        /// <returns>List of bank accounts. <see cref="BotAccountInformation"/></returns>
        [HttpGet("getaccountlistbyloginguid/{loginGuid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotAccountInformation>))]
        [Authorize]
        public async Task<IActionResult> GetAccountsByLoginGuid(string loginGuid)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetAccountsByLoginGuidAsync(loginGuid);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns unused accounts by the login guid
        /// </summary>
        /// <description>
        /// Returns unused accounts by the login guid
        /// </description>
        /// <param name="loginGuid">Guid of the login</param>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        /// <returns>List of bank accounts. <see cref="BotAccountInformation"/></returns>
        [HttpGet("getunusedaccountlistbyloginguid/{loginGuid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotAccountInformation>))]
        [Authorize]
        public async Task<IActionResult> GetUnusedAccountsByLoginGuid(string loginGuid)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetUnusedAccountsByLoginGuidAsync(loginGuid);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Creates new bank login request
        /// </summary>
        /// <description>
        /// Creates new bank login request
        /// </description>
        /// <remarks>
        ///     Sample Request
        ///     
        ///     POST /api/banklogin
        ///     {
        ///         "bankId": 1,
        ///         "username": "total",
        ///         "password": "pass",
        ///         "ownerGuid": "guid".
        ///         "friendlyName" :"my live login",
        ///         "tenantGuid": "guid of my tenant"
        ///     }
        /// </remarks>
        /// <param name="item">Login request parameters. <see cref="BotLoginCreateDTO"/></param>
        /// <returns>Created bank bot login. <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotLoginSelect))]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> Post([FromBody]BotLoginCreateDTO item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.CreateLoginRequest(item);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns the bank login with given id
        /// </summary>
        /// <description>
        /// Returns the bank login with given id
        /// </description>
        /// <param name="id"></param>
        /// <returns>Login with parmeter. <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotLoginSelect))]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetLoginById(id);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured");
            }
        }

        /// <summary>
        /// Updates login record
        /// </summary>
        /// <description>
        /// Updates login record
        /// </description>
        /// <remarks>
        ///     Sample Request
        ///     PUT /api/banklogin/1
        ///     {
        ///         "friendlyName": "My Live Login"
        ///     }
        /// </remarks>
        /// <param name="id">Id of login</param>
        /// <param name="item">Login update parameters. <see cref="OwnerBankLoginUpdateDTO"/></param>
        /// <returns>Updated login info</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OwnerBankLoginUpdateDTO))]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> Put(int id, [FromBody]OwnerBankLoginUpdateDTO item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.UpdateAsync(item);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured");
            }
        }

        /// <summary>
        /// Returns bank informations
        /// </summary>
        /// <description>
        /// Returns bank informations
        /// </description>
        /// <returns>List of <see cref="BotBankInformation"/></returns>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getbanks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BotBankInformation>))]
        [Authorize]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                var response = await _OwnerBankLoginService.GetBanks();

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured");
            }
        }

        /// <summary>
        /// Called from bank bot when the bank login information is changed.
        /// </summary>
        /// <param name="request">Login informations. <see cref="BankBotLoginStatus"/></param>
        /// <returns>Returns http status code only</returns>
        [HttpPost("loginchanged")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> UpdateLoginStatus(BankBotLoginStatus request)
        {
            try
            {
                //_CurrentUser.ApiCall = true;
                var response = await _OwnerBankLoginService.UpdateLoginStatus(request);
                
                if (response.Success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Updates a login information
        /// </summary>
        /// <description>
        /// Updates a login information
        /// </description>
        /// <param name="id">Id of the login</param>
        /// <param name="login">Parameters. <see cref="BankBotUpdateLoginDTO"/></param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("updatelogininformation/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> UpdateLoginInformation(int id, BankBotUpdateLoginDTO login)
        {
            try
            {
                if(id != login.Id)
                {
                    return BadRequest("Id and entity id are different");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.UpdateLoginAsync(login);

                if (response.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Gets the password of a login
        /// </summary>
        /// <description>
        /// Gets the password of a login
        /// </description>
        /// <param name="id">Id of the login</param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("showpassword/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> ShowPassword(int id)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetPassword(id);

                if (response.Success)
                {
                    return Ok(new { password = response.Payload });
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// It is called from the bank bot when a login request status is changed.
        /// </summary>
        /// <description>
        /// It is called from the bank bot when a login reqeust status is chagned.
        /// </description>
        /// <param name="model">Request model. <see cref="BankBotLoginRequestChangedDTO"/></param>
        /// <returns>Status Code</returns>
        [HttpPost("loginrequestchanged")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> LoginRequestChanged([FromBody]BankBotLoginRequestChangedDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _OwnerBankLoginService.LoginRequestChanged(model);

                if (!response.Success)
                {
                    _Logger.LogError(response.Message);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Creates new bank login from login request
        /// </summary>
        /// <description>
        /// Creates new bank login from login request
        /// </description>
        /// <param name="model">Request model. <see cref="CreateLoginFromLoginRequestDTO"/></param>
        /// <returns>Status code</returns>
        [HttpPost("createloginfromloginrequest")]
        [Authorize]
        public async Task<IActionResult> CreateLoginFromLoginRequest([FromBody]CreateLoginFromLoginRequestDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.CreateLoginFromLoginRequest(model);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {

                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        [HttpPost("deletelogininformation/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> DeleteLoginInformation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.DeleteLogin(id);

                if (response.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        [HttpPost("deactivatelogininformation/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> DeactivateLoginInformation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.DeactivateLogin(id);

                if (response.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        [HttpPost("activatelogininformation/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> ActivateLoginInformation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.ActivateLogin(id);

                if (response.Success)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Called from bank bot when the bank login information is changed.
        /// </summary>
        /// <param name="request">Login informations. <see cref="BankBotLoginStatus"/></param>
        /// <returns>Returns http status code only</returns>
        [HttpPost("accountstatuschanged")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> UpdateAccountStatus(AccountStatusChangedDTO request)
        {
            try
            {
                //_CurrentUser.ApiCall = true;
                var response = await _OwnerBankLoginService.AccountStatusChanged(request);

                if (response.Success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        [HttpGet("getblockedcarddetails/{accountGuid}")]
        [Authorize]
        public async Task<IActionResult> GetBlockedCardDetails(string accountGuid)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetBlockedCardDetails(accountGuid);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured");
            }
        }

        [HttpGet("getcardnumber/{loginId}")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> GetCardNumber(int loginId)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetCardNumber(loginId);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates new qr register request
        /// </summary>
        /// <description>
        /// Creates new qr register request
        /// </description>
        /// <param name="loginId">QR register request parameters.</param>
        /// <returns>Login registered. <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("createqrregister/{loginId}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> CreateQRRegisterRequest(int loginId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response =  await _OwnerBankLoginService.CreateQRRegisterRequest(loginId);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Returns the qr registration details with given id
        /// </summary>
        /// <description>
        /// Returns the qr registration details with given id
        /// </description>
        /// <param name="id"></param>
        /// <returns>Login with parmeter. <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getqrregistrationdetails/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BotLoginSelect))]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> GetQRRegistrationDetails(int id)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetQRRegistrationDetailsByLoginId(id);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured");
            }
        }


        /// <summary>
        /// Register QR Code
        /// </summary>
        /// <description>
        /// Register QR Code
        /// </description>
        /// <param name="loginId">QR register request parameters.</param>
        /// <returns>Login registered. <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("registerqrcode")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> RegisterQRCode(QrCodeRegistrationRequest qRRegisterLoginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.RegisterQRCode(qRRegisterLoginDTO);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Gets the device status of login
        /// </summary>
        /// <description>
        /// Gets the device status of login
        /// </description>
        /// <param name="id">Id of the login</param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("showlogindevicestatus/{loginId}")]
        [Authorize(Roles = "CP-LOGIN-DEVICE-STATUS-SETTING")]
        public async Task<IActionResult> ShowLoginDeviceStatus(string loginId)
        {
            try
            {
                var response = await _OwnerBankLoginService.GetLoginDeviceStatusByLoginId(loginId);

                if (response.Success)
                {
                    return Ok(new { loginDeviceStatusDesc = response.Payload });
                }
                else
                {
                    return Ok(new { loginDeviceStatusDesc = response.Message });
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Gets the device status of login list
        /// </summary>
        /// <description>
        /// Gets the device status of login list
        /// </description>
        /// <param name="id">Id of the login</param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("getloginlistdevicestatus")]
        [Authorize(Roles = "CP-LOGIN-DEVICE-STATUS-SETTING")]
        public async Task<IActionResult> GetoginListDeviceStatus()
        {
            try
            {
                var response = await _OwnerBankLoginService.GetDeviceStatusOfLogins();

                if (response.Success)
                {
                    return Ok(new { loginDeviceStatusDesc = response.Payload });
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Returns login list with device status
        /// </summary>
        /// <description>
        /// Returns login list with device status
        /// </description>
        /// <remarks>
        /// 
        ///     Sample Request
        ///     
        ///         GET /api/banklogin/getownerloginlistwithdevicestatus
        /// </remarks>
        /// <returns>Login list. List of <see cref="BotLoginSelect"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("getownerloginlistwithdevicestatus")]
        [Authorize(Roles = "CP-BANK-LOGINS")]
        public async Task<IActionResult> GetOwnerLoginListWithDeviceStatus()
        {
            try
            {
                var response = await _OwnerBankLoginService.GetOwnerLoginListWithDeviceStatus();

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                return Ok(response.Payload);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled Exception Occured");
            }
        }

        /// <summary>
        /// Gets the OTP of device
        /// </summary>
        /// <description>
        /// Gets the OTP of device
        /// </description>
        /// <param name="bankLoginId">Id of the login</param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("getotp/{bankLoginId}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> GetOTP(int bankLoginId)
        {
            try
            {
                var response = await _OwnerBankLoginService.GenerateOTPForRegisteredDevice(bankLoginId);

                if (response.Success)
                {
                    return Ok(new { otp = response.Payload });
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Register login request
        /// </summary>
        /// <description>
        /// Register login request
        /// </description>
        /// <param name="model">Request model. <see cref="BotLoginCreateDTO"/></param>
        /// <returns>Status code</returns>
        [HttpPost("registerloginrequest")]
        [Authorize]
        public async Task<IActionResult> RegisterLoginRequest([FromBody]RegisterLogin model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _OwnerBankLoginService.RegisterLoginRequest(model);

                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {

                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }

        /// <summary>
        /// Switch bank connection program
        /// </summary>
        /// <description>
        /// Switch bank connection program
        /// </description>
        /// <param name="id">Id of the login</param>
        /// <returns>True if the operation is success</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("switchbankconnectionprogram/{id}")]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> SwitchBankConnectionProgram(int id)
        {
            try
            {
                var response = await _OwnerBankLoginService.SwitchBankConnectionProgram(id);

                if (response.Success)
                {
                    return Ok(new { bankConnectionProgram = response.Payload });
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }


        [HttpPost("clearcache")]
        [Authorize]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                await _OwnerBankLoginService.ClearCache();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhnandled exception occured");
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/applicationsettings")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ApplicationSettingsController : ControllerBase
    {
        IApplicationSettingService _ApplicationSettingService;
        IBankBotService _BankBotService = null;
        ILogger _Logger = null;

        public ApplicationSettingsController(IApplicationSettingService applicationSettingService,
            IBankBotService bankBotService,
            ILogger<ApplicationSettingsController> logger)
        {
            _ApplicationSettingService = applicationSettingService;
            _BankBotService = bankBotService;
            _Logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "CP-APPLICATION-SETTINGS")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _ApplicationSettingService.GetAllSettings();

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Authorize(Roles = "CP-APPLICATION-SETTINGS")]
        public async Task<IActionResult> Put([FromBody]ApplicationSettingsDTO model)
        {
            try
            {
                var response = await _ApplicationSettingService.Update(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("gettransferstatuslist")]
        [Authorize]
        public async Task<IActionResult> GetTransferStatusList()
        {
            try
            {
                var response = await _BankBotService.GetTransferStatusList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

     
    }
}
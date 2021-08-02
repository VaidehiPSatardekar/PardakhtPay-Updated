using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TenantManagementController : ControllerBase
    {
        private readonly ILogger<TenantManagementController> logger;
        private readonly IGenericManagementFunctions<TenantManagementSettings> tenantManagementFunctions;

        public TenantManagementController(IGenericManagementFunctions<TenantManagementSettings> tenantManagementFunctions, ILogger<TenantManagementController> logger)
        {
            this.tenantManagementFunctions = tenantManagementFunctions;
            this.logger = logger;
        }

        [HttpGet]
        [HttpDelete]
        [Authorize]
        [Route("api/tenantPlatform/{*remaining}")]
        public async Task<IActionResult> HttpGetDelete()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.HttpGetDelete: an error occurred - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [HttpDelete]
        [Authorize]
        [Route("api/tenant/{*remaining}")]
        public async Task<IActionResult> TenantGetDelete()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.TenantGetDelete: an error occurred - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/tenantPlatform/{*remaining}")]
        public async Task<IActionResult> HttpPost([FromBody] object request, string remaining)
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.HttpPost: an error occurred creating tenant - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("api/tenantPlatform/{*remaining}")]
        public async Task<IActionResult> HttpPut([FromBody] object request, string remaining)
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.HttpPut: an error occurred updating tenant - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/currency")]
        public async Task<IActionResult> GetCurrencies()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.GetCurrencies: an error occurred getting currencies - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/language")]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.GetLanguages: an error occurred getting languages - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/country")]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.GetCountries: an error occurred getting countries - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/timezone")]
        public async Task<IActionResult> GetTimeZones()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.GetTimeZones: an error occurred getting timezones - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("api/platform/{platformGuid}")]
        public async Task<IActionResult> PlatformGet()
        {
            try
            {
                return await tenantManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"TenantManagementController.PlatformGet: an error occurred - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

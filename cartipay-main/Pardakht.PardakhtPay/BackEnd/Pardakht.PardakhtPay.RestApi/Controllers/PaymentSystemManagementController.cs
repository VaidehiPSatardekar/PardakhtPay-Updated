using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentSystemManagementController : ControllerBase
    {
        private readonly ILogger<PaymentSystemManagementController> logger;
        private readonly IGenericManagementFunctions<PaymentManagementSettings> paymentManagementFunctions;

        public PaymentSystemManagementController(IGenericManagementFunctions<PaymentManagementSettings> paymentManagementFunctions, ILogger<PaymentSystemManagementController> logger)
        {
            this.paymentManagementFunctions = paymentManagementFunctions;
            this.logger = logger;
        }

        [HttpGet]
        [HttpDelete]
        [Route("api/PaymentSetting/{*remaining}")]
        public async Task<IActionResult> HttpGetDelete()
        {
            try
            {
                return await paymentManagementFunctions.GenericRequest(null, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"PaymentSystemManagementController.HttpGetDelete: an error occurred - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [HttpPut]
        [Route("api/PaymentSetting/{*remaining}")]
        public async Task<IActionResult> HttpPostTicket([FromBody] object request, string remaining)
        {
            try
            {
                return await paymentManagementFunctions.GenericRequest(request, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"PaymentSystemManagementController.HttpPostPut: an error occurred - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("api/PaymentSetting/upload")]
        public async Task<IActionResult> HttpPostFile()
        {
            try
            {
                if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                {
                    return null;
                }

                var file = Request.Form.Files[0];
                return await paymentManagementFunctions.GenericRequest(file, User, Request);
            }
            catch (Exception ex)
            {
                logger.LogError($"PaymentSystemManagementController.HttpPost: an error occurred uploading file - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
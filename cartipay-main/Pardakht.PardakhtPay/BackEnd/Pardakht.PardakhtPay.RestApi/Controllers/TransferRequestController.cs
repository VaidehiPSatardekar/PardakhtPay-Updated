using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/transferrequest")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TransferRequestController : ControllerBase
    {
        ITransferRequestService _Service = null;
        ILogger _Logger = null;

        public TransferRequestController(ITransferRequestService service,
            ILogger<TransferRequestController> logger)
        {
            _Service = service;
            _Logger = logger;
        }


        [HttpPost("updatetransactionstatus")]
        [ApiCall]
        [Authorize]
        public async Task<IActionResult> UpdateTransactionStatus([FromBody]TransferRequestResponse model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _Service.UpdateTransactionStatus(model);

                if (response.Success)
                {
                    return Ok();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
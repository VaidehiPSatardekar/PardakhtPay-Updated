using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/withdrawal")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WithdrawalController : PardakhtPayBaseController
    {
        IWithdrawalService _Service = null;

        public WithdrawalController(IWithdrawalService service, ILogger<WithdrawalController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpPost("search")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> Search(WithdrawalSearchArgs args)
        {
            try
            {
                Random rnd = new Random();
                var response = await _Service.Search(args);

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
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception");
            }
        }

        [HttpPost("withdrawalcheckcompleted/{id}")]
        [Authorize]
        [ApiCall]
        public async Task<HttpStatusCode?> WithdrawalCheckCompleted(int id)
        {
            try
            {
                var result = await _Service.SendCallback(id);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return null;
            }
        }

        [HttpPost("gettransferreceipt/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTransferReceipt(int id)
        {
            try
            {
                var result = await _Service.GetTransferReceipt(id);

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                string fileType = result.Payload.ContentType == "application/pdf" ? ".pdf" : ".png";
                Response.Headers.Add("File-Name", $"{id}_{result.Payload.TrackingNumber}_{DateTime.Now.ToString("ddMMyyyHHmmss")}{fileType}");

                return File(result.Payload.Data, result.Payload.ContentType, $"{id}_{Guid.NewGuid().ToString()}{fileType}");
                ////Response.Headers.Add("File-Name", $"{id}_{result.Payload.TrackingNumber}_{DateTime.Now.ToString("ddMMyyyHHmmss")}.pdf");
                ////return File(result.Payload.Data, result.Payload.ContentType, $"{id}_{Guid.NewGuid().ToString()}.pdf");
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("cancel/{id}")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _Service.Cancel(id);

                if (result.Success)
                {
                    return Ok(result.Payload);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("retry/{id}")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> Retry(int id)
        {
            try
            {
                var result = await _Service.Retry(id);

                if (result.Success)
                {
                    return Ok(result.Payload);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("setascompleted/{id}")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> SetAsCompleted(int id)
        {
            try
            {
                var result = await _Service.SetAsCompleted(id);

                if (result.Success)
                {
                    return Ok(result.Payload);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("changeprocesstype/{id}/{processType}")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> ChangeProcessType(int id, int processType)
        {
            try
            {
                var result = await _Service.ChangeProcessType(id, processType);

                if (result.Success)
                {
                    return Ok(result.Payload);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("changeallprocesstype/{processType}")]
        [Authorize(Roles = "CP-WITHDRAWALS")]
        public async Task<IActionResult> ChangeAllProcessType([FromBody]WithdrawalSearchArgs args, int processType)
        {
            try
            {
                var result = await _Service.ChangeProcessType(args, processType);

                if (result.Success)
                {
                    return Ok();
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("refund/{transferRequestId}")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> Refund(int transferRequestId)
        {
            try
            {
                var result = await _Service.CheckRefund(transferRequestId);

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpGet("gethistory/{id}")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> GetHistory(int id)
        {
            try
            {
                var result = await _Service.GetWithdrawalHistories(id);

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return Ok(result.Payload);
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        [HttpPost("withdrawalcallbacktomerchant/{id}")]
        [Authorize(Roles = "CP-WITHDRAWALS-CALLBACK-MERCHANT")]
        public async Task<IActionResult> WithdrawalCallbackToMerchant(int id)
        {
            try
            {
                var result = await _Service.WithdrawalCallbackToMerchant(id);

                if (result.Success)
                {
                    return Ok(new { callbackToMerchant = result.Payload });
                }

                return BadRequest("Error in withdrawal call back to merchant.");
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        private IActionResult ReturnUnhandledException(Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new TransactionResponse((int)TransactionResultEnum.UnknownError));
        }
    }
}
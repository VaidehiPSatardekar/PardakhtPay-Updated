using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages withdraw request
    /// </summary>
    [Route("withdraw")]
    [ApiCall]
    public class WithdrawController : Controller
    {
        ILogger _Logger = null;
        IWithdrawalService _Service = null;

        /// <summary>
        /// Initialize a new instance of this controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public WithdrawController(ILogger<WithdrawController> logger,
            IWithdrawalService service)
        {
            _Logger = logger;
            _Service = service;
        }

        /// <summary>
        /// Creates a new withdrawal request.
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     
        ///     api_key = daddfefefefefe9089034fdsfds
        ///     amount = 50000
        ///     first_name = test
        ///     last_name = test
        ///     account_number = 09494044844
        ///     iban = IR870513446259311169774525
        ///     user_id = test
        ///     website_name = test
        ///     return_url = http://returnurl.com
        ///     priority = 1 (1 = Low, 2 = Normal, 3 = High)
        ///     transfer_time = 28.03.2019 14:30
        ///     reference = test123 (Reference must be unique)
        ///     
        /// Success Response : 
        ///     withdrawal_id = 5,
        ///     status = 1
        ///     
        /// Error Response
        ///     status = 0
        ///     errorCode = 201
        ///     errorDescripton = "Account Not Found"
        ///     
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WithdrawSuccessResponse))]
        //[NonUI]
        public async Task<IActionResult> Request([FromForm]WithdrawRequestDTO model)
        {
            _Logger.LogInformation($"Withdraw request {JsonConvert.SerializeObject(model)}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Service.CreateWithdrawRequest(model);

            if (result.Result != WithdrawRequestResult.Success)
            {
                return Ok(new WithdrawErrorResponse
                {
                    status = 0,
                    errorCode = (int)result.Result,
                    errorDescription = result.ResultDescription
                });
            }

            return Ok(new WithdrawSuccessResponse
            {
                status = 1,
                withdrawal_id = result.Id
            });
        }

        /// <summary>
        /// Checks the status of the withdrawal
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     https://apiadress.com/check/1(withdrawal_id)
        ///     send api_key from body
        ///     
        /// Success Response : 
        ///     withdrawal_id = 5,
        ///     status = 1
        ///     transfer_notes = 12345,
        ///     tracking_number = 849849489844,
        ///     reference : test123,
        ///     amount : 50000
        ///     
        /// Error Response
        ///     status = 0
        ///     errorCode = 201
        ///     errorDescripton = "Not Completed"
        ///     
        /// </remarks>
        /// <param name="withdrawal_id"></param>
        /// <param name="api_key"></param>
        /// <returns></returns>
        [HttpPost("check/{withdrawal_id}")]
        //[NonUIAttribute]
        public async Task<IActionResult> Check(int withdrawal_id, [FromForm]string api_key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Service.Check(new WithdrawCheckRequest()
            {
                api_key = api_key,
                withdrawal_id = withdrawal_id
            });

            if(result.Result == WithdrawRequestResult.Success)
            {
                return Ok(new WithdrawCompletedResponse()
                {
                    status = 1,
                    transfer_notes = result.TransferNotes,
                    tracking_number = result.TrackingNumber,
                    reference = result.Reference,
                    withdrawal_id = result.Id,
                    amount = Convert.ToInt32(result.Amount),
                    payments = result.Payments
                });
            }
            else
            {
                return Ok(new WithdrawErrorResponse()
                {
                    status = 0,
                    errorCode = (int)result.Result,
                    errorDescription = result.ResultDescription,
                    amount = Convert.ToInt32(result.Amount),
                    reference = result.Reference,
                    payments = result.Payments
                });
            }
        }

        /// <summary>
        /// Cancels the withdrawal request
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     https://apiadress.com/cancel/1(withdrawal_id)
        ///     send api_key from body
        ///     
        /// Success Response : 
        ///     withdrawal_id = 5,
        ///     status = 1,
        ///     reference : test123,
        ///     amount : 50000
        ///     
        /// Error Response
        ///     status = 0
        ///     errorCode = 201
        ///     errorDescripton = "Not Completed"
        ///     
        /// </remarks>
        /// <param name="withdrawal_id"></param>
        /// <param name="api_key"></param>
        /// <returns></returns>
        [HttpPost("cancel/{withdrawal_id}")]
        //[NonUIAttribute]
        public async Task<IActionResult> Cancel(int withdrawal_id, [FromForm]string api_key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Service.Cancel(new WithdrawCancelRequest()
            {
                api_key = api_key,
                withdrawal_id = withdrawal_id
            });

            if (result.Result == WithdrawRequestResult.Success)
            {
                return Ok(new WithdrawCompletedResponse()
                {
                    status = 1,
                    withdrawal_id = result.Id,
                    reference = result.Reference,
                    tracking_number = result.TrackingNumber,
                    transfer_notes = result.TransferNotes,
                    amount = Convert.ToInt32(result.Amount)
                });
            }
            else
            {
                return Ok(new WithdrawErrorResponse()
                {
                    status = 0,
                    errorCode = (int)result.Result,
                    errorDescription = result.ResultDescription
                });
            }
        }

        /// <summary>
        /// Depending on the amount passed, Cartpay will compare range of withdrawal remaining amounts and will suggest possible list of amounts. If 0 passed as an amount, PardakhtPay will take a max range as 30000000.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost("suggestedamounts")]
        public async Task<IActionResult> SuggestedAmounts([FromForm]string apiKey, [FromForm]long amount)
        {
            try
            {
                var response = await _Service.GetSuggestedWithdrawalAmounts(apiKey, amount);

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

        [HttpPost("suggesteddepositamounts")]
        public async Task<IActionResult> SuggestedDepositAmounts([FromBody]SuggestedDepositAmountRequest model)
        {
            try
            {
                var response = await _Service.GetSuggestedWithdrawalAmounts(model.ApiKey, model.Amount);

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
    }
}
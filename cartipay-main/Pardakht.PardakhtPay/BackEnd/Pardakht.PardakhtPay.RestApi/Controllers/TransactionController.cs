using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages transaction operations
    /// </summary>
    [Route("api/transaction")]
    [ApiController]
    [ActionLog]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TransactionController : Controller
    {
        ITransactionService _TransactionService = null;
        ILogger<TransactionController> _Logger = null;
        CurrentUser _CurrentUser = null;
        IHttpClientFactory _HttpClientFactory = null;
        private readonly int[] _IncompleteStatuses = new int[] { (int)TransactionStatus.Started, (int)TransactionStatus.TokenValidatedFromWebSite, (int)TransactionStatus.WaitingConfirmation };

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="transactionService"></param>
        /// <param name="currentUser"></param>
        /// <param name="logger"></param>
        public TransactionController(ITransactionService transactionService,
            CurrentUser currentUser,
            ILogger<TransactionController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _TransactionService = transactionService;
            _Logger = logger;
            _CurrentUser = currentUser;
            _HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Calls from the Bank Bot web job service when the checking paymnet has completed.
        /// Bank bot web job service doesn't send the status of the transaction. We check from our database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("paymentcheckcompleted")]
        [Authorize]
        public async Task<IActionResult> PaymentCheckCompleted(CheckTransactionRequest request)
        {
            try
            {
                _CurrentUser.ApiCall = true;
                var result = await _TransactionService.Check(request.Token);

                HttpStatusCode? statusCode = null;

                if (string.IsNullOrEmpty(result.ReturnUrl))
                {
                    _Logger.LogCritical($"Payment check completed return url is empty : {JsonConvert.SerializeObject(result)}");
                }
                else
                {
                    statusCode = await SendCallback(result, request.Token);
                }

                int code = (int)HttpStatusCode.InternalServerError;

                if (statusCode.HasValue)
                {
                    code = (int)statusCode.Value;
                }

                return StatusCode(code);
            }
            catch (TransactionException ex)
            {
                _Logger.LogError(ex, ex.Message);

                return BadRequest(new CheckTransactionResponse(ex.Result));
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        /// <summary>
        /// Returns transaction informations
        /// </summary>
        /// <param name="args">Search arguments. <see cref="TransactionSearchArgs"/></param>
        /// <response code="500">if there is an internal server error</response>
        /// <returns>List of transactions</returns>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListSearchResponse<IEnumerable<TransactionSearchDTO>>))]
        [Authorize(Roles = "CP-DEPOSITS")]
        public async Task<IActionResult> Search(TransactionSearchArgs args)
        {
            try
            {
                Random rnd = new Random();
                var response = await _TransactionService.Search(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception");
            }
        }

        [HttpPost("setascompleted/{id}")]
        [Authorize(Roles = "CP-DEPOSITS-COMPLETE")]
        public async Task<IActionResult> SetAsCompleted(int id)
        {
            try
            {
                var response = await _TransactionService.CompleteTransaction(id);

                if (response.Success)
                {
                    return Ok();
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("setasexpired/{id}")]
        [Authorize(Roles = "CP-DEPOSITS")]
        public async Task<IActionResult> SetAsExpired(int id)
        {
            try
            {
                var response = await _TransactionService.ExpireTransaction(id);

                if (response.Success)
                {
                    return Ok();
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Sends callback to payment gateway to inform the status of the transaction
        /// </summary>
        /// <param name="result"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<HttpStatusCode?> SendCallback(TransactionCheckResult result, string token)
        {
            try
            {
                _Logger.LogInformation($"Informing the return url {JsonConvert.SerializeObject(result)} Token:{token}");
                var client = _HttpClientFactory.CreateClient();

                var inProcess = _IncompleteStatuses.Contains(result.Status) ? 1 : 0;

                using (MultipartFormDataContent multi = new MultipartFormDataContent()) {

                    if (result.Result == TransactionResultEnum.Success)
                    {
                        multi.Add(new StringContent(token), "invoice_key");
                        multi.Add(new StringContent("1"), "status");
                        multi.Add(new StringContent(result.BankNumber ?? ""), "bank_code");
                        multi.Add(new StringContent(result.CardNumber ?? ""), "card_number");
                        multi.Add(new StringContent(result.PaymentType.ToString()), "payment_type");
                        multi.Add(new StringContent(result.Amount.ToString()), "amount");
                        multi.Add(new StringContent("false"), "redirect_to_page");
                        multi.Add(new StringContent(inProcess.ToString()), "inProcess");
                    }
                    else
                    {
                        multi.Add(new StringContent(token), "invoice_key");
                        multi.Add(new StringContent("0"), "status");
                        multi.Add(new StringContent(result.CardNumber ?? ""), "card_number");
                        multi.Add(new StringContent(result.Amount.ToString()), "amount");
                        multi.Add(new StringContent(result.ResultCode.ToString()), "errorCode");
                        multi.Add(new StringContent(result.ErrorDescription), "errorDescription");
                        multi.Add(new StringContent(result.PaymentType.ToString()), "payment_type");
                        multi.Add(new StringContent("false"), "redirect_to_page");
                        multi.Add(new StringContent(inProcess.ToString()), "inProcess");
                    }

                    _Logger.LogInformation($"Informing the return url response. Content {JsonConvert.SerializeObject(multi)} Token:{token}");

                    using (var response = await client.PostAsync(result.ReturnUrl, multi))
                    {
                        _Logger.LogInformation($"Informing the payment gateway. Response Code : {response.StatusCode} Token:{token}");

                        return response.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private IActionResult ReturnUnhandledException(Exception ex)
        {
            _Logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new TransactionResponse((int)TransactionResultEnum.UnknownError));
        }

        [HttpPost("transactioncallbacktomerchant/{id}")]
        [Authorize(Roles = "CP-TRANSACTION-CALLBACK-MERCHANT")]
        public async Task<IActionResult> TransactionCallbackToMerchant(int id)
        {
            try
            {
                var result = await _TransactionService.TransactionCallbackToMerchant(id);

                if (result.Success)
                {
                    return Ok(new { callbackToMerchant = result.Payload });
                }

                return BadRequest("Error in transaction call back to merchant.");
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }
    }
}
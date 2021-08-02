using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BotDetect.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.RestApi.Model;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Invoice;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;
using Serilog;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages transaction craete and check operations
    /// </summary>
    [Route("invoice")]
    [ApiCall]
    //[ApiController]
    public class InvoiceController : Controller
    {
        ITransactionService _TransactionService = null;
        ILogger<TransactionController> _Logger = null;
        TransactionConfiguration _Configuration = null;
        private readonly int[] _IncompleteStatuses = new int[] { (int) TransactionStatus.Started, (int) TransactionStatus.TokenValidatedFromWebSite, (int) TransactionStatus.WaitingConfirmation };
        AppSettings _AppSettings = null;
    /// <summary>
    /// Creates a new instance of this class
    /// </summary>
    /// <param name="transactionService"></param>
    /// <param name="currentUser"></param>
    /// <param name="transactionOptions"></param>
    /// <param name="logger"></param>
    /// <param name="settingOptions"></param>
    public InvoiceController(ITransactionService transactionService,
            CurrentUser currentUser,
            IOptions<TransactionConfiguration> transactionOptions,
            ILogger<TransactionController> logger,
            IOptions<AppSettings> settingOptions)
        {
            _TransactionService = transactionService;
            _Logger = logger;
            _Configuration = transactionOptions.Value;
            _AppSettings = settingOptions.Value;
            //currentUser.ApiCall = true;
        }

        /// <summary>
        /// Creates a new transaction and generates a token.
        /// We excpect the api key, amount and the callback url
        /// We save the transaction and set token and the callback url
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     
        ///     api_key = daddfefefefefe9089034fdsfds
        ///     amount = 50000
        ///     return_url = http://returnurl.com
        ///     
        /// Error Codes :
        ///     101 --> Api Key Is Null Or Invalid
        ///     102 --> Amount is equal or less than zero
        ///     103 --> Callback url is empty
        ///     200 --> Token is invalid
        ///     201 --> Deposit hasn't been confirmed yet
        ///     202 --> Deposit is expired or canceled
        ///     204 --> Transaction reversed
        ///     207 --> A valid account wasn't found for a deposit or withdrawal
        /// </remarks>
        ///
        /// <param name="invoiceRequest">Parameters. <see cref="InvoiceRequest"/></param>
        /// <returns>Generated token of the transaction. <see cref="InvoiceCreateResponse"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvoiceCreateResponse))]
        //[NonUIAttribute]
        public async Task<IActionResult> Create([FromForm]InvoiceRequest invoiceRequest)
        {
            _Logger.LogInformation($"Request started. {invoiceRequest.reference}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var request = new CreateTransactionRequest()
                {
                    Amount = invoiceRequest.amount,
                    ApiKey = invoiceRequest.api_key,
                    ReturnUrl = invoiceRequest.return_url,
                    //Description = invoiceRequest.description,
                    UserActivityScore = invoiceRequest.user_activity_score,
                    UserDepositNumber = invoiceRequest.user_deposit_number,
                    UserGroupName = invoiceRequest.user_group_name,
                    UserId = invoiceRequest.user_id,
                    UserTotalDeposit = invoiceRequest.user_total_deposit,
                    UserTotalWithdraw = invoiceRequest.user_total_withdraw,
                    UserWithdrawNumber = invoiceRequest.user_withdraw_number,
                    WebsiteName = invoiceRequest.website_name,
                    Reference = invoiceRequest.reference,
                    UserCasinoNumber = invoiceRequest.user_casino_number,
                    UserSportbookNumber = invoiceRequest.user_sportbook_number,
                    UserTotalCasino = invoiceRequest.user_total_casino,
                    UserTotalSportbook = invoiceRequest.user_total_sportbook,
                    PaymentType = invoiceRequest.payment_type,
                    CustomerCardNumber = invoiceRequest.card_number
                };

                if(DateTime.TryParseExact(invoiceRequest.user_register_date, Helper.DateTimeFormats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime registerDate))
                {
                    request.UserRegisterDate = registerDate;
                }

                if (DateTime.TryParseExact(invoiceRequest.user_last_activity, Helper.DateTimeFormats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime activityDate))
                {
                    request.UserLastActivity = activityDate;
                }

                var ipAddress = HttpContext.GetIpAddress();
                var result = await _TransactionService.CreateTransaction(request, ipAddress);

                _Logger.LogInformation($"Request ended. {invoiceRequest.reference}");

                var response = new CreateTransactionResponse(result.ResultCode);

                if (result.Result == TransactionResultEnum.Success)
                {
                    var invoiceRespose = new InvoiceCreateResponse()
                    {
                        status = 1,
                        invoice_key = result.Item.Token,
                        deposit_id = result.Item.Id
                    };

                    return Ok(invoiceRespose);
                }
                else
                {
                    return Ok(new InvoiceErrorResponse()
                    {
                        status = 0,
                        errorCode = result.ResultCode.ToString(),
                        errorDescription = result.ErrorDescription
                    });
                }
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        /// <summary>
        /// Returns the status of the transaction with the requested token
        /// We don't call the bank bot in this request, we only check from our database.
        /// Bank bot web job service checks the status and updates the database
        /// </summary>
        /// <remarks>
        /// 
        ///     
        /// Error Codes :
        ///     101 --> Api Key Is Null Or Invalid
        ///     102 --> Amount is equal or less than zero
        ///     103 --> Callback url is empty
        ///     200 --> Token is invalid
        ///     201 --> Deposit hasn't been confirmed yet
        ///     202 --> Deposit is expired or canceled
        ///     204 --> Transaction reversed
        ///     207 --> A valid account wasn't found for a deposit or withdrawal
        ///     </remarks>
        /// <param name="invoice_key">Invoice key</param>
        /// <param name="api_key">Api key</param>
        /// <returns></returns>
        [HttpPost("check/{invoice_key}")]
        //[NonUIAttribute]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckInvoiceResponse))]
        public async Task<IActionResult> Check(string invoice_key, [FromForm]string api_key)
        {
            try
            {
                var result = await _TransactionService.Check(invoice_key, api_key);

                if (result.Result == TransactionResultEnum.Success)
                {
                    var invoiceRespose = new CheckInvoiceResponse()
                    {
                        status = 1,
                        amount = result.Amount,
                        bank_code = result.BankNumber,
                        card_number = result.CardNumber,
                        payment_type = result.PaymentType,
                        inProcess = _IncompleteStatuses.Contains(result.Status) ? 1 : 0
                };

                    //_Logger.LogInformation("Check Request : " + invoice_key + " " + api_key + "and Check Response : " + JsonConvert.SerializeObject(invoiceRespose));

                    return Ok(invoiceRespose);
                }
                else
                {
                    var invoiceResponse = new InvoiceErrorResponse()
                    {
                        status = 0,
                        errorCode = result.ResultCode.ToString(),
                        errorDescription = result.ErrorDescription,
                        card_number = result.CardNumber,
                        payment_type = result.PaymentType,
                        inProcess = _IncompleteStatuses.Contains(result.Status) ? 1 : 0
                    };
                    //_Logger.LogInformation("Check Request : " + invoice_key + " " + api_key + "and Check Response : " + JsonConvert.SerializeObject(invoiceResponse));

                    return Ok(invoiceResponse);
                }
            }
            catch (TransactionException ex)
            {
                return BadRequest(new CheckTransactionResponse(ex.Result));
            }
            catch (Exception ex)
            {
                return ReturnUnhandledException(ex);
            }
        }

        /// <summary>
        /// Redirection page of the payment
        /// </summary>
        /// <param name="invoice_key">Invoice key which is generated by the system</param>
        /// <returns></returns>
        //[HttpPost("pay")]
        [HttpGet("pay/{invoice_key}")]
        [HttpPost("pay/{invoice_key}")]
        //[PaymentCall]
        public async Task<IActionResult> Pay(string invoice_key)
        {
            try
            {
                if(!Guid.TryParse(invoice_key, out Guid g))
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string deviceKey = Guid.NewGuid().ToString();

                if (Request.Cookies.ContainsKey(Helper.DeviceKeyCookieName))
                {
                    var cookie = Request.Cookies[Helper.DeviceKeyCookieName];

                    if (!string.IsNullOrEmpty(cookie))
                    {
                        deviceKey = cookie;
                    }
                }

                var ipAddress = HttpContext.GetIpAddress();

                var result = await _TransactionService.GetPaymentInformation(new PaymentInformationRequest()
                {
                    Token = invoice_key,
                    IpAddress = ipAddress,
                    DeviceKey = deviceKey
                });

                if (result.Item != null)
                {
                    var options = new CookieOptions();
                    options.Expires = new DateTime(2035, 1, 1);

                    Response.Cookies.Append(Helper.DeviceKeyCookieName, deviceKey, options);
                }

                TransactionPaymentInformationResponse response = new TransactionPaymentInformationResponse(result.Result);


                if(response.ResultCode == TransactionResultEnum.SmsConfirmationNeeded)
                {
                    Log.Information($"Token {invoice_key}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), result.Result)}");
                    return RedirectToAction("sendsms", new { invoice_key = invoice_key });
                }
                else if (response.ResultCode != TransactionResultEnum.Success)
                {
                    Log.Information($"Token {invoice_key}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), result.Result)}");
                    if (result.Item != null && !string.IsNullOrEmpty(result.Item.ReturnUrl))
                    {
                        var model = new CompletePaymentResponse(result.Result);
                        model.ReturnUrl = result.Item.ReturnUrl;
                        model.Token = invoice_key;
                        model.InProcess = 1;

                        return View("Complete", model);

                    }
                    else
                    {
                        Log.Information($"Token {invoice_key}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), result.Result)}");
                        TempData["Error"] = response.ResultDescription;
                        TempData["ErrorCode"] = ((int)response.ResultCode).ToString();
                        return RedirectToAction("Error");
                    }
                }

                response.CardNumber = result.Item.CardNumber;
                response.CardHolderName = result.Item.CardHolderName;
                response.Amount = result.Item.Amount;
                response.ReturnUrl = result.Item.ReturnUrl;
                response.Token = invoice_key;
                response.TransactionId = result.Item.TransactionId;
                response.CustomerCardNumber = result.Item.CustomerCardNumber;
                response.CaptchaImageData = result.Item.CaptchaImage;

                ViewBag.timer = result.Item.CreationDate.Add(_Configuration.TransactionTimeout).Subtract(DateTime.UtcNow).TotalSeconds;

                if(result.Item.PaymentType == Shared.Models.Entities.PaymentType.CardToCard)
                {
                    return View(response);
                }
                else
                {
                    Log.Information($"Token {invoice_key}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), result.Result)}, redirecting to payment page.");
                    return View("PayMobile", response);
                }

            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                TempData["Error"] = "System Error";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice_key"></param>
        /// <returns></returns>
        [HttpGet("sendsms")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendSms(string invoice_key)
        {
            SmsPhoneNumberModel model = new SmsPhoneNumberModel();
            model.InvoiceKey = invoice_key;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("sendsms")]
       // [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendSms(SmsPhoneNumberModel model)
        {            
            var response = await _TransactionService.SendSms(model);
            if (response.Payload.InvoiceKey != "NoSMSNovin")
            {
                if (!response.Success)
                {

                    TempData["Error"] = "خطای سیستمی";
                    return RedirectToAction("sendsms", new { invoice_key = model.InvoiceKey });
                }
                return RedirectToAction("SmsConfirmation", new { invoice_key = model.InvoiceKey });
            }
            else {
                return Redirect("/invoice/pay/" + model.InvoiceKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice_key"></param>
        /// <returns></returns>
        [HttpGet("smsconfirmation")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SmsConfirmation(string invoice_key)
        {
            SmsVerifyModel model = new SmsVerifyModel();
            model.InvoiceKey = invoice_key;
            model.Seconds = Convert.ToInt32(await _TransactionService.GetTimeoutDuration(invoice_key));

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("smsconfirmation")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SmsConfirmation(SmsVerifyModel model)
        {
            var response = await _TransactionService.VerifySms(model);

            if (response.Success)
            {
                return Redirect("/invoice/pay/" + model.InvoiceKey);
            }
            else
            {
                if (response.Payload != null && response.Payload.IsWrongCode)
                {
                    TempData["Error"] = "کد پیامکی وارد شده معتبر نیست.";
                }
                else
                {
                    TempData["Error"] = "خطای سیستمی";
                }
                return RedirectToAction("smsconfirmation", new { invoice_key = model.InvoiceKey });
            }
        }

        /// <summary>
        /// Completes operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("complete")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [CaptchaValidation("CaptchaCode", "CardToCardCaptcha", "لطفا کد امنیتی را درست وارد نمایید.")]
        public async Task<IActionResult> Complete(CompletePaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid && !_AppSettings.DisableCaptcha)
                {
                    TempData["Error"] = string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(p => p.ErrorMessage));

                    return RedirectToAction("Pay", new { invoice_key = request.Token });
                }
                else
                {                    
                    
                    MvcCaptcha.ResetCaptcha("CardToCardCaptcha");
                }


                string extensions = string.Empty;

                request.IpAddress = HttpContext.GetIpAddress();

                var result = await _TransactionService.CompletePayment(request);

                if (result.ResultCode != TransactionResultEnum.UnknownError)
                {
                    return View(result);
                }

                TempData["Error"] = "System Error";
                TempData["ErrorCode"] = ((int)TransactionResultEnum.UnknownError).ToString();
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                TempData["Error"] = "System Error";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Completes operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("completemobile")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[CaptchaValidation("CaptchaCode", "MobileCaptcha", "لطفا کد امنیتی را درست وارد نمایید.")]
        public async Task<IActionResult> CompleteMobile(CompleteMobilePaymentRequest request)
        {
            try
            {
                //if (!ModelState.IsValid && !_AppSettings.DisableCaptcha)
                //{
                //    TempData["Error"] = string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(p => p.ErrorMessage));

                //    return RedirectToAction("Pay", new { invoice_key = request.Token });
                //}
                //else
                //{
                //    MvcCaptcha.ResetCaptcha("MobileCaptcha");
                //}

                request.CustomerCardNumber = request.CustomerCardNumber.Replace("-", "");

                string extensions = string.Empty;

                request.IpAddress = HttpContext.GetIpAddress();

                var completePaymentRequest = AutoMapper.Mapper.Map<CompletePaymentRequest>(request);

                var result = await _TransactionService.CompletePayment(completePaymentRequest);

                if (result.ResultCode != TransactionResultEnum.UnknownError)
                {
                    return View("PayResult", result);
                }

                TempData["Error"] = "System Error";
                TempData["ErrorCode"] = ((int)TransactionResultEnum.UnknownError).ToString();
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                TempData["Error"] = "System Error";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Cancel operation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("cancel")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Cancel(CancelPaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _Logger.LogInformation($"Transaction is cancelling {request.Token}");

            var result = await _TransactionService.CancelTransaction(request.Token);

            var response = new CompletePaymentResponse(result.Result);

            if (result.Item != null)
            {
                response.ReturnUrl = result.Item.ReturnUrl;
                response.Token = request.Token;

                return View("Complete", response);
            }

            TempData["Error"] = "System Error";
            TempData["ErrorCode"] = ((int)result.ResultCode).ToString();
            return RedirectToAction("Error");
        }

        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtp([FromBody]OtpModel model)
        {
            try
            {
                var response = await _TransactionService.SendOtpMessage(new Shared.Models.MobileTransfer.MobileTransferStartTransferModel()
                {
                    FromCardNo = model.CardNo.Replace("-", ""),
                    TransactionToken = model.Token,
                    Captcha = model.Captcha
                });

                if (response.IsSuccess)
                {
                    return Ok(new { success = 1 });
                }
                else
                {
                    return Ok(new { success = 0, message = "System Error" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = 0, message = "System Error" });
            }
        }

        /// <summary>
        /// Shows error page
        /// </summary>
        /// <returns></returns>
        [HttpGet("Error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }

        private IActionResult ReturnUnhandledException(Exception ex)
        {
            _Logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new TransactionResponse((int)TransactionResultEnum.UnknownError));
        }
    }
}
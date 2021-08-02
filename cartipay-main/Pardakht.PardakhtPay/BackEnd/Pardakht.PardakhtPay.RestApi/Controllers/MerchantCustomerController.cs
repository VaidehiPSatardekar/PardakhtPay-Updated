using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages merchant customer operations
    /// </summary>
    [Route("api/merchantcustomer")]
    [ApiController]
    //[Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MerchantCustomerController : PardakhtPayBaseController
    {
        IMerchantCustomerService _Service;
        ITransactionService _TransactionService = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        /// <param name="transactionService"></param>
        public MerchantCustomerController(IMerchantCustomerService service,
            ILogger<MerchantCustomerController> logger,
            ITransactionService transactionService) : base(logger)
        {
            _Service = service;
            _TransactionService = transactionService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _Service.GetCustomerAsync(id);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("search")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> Search(MerchantCustomerSearchArgs args)
        {
            try
            {
                var response = await _Service.Search(args);


                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getPhoneNumberRelatedCustomers/{phoneNumber}")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> GetPhoneNumberRelatedCustomers(string phoneNumber)
        {
            try
            {
                var response = await _Service.GetPhoneNumberRelatedCustomer(phoneNumber);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getRelatedCustomers/{id}")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> GetRelatedCustomers(int id)
        {
            try
            {
                var response = await _Service.GetRelatedCustomers(id);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getCardNumbersCount/{id}")]
       // [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> GetCardNumbersCount(int id)
        {
            try
            {
                var response = await _Service.GetCardNumberCounts(id);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("updateusersegmentgroup/{id}")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> UpdateUserSegmentGroup(int id, [FromBody]MerchantCustomerDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != model.Id)
                {
                    return BadRequest("Identities are different");
                }

                var response = await _Service.UpdateUserSegmentGroup(model);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpPost("send-sms")]
        [ApiCall]
        public async Task<IActionResult> SendSms([FromBody]ExternalSendSmsConfirmationRequest model)
        {
            try
            {
                Logger.LogInformation(JsonConvert.SerializeObject(model));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _TransactionService.ExternalSendCustomerSms(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("verify-sms")]
        [ApiCall]
        public async Task<IActionResult> VerifySmsCode([FromBody]ExternalSmsVerifiyRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _TransactionService.ExternalVerifyCustomerSms(model);

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
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("replace-userid")]
        [ApiCall]
        public async Task<IActionResult> ReplaceUserId([FromBody]ReplaceUserIdRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _Service.ReplaceUserId(model);

                if (response.Success)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("downloadphonenumbers")]
        [Authorize(Roles = "CP-CUSTOMERS")]
        public async Task<IActionResult> DownloadPhoneNumbers(MerchantCustomerSearchArgs args)
        {
            try
            {
                var response = await _Service.Search(args);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("exportphonenumbers")]
        [Authorize(Roles = "CP-CUSTOMERS-PHONENUMBERS")]
        public async Task<IActionResult> exportphonenumbers(MerchantCustomerSearchArgs args)
        {
            try
            {
                var response = await _Service.ExportPhoneNumbers(args);
                if (!response.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                Response.Headers.Add("File-Name", $"PhoneNumbers_{DateTime.Now.ToString("ddMMyyyHHmmss")}.csv");

                return File(response.Payload.Data, response.Payload.ContentType, $"{Guid.NewGuid().ToString()}.csv");

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getregisteredphones/{id}")]
        //[Authorize(Roles = "CP-CUSTOMERS-REMOVE-PHONENUMBERS")]
        public async Task<IActionResult> GetRegisteredPhones(int id)
        {
            try
            {
                var response = await _Service.GetRegisteredPhones(id);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("removeregisteredphones/{id}")]
        [Authorize(Roles = "CP-CUSTOMERS-REMOVE-PHONENUMBERS")]
        public async Task<IActionResult> RemoveRegisteredPhones(int id, RegisteredPhoneNumbersList phoneNumbers)
        {
            try
            {
                var response = await _Service.RemoveRegisteredPhones(id, phoneNumbers);

                if (response.Success)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
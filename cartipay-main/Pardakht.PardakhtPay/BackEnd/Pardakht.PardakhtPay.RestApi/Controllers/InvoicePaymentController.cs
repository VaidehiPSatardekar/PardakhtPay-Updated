using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/invoicepayment")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InvoicePaymentController : PardakhtPayBaseController
    {
        IInvoicePaymentService _Service = null;

        public InvoicePaymentController(ILogger<InvoicePaymentController> logger,
            IInvoicePaymentService service):base(logger)
        {
            _Service = service;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Permissions.AddInvoicePayment)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _Service.GetItemById(id);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("search")]
        [Authorize(Roles = Permissions.ListInvoicePayments)]
        public async Task<IActionResult> Search([FromBody]InvoicePaymentSearchArgs args)
        {
            try
            {
                var response = await _Service.Search(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Roles = Permissions.AddInvoicePayment)]
        public async Task<IActionResult> Post([FromBody]InvoicePaymentDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Permissions.AddInvoicePayment)]
        public async Task<IActionResult> Put(int id, [FromBody]InvoicePaymentDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest("Entity and identity is different");
            }

            var result = await _Service.UpdateAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Permissions.AddInvoicePayment)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }
    }
}
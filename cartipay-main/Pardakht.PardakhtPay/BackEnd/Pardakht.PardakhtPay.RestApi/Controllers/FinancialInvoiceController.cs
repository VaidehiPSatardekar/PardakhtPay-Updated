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
    [Route("api/financialinvoice")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FinancialInvoiceController : PardakhtPayBaseController
    {
        IInvoiceService _Service = null;

        public FinancialInvoiceController(ILogger<FinancialInvoiceController> logger,
            IInvoiceService service)
            :base(logger)
        {
            _Service = service;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Permissions.ListInvoices)]
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
        [Authorize(Roles = Permissions.ListInvoices)]
        public async Task<IActionResult> Search([FromBody]InvoiceSearchArgs args)
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
    }
}
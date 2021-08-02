using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/financialinvoiceownersetting")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FinancialInvoiceOwnerSettingController : PardakhtPayBaseController
    {
        IInvoiceOwnerSettingService _Service = null;

        public FinancialInvoiceOwnerSettingController(IInvoiceOwnerSettingService service,
            ILogger<FinancialInvoiceOwnerSettingController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [Authorize(Roles = Permissions.ListInvoiceOwnerSetting)]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Permissions.AddInvoiceOwnerSetting)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetInvoiceOwnerSettingById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = Permissions.AddInvoiceOwnerSetting)]
        public async Task<IActionResult> Post([FromBody]InvoiceOwnerSettingDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Permissions.AddInvoiceOwnerSetting)]
        public async Task<IActionResult> Put(int id, [FromBody]InvoiceOwnerSettingDTO item)
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
        [Authorize(Roles = Permissions.AddInvoiceOwnerSetting)]
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
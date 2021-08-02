using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/transferaccount")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TransferAccountController : PardakhtPayBaseController
    {
        ITransferAccountService _Service = null;

        public TransferAccountController(ITransferAccountService service, ILogger<TransferAccountController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CP-ADD-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = "CP-ADD-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Post([FromBody]TransferAccountDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CP-ADD-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Put(int id, [FromBody]TransferAccountDTO item)
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
        [Authorize(Roles = "CP-ADD-TRANSFER-ACCOUNT")]
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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/mobiletransferaccount")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileTransferAccountController : PardakhtPayBaseController
    {
        IMobileTransferCardAccountService _Service = null;

        public MobileTransferAccountController(IMobileTransferCardAccountService service,
            ILogger<MobileTransferAccountController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [Authorize(Roles = "CP-MOBILE-ACCOUNTS")]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CP-MOBILE-ACCOUNTS")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Post([FromBody]MobileTransferCardAccountDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Put(int id, [FromBody]MobileTransferCardAccountDTO item)
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
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Json("Operation completed successfully");
            }

            return BadRequest(result.Message);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/mobiletransferdevice")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileTransferDeviceController : PardakhtPayBaseController
    {
        IMobileTransferDeviceService _Service;

        public MobileTransferDeviceController(IMobileTransferDeviceService service,
            ILogger<MobileTransferDeviceController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [Authorize(Roles = "CP-MOBILE-DEVICES")]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CP-MOBILE-DEVICES")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> Post([FromBody]MobileTransferDeviceDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> Put(int id, [FromBody]MobileTransferDeviceDTO item)
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
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Json("Operation completed successfully");
            }

            return BadRequest(result.Message);
        }

        [HttpPost("sendsms/{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> SendSms(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.SendSmsAsync(id);

            return ReturnWebResponse(result);
        }

        [HttpPost("activate/{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> Activate(int id, [FromBody]MobileTransferActivateDeviceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.ActivateAsync(id, model.VerificationCode);

            return ReturnWebResponse(result);
        }

        [HttpPost("checkstatus/{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> CheckStatus(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.CheckStatusAsync(id);

            return ReturnWebResponse(result);
        }

        [HttpPost("remove/{id}")]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-DEVICE")]
        public async Task<IActionResult> Remove(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.RemoveAsync(id);

            return ReturnWebResponse(result);
        }
    }
}
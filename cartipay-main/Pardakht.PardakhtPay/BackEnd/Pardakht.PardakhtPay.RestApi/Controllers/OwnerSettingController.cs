using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/ownersetting")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OwnerSettingController : PardakhtPayBaseController
    {
        IOwnerSettingService _Service = null;

        public OwnerSettingController(IOwnerSettingService service,
            ILogger<OwnerSettingController> logger)
            :base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [Authorize(Roles = Permissions.OwnerSettings)]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.Get();

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = Permissions.OwnerSettings)]
        public async Task<IActionResult> Post([FromBody]OwnerSettingDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            WebResponse<OwnerSettingDTO> result = null;

            if (item.Id == 0)
            {
                result = await _Service.InsertAsync(item);
            }
            else
            {
                result = await _Service.UpdateAsync(item);
            }

            return ReturnWebResponse(result);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/blockedCardnumber")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BlockedCardNumberController : PardakhtPayBaseController
    {
        IBlockedCardNumberService _Service = null;

        public BlockedCardNumberController(IBlockedCardNumberService service,
            ILogger<BlockedCardNumberController> logger) : base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BlockedCardNumberDTO>))]
        [Authorize(Roles = Permissions.BlockedCardNumbers)]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockedCardNumberDTO))]
        [Authorize(Roles = Permissions.AddBlockedCardNumber)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = Permissions.AddBlockedCardNumber)]
        public async Task<IActionResult> Post([FromBody] BlockedCardNumberDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockedCardNumberDTO))]
        [Authorize(Roles = Permissions.AddBlockedCardNumber)]
        public async Task<IActionResult> Put(int id, [FromBody] BlockedCardNumberDTO item)
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
        [Authorize(Roles = Permissions.AddBlockedCardNumber)]
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

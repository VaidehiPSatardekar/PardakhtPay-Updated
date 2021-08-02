using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/blockedphonenumber")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BlockedPhoneNumberController : PardakhtPayBaseController
    {
        IBlockedPhoneNumberService _Service = null;

        public BlockedPhoneNumberController(IBlockedPhoneNumberService service,
            ILogger<BlockedPhoneNumberController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BlockedPhoneNumberDTO>))]
        [Authorize(Roles = Permissions.BlockedPhoneNumbers)]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockedPhoneNumberDTO))]
        [Authorize(Roles = Permissions.EditBlockedPhoneNumber)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = Permissions.AddBlockedPhoneNumber)]
        public async Task<IActionResult> Post([FromBody]BlockedPhoneNumberDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlockedPhoneNumberDTO))]
        [Authorize(Roles = Permissions.EditBlockedPhoneNumber)]
        public async Task<IActionResult> Put(int id, [FromBody]BlockedPhoneNumberDTO item)
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
        [Authorize(Roles = Permissions.DeleteBlockedPhoneNumber)]
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
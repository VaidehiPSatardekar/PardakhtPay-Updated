using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/mobiletransfercardaccountgroup")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileTransferCardAccountGroupController : PardakhtPayBaseController
    {
        IMobileTransferCardAccountGroupService _Service;

        public MobileTransferCardAccountGroupController(IMobileTransferCardAccountGroupService service,
            ILogger<MobileTransferCardAccountGroupController> logger):base(logger)
        {
            _Service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MobileTransferCardAccountGroupDTO>))]
        [Authorize(Roles = "CP-MOBILE-ACCOUNT-GROUPS")]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Returns a card to card account with the given id
        /// </summary>
        /// <description>
        /// Returns a card to card account with the given id
        /// </description>
        /// <param name="id">id of the card to card account</param>
        /// <returns>Card to card account with given id. <see cref="MobileTransferCardAccountGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MobileTransferCardAccountGroupDTO))]
        [Authorize(Roles = "CP-MOBILE-ACCOUNT-GROUPS")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Creates a new card to card account
        /// </summary>
        /// <description>
        /// Creates a new card to card account
        /// </description>
        /// <param name="item">Card to card account parameters. <see cref="MobileTransferCardAccountGroupDTO"/></param>
        /// <returns>Created card to card accounts</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT-GROUP")]
        public async Task<IActionResult> Post([FromBody]MobileTransferCardAccountGroupDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Updates a card to card account
        /// </summary>
        /// <description>
        /// Updates a card to card account
        /// </description>
        /// <param name="id">Id of the card to card account</param>
        /// <param name="item">Card to card account dto. <see cref="MobileTransferCardAccountGroupDTO"/></param>
        /// <returns>Updated card to card account. <see cref="MobileTransferCardAccountGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MobileTransferCardAccountGroupDTO))]
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT-GROUP")]
        public async Task<IActionResult> Put(int id, [FromBody]MobileTransferCardAccountGroupDTO item)
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
        [Authorize(Roles = "CP-ADD-MOBILE-TRANSFER-ACCOUNT-GROUP")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }

        [HttpPost("clearcache")]
        [Authorize]
        public async Task<IActionResult> ClearCache()
        {
            await _Service.ClearCache();
            return Ok();
        }
    }
}
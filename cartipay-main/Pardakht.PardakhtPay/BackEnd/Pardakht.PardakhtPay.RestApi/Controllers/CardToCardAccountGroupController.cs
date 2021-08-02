using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages card to card account group api operations
    /// </summary>
    [Route("api/cardToCardAccountGroup")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CardToCardAccountGroupController : PardakhtPayBaseController
    {
        ICardToCardAccountGroupService _Service = null;

        /// <summary>
        /// Intialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public CardToCardAccountGroupController(ICardToCardAccountGroupService service,
            ILogger<CardToCardAccountGroupController> logger):base(logger)
        {
            _Service = service;
        }

        /// <summary>
        /// Returns all card to card accounts
        /// </summary>
        /// <description>
        /// Returns all card to card accounts
        /// </description>
        /// <returns>List of <see cref="CardToCardAccountGroupDTO"/></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CardToCardAccountGroupDTO>))]
        [Authorize(Roles = "CP-BANK-ACC-GROUPS")]
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
        /// <returns>Card to card account with given id. <see cref="CardToCardAccountGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardToCardAccountGroupDTO))]
        [Authorize(Roles = "CP-BANK-ACC-GROUPS")]
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
        /// <param name="item">Card to card account parameters. <see cref="CardToCardAccountGroupDTO"/></param>
        /// <returns>Created card to card accounts</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [Authorize(Roles = "CP-ADD-BANK-ACCOUNT-GROUP")]
        public async Task<IActionResult> Post([FromBody]CardToCardAccountGroupDTO item)
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
        /// <param name="item">Card to card account dto. <see cref="CardToCardAccountGroupDTO"/></param>
        /// <returns>Updated card to card account. <see cref="CardToCardAccountGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardToCardAccountGroupDTO))]
        [Authorize(Roles = "CP-ADD-BANK-ACCOUNT-GROUP")]
        public async Task<IActionResult> Put(int id, [FromBody]CardToCardAccountGroupDTO item)
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
        [Authorize(Roles = "CP-ADD-BANK-ACCOUNT-GROUP")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }

        /// <summary>
        /// Checks paused accounts
        /// </summary>
        /// <returns></returns>
        [HttpPost("checkpausedaccounts")]
        [Authorize]
        [ApiCall]
        public async Task<IActionResult> CheckPausedAccounts()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.CheckPausedAccounts();

            return Ok();
        }
    }
}
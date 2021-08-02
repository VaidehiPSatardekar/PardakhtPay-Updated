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
    /// Manages card to card operations
    /// </summary>
    [Route("api/cardtocardaccount")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CardToCardAccountController : PardakhtPayBaseController
    {
        ICardToCardAccountService _Service = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public CardToCardAccountController(ICardToCardAccountService service, ILogger<CardToCardAccountController> logger):base(logger)
        {
            _Service = service;
        }

        /// <summary>
        /// Returns all card to card accounts
        /// </summary>
        /// <description>
        /// Returns all card to card accounts
        /// </description>
        /// <returns>List of <see cref="CardToCardAccountDTO"/></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CardToCardAccountDTO>))]
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
        /// <returns>Card to card account with given id. <see cref="CardToCardAccountDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardToCardAccountDTO))]
        [Authorize(Roles = "CP-BANK-ACCOUNTS")]
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
        /// <param name="item">Card to card account parameters. <see cref="CardToCardAccountDTO"/></param>
        /// <returns>Created card to card accounts</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> Post([FromBody]CardToCardAccountDTO item)
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
        /// <param name="item">Card to card account dto. <see cref="CardToCardAccountDTO"/></param>
        /// <returns>Updated card to card account. <see cref="CardToCardAccountDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardToCardAccountDTO))]
        [Authorize(Roles = "CP-ADD-BANK-LOGIN")]
        public async Task<IActionResult> Put(int id, [FromBody]CardToCardAccountDTO item)
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _Service.DeleteAsync(id);

        //    if (result.Success)
        //    {
        //        return Json("Operation completed successfully");
        //    }

        //    return BadRequest(result.Message);
        //}
    }
}
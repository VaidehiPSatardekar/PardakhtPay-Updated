using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages merchant operations
    /// </summary>
    [Route("api/merchant")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MerchantController : PardakhtPayBaseController
    {
        IMerchantService _Service = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public MerchantController(IMerchantService service, ILogger<MerchantController> logger) : base(logger)
        {
            _Service = service;
        }

        /// <summary>
        /// Returns all merchants
        /// </summary>
        /// <returns>List of merchants. <see cref="MerchantDTO"/></returns>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MerchantDTO>))]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Returns the merchant with given id
        /// </summary>
        /// <param name="id">Id of the merchant</param>
        /// <returns>Merchant. <see cref="MerchantUpdateDTO"/></returns>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MerchantUpdateDTO))]
        [Authorize(Roles = "CP-ADD-MERCHANT")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetMerchantById(id);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Creates a new merchant
        /// </summary>
        /// <param name="item">Parameters. <see cref="MerchantCreateDTO"/></param>
        /// <returns>Created merchant. <see cref="MerchantCreateDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MerchantCreateDTO))]
        [Authorize(Roles = "CP-ADD-MERCHANT")]
        public async Task<IActionResult> Post([FromBody]MerchantCreateDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertWithAccountsAsync(item);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Updates the merchant.
        /// </summary>
        /// <param name="id">Id of merchant record</param>
        /// <param name="item">Parameters. <see cref="MerchantUpdateDTO"/></param>
        /// <returns>Updated merchant</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MerchantUpdateDTO))]
        [Authorize(Roles = "CP-ADD-MERCHANT")]
        public async Task<IActionResult> Put(int id, [FromBody]MerchantUpdateDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest("Entity and identity is different");
            }

            var result = await _Service.UpdateWithAccountsAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CP-ADD-MERCHANT")]
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
        /// Returns all merchants
        /// </summary>
        /// <param name="term"></param>
        /// <returns>List of merchants. <see cref="Merchant"/></returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Merchant>))]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                term = term ?? string.Empty;

                var result = await _Service.Search(term);

                if (!result.Success)
                    return BadRequest(result.Message);
                else
                    return Ok(result.Payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"An error occurred searching tenants: term={term}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred searching merchants: term={term}");
            }
        }
    }
}
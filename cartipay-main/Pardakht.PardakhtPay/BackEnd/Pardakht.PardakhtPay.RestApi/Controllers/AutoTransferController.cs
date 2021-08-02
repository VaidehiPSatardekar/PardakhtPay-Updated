using System;
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
    /// Manages auto transfer operations
    /// </summary>
    [Route("api/autotransfer")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AutoTransferController : ControllerBase
    {
        IAutoTransferService _Service = null;
        ILogger<AutoTransferController> _Logger;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public AutoTransferController(IAutoTransferService service,
            ILogger<AutoTransferController> logger)
        {
            _Service = service;
            _Logger = logger;
        }

        /// <summary>
        /// Search accounting values
        /// </summary>
        /// <description>
        /// Search accounting values
        /// </description>
        /// <remarks>
        ///  Sample Request
        ///     POST /api/autotransfer/search
        ///     {
        ///         "pageSize":20,
        ///         "pageNumber":0,
        ///         "dateRange":"dt",
        ///         "tenants":["308ed0c8-3c7a-6975-8180-f80722dc5490"],
        ///         "timeZoneInfoId":"UTC"
        ///     }
        /// 
        /// </remarks>
        /// <param name="args">Search parameters. See <see cref="AutoTransferSearchArgs"/></param>
        /// <returns>List of accounting values. See <see cref="AutoTransferDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListSearchResponse<IEnumerable<AutoTransferDTO>>))]
        [Authorize(Roles = "CP-AUTO-TRANSFERS")]
        public async Task<IActionResult> Search(AutoTransferSearchArgs args)
        {
            try
            {
                Random rnd = new Random();
                var response = await _Service.Search(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception");
            }
        }
    }
}
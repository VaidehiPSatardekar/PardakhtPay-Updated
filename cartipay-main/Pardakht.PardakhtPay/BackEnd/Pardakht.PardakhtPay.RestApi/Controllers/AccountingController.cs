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
    /// Manages accounting operations
    /// </summary>
    [Route("api/accounting")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountingController : Controller
    {
        ITransactionService _TransactionService = null;
        ILogger<AccountingController> _Logger = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="transactionService"></param>
        /// <param name="logger"></param>
        public AccountingController(ITransactionService transactionService,
            ILogger<AccountingController> logger)
        {
            _TransactionService = transactionService;
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
        ///     POST /api/accounting/search
        ///     {
        ///         "pageSize":20,
        ///         "pageNumber":0,
        ///         "dateRange":"dt",
        ///         "statuses":["1","2","3","4","5","6"],
        ///         "tenants":["308ed0c8-3c7a-470f-8180-f80722dc5490"],
        ///         "timeZoneInfoId":"UTC"
        ///     }
        /// 
        /// </remarks>
        /// <param name="args">Search parameters. See <see cref="TransactionSearchArgs"/></param>
        /// <returns>List of accounting values. See <see cref="DailyAccountingDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListSearchResponse<IEnumerable<DailyAccountingDTO>>))]
        [Authorize(Roles = "CP-ACCOUNTING")]
        public async Task<IActionResult> Search(AccountingSearchArgs args)
        {
            try
            {
                Random rnd = new Random();
                var response = await _TransactionService.SearchAccounting(args);

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
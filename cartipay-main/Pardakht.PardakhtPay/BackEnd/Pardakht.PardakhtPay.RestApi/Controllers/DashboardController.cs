using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Dashboard;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages dashboard operations
    /// </summary>
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DashboardController : PardakhtPayBaseController
    {
        IDashboardService _DashboardService = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public DashboardController(IDashboardService service,
            ILogger<DashboardController> logger):base(logger)
        {
            _DashboardService = service;
        }

        /// <summary>
        /// Returns transaction chart report
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Chart report of transactions</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "C-001")]
        [HttpPost("gettransactiongraphreport")]
        [Authorize(Roles = "CP-DB-DEPOSIT-GRAPH")]
        public async Task<IActionResult> GetTransactionChartReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _DashboardService.GetChartWidget(WidgetType.SalesReport, query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction chart report");
            }
        }

        /// <summary>
        /// Returns accounting chart report
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Chart report of accounting</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("getaccountinggraphreport")]
        [Authorize(Roles = "CP-DB-ACCOUNTING-GRAPH")]
        public async Task<IActionResult> GetAccountingChartReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _DashboardService.GetChartWidget(WidgetType.Accounting, query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction chart report");
            }
        }

        /// <summary>
        /// Returns transaction report values
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Transaction report values</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "C-008")]
        [HttpPost("gettransactionreport")]
        [Authorize(Roles = "CP-DB-DEPOSIT-TOTAL")]
        public async Task<IActionResult> GetTransactionReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _DashboardService.GetWidget(WidgetType.SalesReport, query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction report");
            }
        }

        /// <summary>
        /// Reports transaction reports by merchants
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Merchant transaction reports</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("getmerchanttransactionreport")]
        [Authorize(Roles = "CP-DB-MERCHANT")]
        public async Task<IActionResult> GetMerchantTransactionReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                //var response = await _DashboardService.GetWidget(WidgetType.SalesForMerchants, query);

                var response = await _DashboardService.GetMerchantTransactionReport(query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction report");
            }
        }

        /// <summary>
        /// Returns account status report
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Account status report values</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("getaccountstatusreport")]
        [Authorize(Roles = "CP-DB-ACCOUNT-SUMMARY")]
        public async Task<IActionResult> GetAccountStatusReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                //var response = await _DashboardService.GetWidget(WidgetType.AccountStatusReport, query);

                //return ReturnWebResponse(response);

                var response = await _DashboardService.GetAccountStatuses(query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing account status report");
            }
        }

        /// <summary>
        /// Returns deposit transaction chart report
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Chart report of deposit transactions</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "C-001")]
        [HttpPost("gettransactiondepositbreakdowngraphreport")]
        [Authorize(Roles = "CP-DB-DEPOSIT-GRAPH")]
        public async Task<IActionResult> GetTransactionDepositBreakDownChartReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _DashboardService.GetChartWidget(WidgetType.DepositBreakDownReport, query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction chart report");
            }
        }

        /// <summary>
        /// Returns transaction report values for withdrawal
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Transaction report values for withd</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "C-008")]
        [HttpPost("gettransactionreportforwithdrawal")]
        [Authorize(Roles = "CP-DB-DEPOSIT-TOTAL")]
        public async Task<IActionResult> GetTransactionReportForWithdrawal([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _DashboardService.GetWidget(WidgetType.WithdrawPaymentWidget, query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction report");
            }
        }
              

        /// <summary>
        /// Reports Payment Break Down Report
        /// </summary>
        /// <param name="query">Parameters</param>
        /// <returns>Merchant transaction reports</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("gettransactionbypaymenttypereport")]
        [Authorize(Roles = "CP-DB-MERCHANT")]
        public async Task<IActionResult> GetTransactionByPaymentTypeReport([FromBody]DashboardQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                //var response = await _DashboardService.GetWidget(WidgetType.SalesForMerchants, query);

                var response = await _DashboardService.GetTransactionByPaymentTypeReport(query);

                return ReturnWebResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled exception occured while processing transaction report");
            }
        }
    }
}
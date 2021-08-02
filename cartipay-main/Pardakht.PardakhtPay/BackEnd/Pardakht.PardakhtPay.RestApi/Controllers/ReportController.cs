using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/report")]
    [ApiController]
   // [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportController : ControllerBase
    {
        IReportService _Service = null;
        ILogger _Logger = null;

        public ReportController(IReportService service,
            ILogger<ReportController> logger)
        {
            _Service = service;
            _Logger = logger;
        }

        [HttpPost("getusersegmentreport")]
        [Authorize(Roles = Permissions.UserSegmentReport)]
        public async Task<IActionResult> GetUserSegmentReport(UserSegmentReportSearchArgs args)
        {
            try
            {
                var response = await _Service.GetUserSegmentReport(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("gettenantbalances")]
        [Authorize(Roles = Permissions.TenantCurrentBalanceReport)]
        public async Task<IActionResult> GetTenantBalances([FromBody]TenantBalanceSearchDTO model)
        {
            try
            {
                var response = await _Service.GetTenantBalances(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getdepositwithdrawalwidget")]
        [Authorize(Roles = Permissions.DepositWithdrawalReport)]
        public async Task<IActionResult> GetDepositWithdrawalWidget(TransactionReportSearchArgs args)
        {
            try
            {
                var response = await _Service.GetDepositWithdrawalWidget(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getwithdrawalpaymentwidget")]
        [Authorize(Roles = Permissions.WithdrawalPayments)]
        public async Task<IActionResult> GetWithdrawalPaymentWidget(WithdrawalPaymentReportArgs args)
        {
            try
            {
                var response = await _Service.GetWithdrawalPaymentWidget(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getdepositbyaccountidwidget")]
        [Authorize(Roles = Permissions.DepositWithdrawalReport)]
        public async Task<IActionResult> GetDepositByAccountIdWidget(TransactionReportSearchArgs args)
        {
            try
            {
                var response = await _Service.GetDepositByAccountIdWidget(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getdepositbybreakdownlist")]
        [Authorize(Roles = Permissions.DepositWithdrawalReport)]
        public async Task<IActionResult> GetDepositBreakdownList(TransactionReportSearchArgs args)
        {
            try
            {
                var response = await _Service.GetDepositBreakdownList(args);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
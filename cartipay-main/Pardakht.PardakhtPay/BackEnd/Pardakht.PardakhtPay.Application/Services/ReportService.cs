using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class ReportService : IReportService
    {
        IReportManager _ReportManager = null;
        ILogger _Logger = null;

        public ReportService(IReportManager reportManager,
            ILogger<ReportService> logger)
        {
            _ReportManager = reportManager;
            _Logger = logger;
        }

        public async Task<WebResponse<DashboardChartWidget>> GetDepositWithdrawalWidget(TransactionReportSearchArgs args)
        {
            try
            {
                var widget = await _ReportManager.GetDepositWithdrawalWidget(args);

                return new WebResponse<DashboardChartWidget>(widget);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<DashboardChartWidget>(ex);
            }
        }

 
        public async Task<WebResponse<List<TenantBalanceDTO>>> GetTenantBalances(TenantBalanceSearchDTO model)
        {
            try
            {
                var response = await _ReportManager.GetTenantBalances(model);

                return new WebResponse<List<TenantBalanceDTO>>(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<List<TenantBalanceDTO>>(ex);
            }
        }

        public async Task<WebResponse<List<UserSegmentReportDTO>>> GetUserSegmentReport(UserSegmentReportSearchArgs args)
        {
            try
            {
                var response = await _ReportManager.GetUserSegmentReport(args);

                return new WebResponse<List<UserSegmentReportDTO>>(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<List<UserSegmentReportDTO>>(ex);
            }
        }

        public async Task<WebResponse<DashboardChartWidget>> GetWithdrawalPaymentWidget(WithdrawalPaymentReportArgs args)
        {
            try
            {
                var response = await _ReportManager.GetWithdrawalPaymentWidget(args);

                return new WebResponse<DashboardChartWidget>(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<DashboardChartWidget>(ex);
            }
        }

        public async Task<WebResponse<DashboardChartWidget>> GetDepositByAccountIdWidget(TransactionReportSearchArgs args)
        {
            try
            {
                var widget = await _ReportManager.GetDepositByAccountIdWidget(args);

                return new WebResponse<DashboardChartWidget>(widget);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<DashboardChartWidget>(ex);
            }
        }

        public async Task<WebResponse<ListSearchResponse<List<DepositBreakDownReport>>>> GetDepositBreakdownList(TransactionReportSearchArgs args)
        {
            try
            {
                var widget = await _ReportManager.GetDepositBreakdownList(args);

                return new WebResponse<ListSearchResponse<List<DepositBreakDownReport>>>(widget);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<ListSearchResponse<List<DepositBreakDownReport>>>(ex);
            }
        }

    }
}

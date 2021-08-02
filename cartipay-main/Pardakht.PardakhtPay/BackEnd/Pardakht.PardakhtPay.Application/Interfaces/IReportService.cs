using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IReportService
    {
        Task<WebResponse<List<UserSegmentReportDTO>>> GetUserSegmentReport(UserSegmentReportSearchArgs args);

        Task<WebResponse<DashboardChartWidget>> GetWithdrawalPaymentWidget(WithdrawalPaymentReportArgs args);

        Task<WebResponse<List<TenantBalanceDTO>>> GetTenantBalances(TenantBalanceSearchDTO model);

        Task<WebResponse<DashboardChartWidget>> GetDepositWithdrawalWidget(TransactionReportSearchArgs args);

        Task<WebResponse<DashboardChartWidget>> GetDepositByAccountIdWidget(TransactionReportSearchArgs args);

        Task<WebResponse<ListSearchResponse<List<DepositBreakDownReport>>>> GetDepositBreakdownList(TransactionReportSearchArgs args);


    }
}

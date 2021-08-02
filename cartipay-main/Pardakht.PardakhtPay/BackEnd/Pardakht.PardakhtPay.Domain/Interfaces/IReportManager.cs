using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IReportManager
    {
        Task<DashboardChartWidget> GetDepositWithdrawalWidget(TransactionReportSearchArgs args);

        Task<DashboardChartWidget> GetWithdrawalPaymentWidget(WithdrawalPaymentReportArgs args);

        Task<List<UserSegmentReportDTO>> GetUserSegmentReport(UserSegmentReportSearchArgs args);

        Task<List<TenantBalanceDTO>> GetTenantBalances(TenantBalanceSearchDTO model);

        Task<DashboardChartWidget> GetDepositByAccountIdWidget(TransactionReportSearchArgs args);


        Task<ListSearchResponse<List<DepositBreakDownReport>>> GetDepositBreakdownList(TransactionReportSearchArgs args);


    }
}

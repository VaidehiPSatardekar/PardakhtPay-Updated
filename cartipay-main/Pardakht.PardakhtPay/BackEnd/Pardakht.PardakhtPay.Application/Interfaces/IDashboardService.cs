using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    /// <summary>
    /// Represents an interface for dashboard service operations
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Returns the chart widget for given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<WebResponse<DashboardChartWidget>> GetChartWidget(WidgetType type, DashboardQuery query);

        /// <summary>
        /// Returns the widget for given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<WebResponse<DashboardWidget>> GetWidget(WidgetType type, DashboardQuery query);

        Task<WebResponse<List<DashboardAccountStatusDTO>>> GetAccountStatuses(DashboardQuery query);

        Task<WebResponse<List<DashboardMerchantTransactionDTO>>> GetMerchantTransactionReport(DashboardQuery query);

        Task<WebResponse<List<DashboardPaymentTypeBreakDown>>> GetTransactionByPaymentTypeReport(DashboardQuery query);

    }
}

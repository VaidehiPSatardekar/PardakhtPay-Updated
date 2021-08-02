using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    /// <summary>
    /// Represents and interface to define dashboard managing methods
    /// </summary>
    public interface IDashboardManager
    {
        /// <summary>
        /// Returns a chart widget for given type
        /// </summary>
        /// <param name="widgetType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<DashboardChartWidget> GetChartWidget(WidgetType widgetType, DashboardQuery query);

        /// <summary>
        /// Returns a widget for given type
        /// </summary>
        /// <param name="widgetType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<DashboardWidget> GetWidget(WidgetType widgetType, DashboardQuery query);

        Task<List<DashboardAccountStatusDTO>> GetAccountStatuses(DashboardQuery query);

        Task<List<DashboardMerchantTransactionDTO>> GetMerchantTransactionDTO(DashboardQuery query);

        Task<List<DashboardPaymentTypeBreakDown>> GetTransactionByPaymentTypeReport(DashboardQuery query);
    }
}

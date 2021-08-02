using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    /// <summary>
    /// Represents a class which implements <see cref="IDashboardManager"/> to manage dashboard widget operations
    /// </summary>
    public class DashboardService : IDashboardService
    {
        IDashboardManager _DashboardManager;
        ILogger _Logger;

        public DashboardService(IDashboardManager manager,
            ILogger<DashboardService> logger)
        {
            _DashboardManager = manager;
            _Logger = logger;
        }

        /// <summary>
        /// Generates and returns a chart widget for given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<WebResponse<DashboardChartWidget>> GetChartWidget(WidgetType type, DashboardQuery query)
        {
            try
            {
                var widget = await _DashboardManager.GetChartWidget(type, query);

                return new WebResponse<DashboardChartWidget>(true, string.Empty, widget);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<DashboardChartWidget>(false, ex.Message);
            }
        }

        /// <summary>
        /// Generates and returns a widget for given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<WebResponse<DashboardWidget>> GetWidget(WidgetType type, DashboardQuery query)
        {
            try
            {
                var widget = await _DashboardManager.GetWidget(type, query);

                return new WebResponse<DashboardWidget>(true, string.Empty, widget);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<DashboardWidget>(false, ex.Message);
            }
        }

        public async Task<WebResponse<List<DashboardAccountStatusDTO>>> GetAccountStatuses(DashboardQuery query)
        {
            try
            {
                var items = await _DashboardManager.GetAccountStatuses(query);

                return new WebResponse<List<DashboardAccountStatusDTO>>(items);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);

                return new WebResponse<List<DashboardAccountStatusDTO>>(ex);
            }
        }

        public async Task<WebResponse<List<DashboardMerchantTransactionDTO>>> GetMerchantTransactionReport(DashboardQuery query)
        {
            try
            {
                var items = await _DashboardManager.GetMerchantTransactionDTO(query);

                return new WebResponse<List<DashboardMerchantTransactionDTO>>(items);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<List<DashboardMerchantTransactionDTO>>(ex);
            }
        }
              

        public async Task<WebResponse<List<DashboardPaymentTypeBreakDown>>> GetTransactionByPaymentTypeReport(DashboardQuery query)
        {
            try
            {
                var items = await _DashboardManager.GetTransactionByPaymentTypeReport(query);

                return new WebResponse<List<DashboardPaymentTypeBreakDown>>(items);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse<List<DashboardPaymentTypeBreakDown>>(ex);
            }
        }
    }
}

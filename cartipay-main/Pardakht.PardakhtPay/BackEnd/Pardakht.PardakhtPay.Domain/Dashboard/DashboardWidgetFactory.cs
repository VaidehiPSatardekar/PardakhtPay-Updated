using System;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    public class DashboardWidgetFactory : IDashboardWidgetFactory
    {
        IServiceProvider _ServiceProvider = null;

        public DashboardWidgetFactory(IServiceProvider provider)
        {
            _ServiceProvider = provider;
        }

        public IChartWidgetBuilder GetChartWidgetBuilder(WidgetType type)
        {
            switch (type)
            {
                case WidgetType.SalesReport:
                    return _ServiceProvider.GetRequiredService<ITransactionReportChartWidgetBuilder>();
                case WidgetType.Accounting:
                    return _ServiceProvider.GetRequiredService<IAccountingReportChartWidgetBuilder>();
                case WidgetType.DepositBreakDownReport:
                    return _ServiceProvider.GetRequiredService<IDepositBreakDownReportChartWidgetBuilder>();
                default:
                    throw new NotImplementedException();
            }
        }

        public IWidgetBuilder GetWidgetBuilder(WidgetType type)
        {
            switch (type)
            {
                case WidgetType.SalesReport:
                    return _ServiceProvider.GetRequiredService<ITransactionReportWidgetBuilder>();
                case WidgetType.SalesForMerchants:
                    return _ServiceProvider.GetRequiredService<IMerchantTransactionReportWidgetBuilder>();
                case WidgetType.AccountStatusReport:
                    return _ServiceProvider.GetRequiredService<IAccountStatusWidgetBuilder>();
                case WidgetType.WithdrawPaymentWidget:
                    return _ServiceProvider.GetRequiredService<ITransactionWithdrawalReportWidgetBuilder>();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

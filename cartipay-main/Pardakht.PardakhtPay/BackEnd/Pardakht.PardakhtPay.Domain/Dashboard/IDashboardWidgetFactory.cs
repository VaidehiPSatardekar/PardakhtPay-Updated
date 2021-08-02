using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    public interface IDashboardWidgetFactory
    {
        IWidgetBuilder GetWidgetBuilder(WidgetType type);
        IChartWidgetBuilder GetChartWidgetBuilder(WidgetType type);
    }
}

using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard.Interfaces
{
    public interface IWidgetBuilder
    {
        Task<DashboardWidget> Build(DashboardQuery query);
    }
}

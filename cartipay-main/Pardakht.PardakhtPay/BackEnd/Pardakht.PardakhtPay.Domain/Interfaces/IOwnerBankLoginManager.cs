using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IOwnerBankLoginManager : IBaseManager<OwnerBankLogin>
    {
        OwnerBankLogin GetLoginWithGuid(string loginGuid);

        OwnerBankLogin GetLoginWithLoginRequestId(int loginRequestId);

        Task<List<DailyAccountInformation>> GetDailyAccountInformations(DashboardQuery dashboardQuery);
    }
}

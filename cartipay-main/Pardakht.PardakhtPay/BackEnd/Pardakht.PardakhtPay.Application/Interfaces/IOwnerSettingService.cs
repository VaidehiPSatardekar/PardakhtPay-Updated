using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IOwnerSettingService : IServiceBase<OwnerSetting, IOwnerSettingManager>
    {
        Task<WebResponse<OwnerSettingDTO>> Get();

        Task<WebResponse<OwnerSettingDTO>> InsertAsync(OwnerSettingDTO item);

        Task<WebResponse<OwnerSettingDTO>> UpdateAsync(OwnerSettingDTO item);

        Task<WebResponse> DeleteAsync(int id);
    }
}

using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IApplicationSettingManager : IBaseManager<ApplicationSetting>
    {
        Task<ApplicationSetting> Get<T>();
    }
}

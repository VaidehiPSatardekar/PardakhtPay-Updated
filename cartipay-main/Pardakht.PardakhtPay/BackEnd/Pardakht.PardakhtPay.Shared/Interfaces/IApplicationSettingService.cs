using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface IApplicationSettingService
    {
        Task<T> Get<T>() where T : new();

        Task<WebResponse<ApplicationSettingsDTO>> GetAllSettings();

        Task<WebResponse<ApplicationSettingsDTO>> Update(ApplicationSettingsDTO model);
    }
}

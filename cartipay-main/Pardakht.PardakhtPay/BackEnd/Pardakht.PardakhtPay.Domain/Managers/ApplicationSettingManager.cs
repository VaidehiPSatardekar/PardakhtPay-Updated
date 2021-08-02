using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class ApplicationSettingManager : BaseManager<ApplicationSetting, IApplicationSettingRepository>, IApplicationSettingManager
    {
        IReflectionService _ReflectionService = null;

        public ApplicationSettingManager(IApplicationSettingRepository repository,
            IReflectionService reflectionService):base(repository)
        {
            _ReflectionService = reflectionService;
        }

        public async Task<ApplicationSetting> Get<T>()
        {
            var attributes = _ReflectionService.GetAttributes<T>();

            var attribute = attributes.OfType<SettingAttribute>().FirstOrDefault();

            return await Repository.GetItemAsync(t => t.Key == attribute.Key);
        }
    }
}

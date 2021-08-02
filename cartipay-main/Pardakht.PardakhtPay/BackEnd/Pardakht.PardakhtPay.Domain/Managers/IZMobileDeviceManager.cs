using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class IZMobileDeviceManager : BaseManager<IZMobileDevice, IIZMobileDeviceRepository>, IIZMobileDeviceManager
    {
        public IZMobileDeviceManager(IIZMobileDeviceRepository repository)
            :base(repository)
        {

        }
    }
}

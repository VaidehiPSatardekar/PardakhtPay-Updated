using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class SesDeviceManager : BaseManager<SesDevice, ISesDeviceRepository>, ISesDeviceManager
    {
        public SesDeviceManager(ISesDeviceRepository repository):base(repository)
        {

        }
    }
}

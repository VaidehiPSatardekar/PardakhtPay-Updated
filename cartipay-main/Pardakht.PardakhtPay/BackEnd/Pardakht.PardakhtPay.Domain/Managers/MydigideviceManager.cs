using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MydigiDeviceManager: BaseManager<MydigiDevice, IMydigiDeviceRepository>, IMydigiDeviceManager
    {
        public MydigiDeviceManager(IMydigiDeviceRepository repository)
            :base(repository)
        {

        }

        public async Task DeActivateDevices(string phoneNumber)
        {
            var items = await GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber);

            for (int i = 0; i < items.Count; i++)
            {
                var d = items[i];
                d.IsRegistered = false;
                d.TryCount = 0;

                await UpdateAsync(d);
            }

            await SaveAsync();
        }
    }
}

using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MobileTransferDeviceManager : BaseManager<MobileTransferDevice, IMobileTransferDeviceRepository>, IMobileTransferDeviceManager
    {
        ICachedObjectManager _CachedObjectManager = null;

        public MobileTransferDeviceManager(IMobileTransferDeviceRepository repository,
            ICachedObjectManager cachedObjectManager) : base(repository)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<MobileTransferDevice> DeactivateMobileDevice(string phoneNumber)
        {
            var deviceList = await Repository.GetItemsAsync(t => t.PhoneNumber == phoneNumber);

            foreach (var device in deviceList)
            {

                if (device != null)
                {
                    device.Status = (int)MobileTransferDeviceStatus.Removed;
                    device.TryCount = 0;

                    await Repository.UpdateAsync(device);
                }

            }

            await Repository.SaveChangesAsync();

            return deviceList.FirstOrDefault();
        }
        public async Task RemoveDeviceCustomerRelation(string phoneNumber, int merchantCustomerId)
        {
            var deviceList = await Repository.GetItemsAsync(t => t.PhoneNumber == phoneNumber && t.MerchantCustomerId == merchantCustomerId);

            foreach (var device in deviceList)
            {
                if (device != null)
                {
                    device.MerchantCustomerId = 0;
                    device.TryCount = 0;

                    await Repository.UpdateAsync(device);
                }
            }

            await Repository.SaveChangesAsync();
        }
    }
}

using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMobileTransferDeviceManager: IBaseManager<MobileTransferDevice>
    {
        Task<MobileTransferDevice> DeactivateMobileDevice(string phoneNumber);

        Task RemoveDeviceCustomerRelation(string phoneNumber, int merchantCustomerId);
    }
}

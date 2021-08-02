using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ISekehDeviceManager : IBaseManager<SekehDevice>
    {
        Task<SekehDevice> GetRandomDeviceAsync();

        Task DeActivateSekehDevices(string phoneNumber);

        Task RemoveSekehDeviceCustomerRelation(string phoneNumber, int merchantCustomerId);
    }
}

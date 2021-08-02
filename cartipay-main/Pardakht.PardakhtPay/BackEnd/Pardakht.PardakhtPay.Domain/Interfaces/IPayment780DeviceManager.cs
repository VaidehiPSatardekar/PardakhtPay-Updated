using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IPayment780DeviceManager : IBaseManager<Payment780Device>
    {
        Task<Payment780Device> GetRandomDeviceAsync();

        Task DeActivateDevices(string phoneNumber);

        Task RemoveDeviceCustomerRelation(string phoneNumber, int merchantCustomerId);
    }
}

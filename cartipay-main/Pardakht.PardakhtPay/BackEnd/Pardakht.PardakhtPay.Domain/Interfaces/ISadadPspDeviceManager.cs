using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ISadadPspDeviceManager : IBaseManager<SadadPspDevice>
    {
        Task<SadadPspDevice> GetRandomDeviceAsync();

        Task DeActivateDevices(string phoneNumber);
    }
}

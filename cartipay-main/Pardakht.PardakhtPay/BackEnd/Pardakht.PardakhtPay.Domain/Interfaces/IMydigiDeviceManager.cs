using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMydigiDeviceManager : IBaseManager<MydigiDevice>
    {
        Task DeActivateDevices(string phoneNumber);
    }
}

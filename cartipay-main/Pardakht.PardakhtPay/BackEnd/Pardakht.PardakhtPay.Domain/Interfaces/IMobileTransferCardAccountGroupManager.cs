using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMobileTransferCardAccountGroupManager: IBaseManager<MobileTransferCardAccountGroup>
    {
        Task<MobileTransferCardAccountGroupItem> GetRandomActiveAccount(Merchant merchant);
    }
}

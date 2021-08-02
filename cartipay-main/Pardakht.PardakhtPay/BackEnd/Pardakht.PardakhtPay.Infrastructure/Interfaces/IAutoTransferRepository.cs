using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Interfaces;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface IAutoTransferRepository : IGenericRepository<AutoTransfer>
    {
        Task<AutoTransfer> GetLatesAutoTranfer(int cardToCardAccountId);
    }
}

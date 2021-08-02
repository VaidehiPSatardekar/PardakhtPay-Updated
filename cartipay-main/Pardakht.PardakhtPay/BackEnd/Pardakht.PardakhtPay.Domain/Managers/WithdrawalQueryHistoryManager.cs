using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class WithdrawalQueryHistoryManager : BaseManager<WithdrawalQueryHistory, IWithdrawalQueryHistoryRepository>, IWithdrawalQueryHistoryManager
    {
        public WithdrawalQueryHistoryManager(IWithdrawalQueryHistoryRepository repository):base(repository)
        {

        }
    }
}

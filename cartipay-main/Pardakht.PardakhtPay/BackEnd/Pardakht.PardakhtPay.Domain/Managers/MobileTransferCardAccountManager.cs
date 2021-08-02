using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MobileTransferCardAccountManager : BaseManager<MobileTransferCardAccount, IMobileTransferCardAccountRepository>, IMobileTransferCardAccountManager
    {
        public MobileTransferCardAccountManager(IMobileTransferCardAccountRepository repository) : base(repository)
        {

        }
    }
}

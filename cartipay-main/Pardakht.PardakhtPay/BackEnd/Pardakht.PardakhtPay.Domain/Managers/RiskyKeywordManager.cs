using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class RiskyKeywordManager : BaseManager<RiskyKeyword, IRiskyKeywordRepository>, IRiskyKeywordManager
    {
        public RiskyKeywordManager(IRiskyKeywordRepository repository):base(repository)
        {

        }
    }
}

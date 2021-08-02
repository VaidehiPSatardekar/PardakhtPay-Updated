using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IBankStatementItemManager : IBaseManager<BankStatementItem>
    {
        Task<ListSearchResponse<List<BankStatementItemSearchDTO>>> Search(BankStatementSearchArgs args);

        Task UpdateStatementsWithTransaction(List<int> statementIds, int transactionId);

        Task<decimal> GetTotalCreditAmount(CardToCardAccount account);
    }
}

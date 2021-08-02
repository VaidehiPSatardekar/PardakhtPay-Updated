using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ICardToCardAccountManager : IBaseManager<CardToCardAccount>
    {
        Task<List<CardToCardAccount>> GetUsedCardToCardAccountsAsync(string loginGuid);

        Task<List<CardToCardAccount>> GetActiveCardToCardAccountsAsync();

        Task<List<CardToCardAccount>> GetCardToCardAccounts(string accountGuid);
    }
}

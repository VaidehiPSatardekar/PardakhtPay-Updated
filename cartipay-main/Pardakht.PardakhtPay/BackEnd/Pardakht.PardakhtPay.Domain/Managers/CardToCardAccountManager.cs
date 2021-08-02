using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class CardToCardAccountManager : BaseManager<CardToCardAccount, ICardToCardAccountRepository>, ICardToCardAccountManager
    {
        public CardToCardAccountManager(ICardToCardAccountRepository repository):base(repository)
        {

        }

        public async Task<List<CardToCardAccount>> GetActiveCardToCardAccountsAsync()
        {
            return await Repository.GetItemsAsync(t => t.IsActive);
        }

        public async Task<List<CardToCardAccount>> GetCardToCardAccounts(string accountGuid)
        {
            return await Repository.GetItemsAsync(t => t.AccountGuid == accountGuid);
        }

        public async Task<List<CardToCardAccount>> GetUsedCardToCardAccountsAsync(string loginGuid)
        {
            return await Repository.GetItemsAsync(t => t.LoginGuid == loginGuid);
        }
    }
}

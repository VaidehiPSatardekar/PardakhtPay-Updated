using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ICardToCardAccountService : IServiceBase<CardToCardAccount, ICardToCardAccountManager>
    {
        Task<WebResponse<List<CardToCardAccountDTO>>> GetAllItemsAsync();

        Task<WebResponse<CardToCardAccountDTO>> InsertAsync(CardToCardAccountDTO item);

        Task<WebResponse<CardToCardAccountDTO>> UpdateAsync(CardToCardAccountDTO item);

        Task<WebResponse<CardToCardAccountDTO>> GetItemById(int id);
    }
}

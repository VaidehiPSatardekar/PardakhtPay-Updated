using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ICardToCardAccountGroupService: IServiceBase<CardToCardAccountGroup, ICardToCardAccountGroupManager>
    {
        Task<WebResponse<List<CardToCardAccountGroupDTO>>> GetAllItemsAsync();

        Task<WebResponse<CardToCardAccountGroupDTO>> InsertAsync(CardToCardAccountGroupDTO item);

        Task<WebResponse<CardToCardAccountGroupDTO>> UpdateAsync(CardToCardAccountGroupDTO item);

        Task<WebResponse<CardToCardAccountGroupDTO>> GetItemById(int id);

        Task<WebResponse> CheckPausedAccounts();
    }
}

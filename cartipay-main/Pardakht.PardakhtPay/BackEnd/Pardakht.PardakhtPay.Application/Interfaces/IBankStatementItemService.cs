using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IBankStatementItemService : IServiceBase<BankStatementItem, IBankStatementItemManager>
    {
        Task<WebResponse<List<BankStatementItemDTO>>> GetAllItemsAsync();

        Task<WebResponse<BankStatementItemDTO>> InsertAsync(BankStatementItemDTO item);

        Task<WebResponse> InsertAsync(List<BankStatementItemDTO> item);

        Task<WebResponse<BankStatementItemDTO>> UpdateAsync(BankStatementItemDTO item);

        Task<WebResponse<BankStatementItemDTO>> GetItemById(int id);

        Task<WebResponse<ListSearchResponse<List<BankStatementItemSearchDTO>>>> Search(BankStatementSearchArgs args);
    }
}

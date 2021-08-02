using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ILanguageService : IServiceBase<Language, ILanguageManager>
    {
        Task<WebResponse<List<LanguageDTO>>> GetAllItemsAsync();

        Task<WebResponse<LanguageDTO>> InsertAsync(LanguageDTO item);

        Task<WebResponse<LanguageDTO>> UpdateAsync(LanguageDTO item);

        Task<WebResponse<LanguageDTO>> GetItemById(int id);
    }
}

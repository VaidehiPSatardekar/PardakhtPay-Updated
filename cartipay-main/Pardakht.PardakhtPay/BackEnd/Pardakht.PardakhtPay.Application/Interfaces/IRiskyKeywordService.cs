using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IRiskyKeywordService: IServiceBase<RiskyKeyword, IRiskyKeywordManager>
    {
        Task<WebResponse<string[]>> GetAll();

        Task<WebResponse<string[]>> Update(string[] values);
    }
}

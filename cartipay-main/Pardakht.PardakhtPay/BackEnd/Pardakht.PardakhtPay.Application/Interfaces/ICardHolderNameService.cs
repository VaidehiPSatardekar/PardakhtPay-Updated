using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ICardHolderNameService : IServiceBase<CardHolderName, ICardHolderNameManager>
    {
        Task<WebResponse> UpdateCardHolderName(CardHolderNameResponse response);

        Task<WebResponse<CardHolderNameDTO>> GetCardHolderName(string cardNumber);

        Task<WebResponse<CardHolderNameDTO>> GetIbanName(string iban);

        Task<WebResponse<CardHolderNameDTO>> GetAccountName(CardHolderNameDTO model);
    }
}

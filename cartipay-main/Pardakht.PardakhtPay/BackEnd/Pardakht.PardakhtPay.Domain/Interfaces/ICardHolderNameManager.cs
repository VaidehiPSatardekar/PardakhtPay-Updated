using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ICardHolderNameManager : IBaseManager<CardHolderName>
    {
        Task ProcessForCardNumber(string ecryptedCardnumber);

        Task ProcessForIbanNumber(string ecryptedIbannumber);

        Task<CardHolderName> GetCardHolderName(string clearCardNo, string callbackUrl, string accountContext);

        Task<CardHolderName> GetIbanName(string clearIbanNo, string callbackUrl, string accountContext);
    }
}

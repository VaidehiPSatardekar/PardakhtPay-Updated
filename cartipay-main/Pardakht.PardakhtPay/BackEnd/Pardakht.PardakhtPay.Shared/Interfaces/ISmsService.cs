using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface ISmsService
    {
        Task<SmsServiceSendResponse> SendSms(SmsServiceRequest request);

        Task<SmsServiceBalanceResponse> CheckBalance(SmsServiceBalanceRequest request);
    }
}

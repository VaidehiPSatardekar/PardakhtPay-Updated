using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Shared.Services
{
    public class SmsMockService : ISmsService
    {
        public async Task<SmsServiceSendResponse> SendSms(SmsServiceRequest request)
        {
            return await Task.Run(() =>
            {
                return new SmsServiceSendResponse()
                {
                    IsSent = true,
                    ValidationEndDate = DateTime.UtcNow.AddMinutes(5)
                };
            });
        }

        public Task<SmsServiceBalanceResponse> CheckBalance(SmsServiceBalanceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

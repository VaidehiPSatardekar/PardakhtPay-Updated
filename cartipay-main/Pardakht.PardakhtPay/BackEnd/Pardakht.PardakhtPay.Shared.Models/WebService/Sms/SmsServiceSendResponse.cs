using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Sms
{
    public class SmsServiceSendResponse
    {
        public bool IsSent { get; set; }

        public DateTime ValidationEndDate { get; set; }
    }
}

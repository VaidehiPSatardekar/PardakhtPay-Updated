using System;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    [Setting(Key = ApplicationSettingKeys.SmsService)]
    public class SmsServiceConfiguration : BaseEntityDTO
    {
        public string Url { get; set; }

        public string ApiKey { get; set; }

        public string SmsApiKey { get; set; }

        public string SmsSecretKey { get; set; }

        public string TemplateId { get; set; }

        public bool Mock { get; set; }

        public bool UseSmsConfirmation { get; set; }

        public TimeSpan ExpireTime { get; set; }

        public int Seconds { get; set; }

        public bool SaveDevices { get; set; }

        public int MaximumTryCountForRegisteringDevice { get; set; }
    }
}

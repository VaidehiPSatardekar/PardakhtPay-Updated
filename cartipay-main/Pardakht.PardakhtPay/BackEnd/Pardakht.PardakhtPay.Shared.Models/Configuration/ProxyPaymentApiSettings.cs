using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class ProxyPaymentApiSettings
    {
        public string Url { get; set; }

        public string SamanBankParameterName { get; set; }

        public string MeliPaymentParameterName { get; set; }

        public string ZarinpalParameterName { get; set; }

        public string MellatParameterName { get; set; }

        public string NovinParameterName { get; set; }

        public string ApiKey { get; set; }

        public bool EnabledSwitching { get; set; }

        public TimeSpan SwitchingInterval { get; set; }
    }
}

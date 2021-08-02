namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class BankBotConfiguration
    {
        public string Url { get; set; }

        public string ApiKey { get; set; }

        public bool Mock { get; set; }

        public string CallbackUrl { get; set; }

        public string ConfirmUrl { get; set; }
    }
}

using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class ApplicationSettingsDTO
    {
        public SmsServiceConfiguration SmsConfiguration { get; set; }

        public MaliciousCustomerSettings MaliciousCustomerSettings { get; set; }

        public BankAccountConfiguration BankAccountConfiguration { get; set; }

        public MobileApiConfiguration MobileApiConfiguration { get; set; }
    }
}

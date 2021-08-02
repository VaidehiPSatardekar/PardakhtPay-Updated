using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    [Setting(Key = ApplicationSettingKeys.BankAccount)]
    public class BankAccountConfiguration: BaseEntityDTO
    {
        public long BlockAccountLimit { get; set; }

        public string TestingAccounts { get; set; }

        public bool UseSameWithdrawalAccountForCustomer { get; set; }
    }
}

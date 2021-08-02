using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TenantBalanceDTO
    {
        public string BankName { get; set; }

        public decimal Amount { get; set; }

        public string OwnerGuid { get; set; }
    }

    public class TenantBalanceSearchDTO
    {
        public List<string> AccountGuids { get; set; }
    }
}

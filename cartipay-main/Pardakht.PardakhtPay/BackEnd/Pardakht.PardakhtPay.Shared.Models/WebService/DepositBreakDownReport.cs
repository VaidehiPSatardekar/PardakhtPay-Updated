using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DepositBreakDownReport
    {
        public DateTime BreakDownDate { get; set; }
        public decimal SamanBank { get; set; }
        public decimal MeliBank { get; set; }
        public decimal Zarinpal { get; set; }
        public decimal Mellat { get; set; }
        public decimal Novin { get; set; }
    }

    public class DepositBreakDownData
    {
        public DateTime BreakDownDate { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }

    }


}

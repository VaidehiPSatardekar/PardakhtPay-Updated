using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoiceOwnerSettingDTO : BaseEntityDTO
    {
        public bool IsActive { get; set; }

        public string OwnerGuid { get; set; }

        public DateTime StartDate { get; set; }

        public int InvoicePeriod { get; set; }

        public decimal PardakhtPayDepositRate { get; set; }

        public decimal PardakhtPalDepositRate { get; set; }

        public decimal PardakhtPalWithdrawalRate { get; set; }

        public decimal WithdrawalRate { get; set; }

        public string TenantGuid { get; set; }
    }
}

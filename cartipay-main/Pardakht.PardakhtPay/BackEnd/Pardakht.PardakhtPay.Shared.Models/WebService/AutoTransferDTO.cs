using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class AutoTransferDTO : BaseEntityDTO
    {
        public int? BankLoginId { get; set; }

        public int? BankAccountId { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public int RequestId { get; set; }

        public string RequestGuid { get; set; }

        public int CardToCardAccountId { get; set; }

        public string AccountGuid { get; set; }

        public string TransferFromAccount { get; set; }

        public string TransferToAccount { get; set; }

        public int Priority { get; set; }

        public DateTime TransferRequestDate { get; set; }

        public string TransferRequestDateStr { get; set; }

        public DateTime? TransferredDate { get; set; }

        public string TransferredDateStr { get; set; }

        public DateTime? CancelDate { get; set; }

        public string CancelDateStr { get; set; }

        public bool IsCancelled { get; set; }

        public string FriendlyName { get; set; }
    }

    public class AutoTransferSearchArgs : AgGridSearchArgs
    {
        public List<string> Tenants { get; set; }

        public string DateRange { get; set; }
    }
}

using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class OwnerBankLoginDTO : BaseEntityDTO
    {
        public string FriendlyName { get; set; }

        public string OwnerGuid { get; set; }

        public string BankLoginGuid { get; set; }

        public int BankLoginId { get; set; }

        public string TenantGuid { get; set; }

        public bool IsActive { get; set; }

        public int Status { get; set; }

        public int LoginType { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }
    }
}

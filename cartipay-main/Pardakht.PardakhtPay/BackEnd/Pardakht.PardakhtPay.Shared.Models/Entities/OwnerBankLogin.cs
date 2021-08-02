using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class OwnerBankLogin : BaseEntity, ITenantGuid, IOwnerGuid
    {
        public string FriendlyName { get; set; }

        public string OwnerGuid { get; set; }

        public string BankLoginGuid { get; set; }

        public int BankLoginId { get; set; }

        public string TenantGuid { get; set; }

        public bool IsActive { get; set; }

        public int Status { get; set; }

        public int LoginRequestId { get; set; }

        public string AccountNumbers { get; set; }

        public bool IsDeleted { get; set; }

        public int LoginType { get; set; }

        public int BankId { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        [NotMapped]
        public OwnerBankLoginStatus LoginStatus
        {
            get
            {
                return (OwnerBankLoginStatus)Status;
            }
            set
            {
                Status = (int)value;
            }
        }
    }

    public enum OwnerBankLoginStatus
    {
        WaitingInformation = 1,
        Success = 2,
        Error = 3,
        WaitingApprovement = 4
    }

    public enum LoginType
    {
        Card2Card = 1,
        TransferOnly = 2,
        InfoOnly = 3
    }
}

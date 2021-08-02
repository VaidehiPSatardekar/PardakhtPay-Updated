using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MobileTransferDevice : BaseEntity, ITenantGuid
    {
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string VerificationCode { get; set; }

        public int Status { get; set; }

        public DateTime? VerifyCodeSendDate { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public int? ExternalId { get; set; }

        public string ExternalStatus { get; set; }

        public string TenantGuid { get; set; }

        public DateTime? LastBlockDate { get; set; }

        public bool IsActive { get; set; }

        public int MerchantCustomerId { get; set; }

        public int TryCount { get; set; }
    }

    public enum MobileTransferDeviceStatus
    {
        Created = 1,
        VerifyCodeSended = 2,
        PhoneNumberVerified = 3,
        Removed = 4,
        Error = 5,
        Unkown = 6
    }
}

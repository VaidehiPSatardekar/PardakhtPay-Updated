using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer
{
    public class MobileTransferDeviceDTO : BaseEntityDTO
    {
        public string PhoneNumber { get; set; }

        public string VerificationCode { get; set; }

        public int Status { get; set; }

        public DateTime? VerifyCodeSendDate { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public int? ExternalId { get; set; }

        public string ExternalStatus { get; set; }

        public string TenantGuid { get; set; }

        public bool IsActive { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Sms
{
    public class ExternalSendSmsConfirmationRequest
    {
        //[Required]
        public string ApiKey { get; set; }

        [Required]
        public string WebsiteName { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }

    public class ExternalSmsVerifiyRequest
    {
        //[Required]
        public string ApiKey { get; set; }

        [Required]
        public string WebsiteName { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }

    public class ExternalSendSmsConfirmationResult
    {
        public int MerchantCustomerId { get; set; }

        public DateTime? ConfirmationEndDate { get; set; }
    }

    public class ExternalSendSmsVerifyResult
    {
        public bool IsWrongCode { get; set; }
    }

    public class ReplaceUserIdRequest
    {
        public string ApiKey { get; set; }

        public string OldUserId { get; set; }

        public string NewUserId { get; set; }

        public string WebsiteName { get; set; }
    }
}

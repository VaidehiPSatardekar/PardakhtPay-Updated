using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Transaction : BaseEntity, ITenantGuid, IOwnerGuid
    {
        public DateTime CreationDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TransactionAmount { get; set; }

        public int MerchantId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CardNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string AccountNumber { get; set; }

        public DateTime? TransferredDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Token { get; set; }

        [MaxLength(1000)]
        public string CustomerCardNumber { get; set; }

        [MaxLength(500)]
        public string BankNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string ReturnUrl { get; set; }

        public int Status { get; set; }

        [MaxLength(300)]
        public string CardHolderName { get; set; }

        public int? CardToCardAccountId { get; set; }

        public int? MobileTransferAccountId { get; set; }

        [MaxLength(70)]
        [Required]
        public string TenantGuid { get; set; }

        [MaxLength(70)]
        [Required]
        public string OwnerGuid { get; set; }

        [MaxLength(70)]
        public string AccountGuid { get; set; }

        [MaxLength(70)]
        public string LoginGuid { get; set; }

        [MaxLength(100)]
        //[NotMapped]
        public string Description { get; set; }

        [NotMapped]
        [MaxLength(2000)]
        public string RequestContent { get; set; }

        public int? MerchantCustomerId { get; set; }

        public bool IsMaliciousCustomer { get; set; }

        public int? UserSegmentGroupId { get; set; }

        public bool HideCardNumber { get; set; }

        public int? ExternalId { get; set; }

        [MaxLength(2000)]
        public string ExternalMessage { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        public int PaymentType { get; set; }

        [MaxLength(50)]
        public string MobileDeviceNumber { get; set; }

        public int? WithdrawalId { get; set; }

        public bool IsPhoneNumberBlocked { get; set; }

        [MaxLength(70)]
        public string UpdateUserId { get; set; }

        public int ApiType { get; set; }

        [StringLength(100)]
        public string ExternalReference { get; set; }

        public int? ProxyPaymentAccountId { get; set; }

        public int ProcessDurationInMiliseconds { get; set; }
        [NotMapped]
        public string UserSegmentGroupName { get; set; }

        [NotMapped]
        public TransactionStatus TransactionStatus
        {
            get
            {
                return (TransactionStatus)Status;
            }
            set
            {
                Status = (int)value;
            }
        }

        [NotMapped]
        public ApiType ApiTypeAsEnum
        {
            get
            {
                return (ApiType)ApiType;
            }
            set
            {
                ApiType = (int)value;
            }
        }
    }

    public enum TransactionStatus
    {
        Started = 1,
        TokenValidatedFromWebSite = 2,
        WaitingConfirmation = 3,
        Completed = 4,
        Expired = 5,
        Cancelled = 6,
        Fraud = 7,
        Reversed = 8
    }

    public enum PaymentType
    {
        CardToCard = 1,
        Mobile = 2,
        SamanBank = 3,
        MeliBank = 4,
        Zarinpal = 5,
        Mellat = 6,
        Novin = 7
    }

    public enum ApiType
    {
        AsanPardakht = 1,
        HamrahCard = 2,
        Sekeh = 3,
        Ses = 4,
        SadadPsp = 5,
        Mydigi = 6,
        IZMobile = 7,
        Payment780 = 8
    }
}

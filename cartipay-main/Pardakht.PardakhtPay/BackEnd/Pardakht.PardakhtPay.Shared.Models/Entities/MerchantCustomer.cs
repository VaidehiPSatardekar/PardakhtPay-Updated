using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MerchantCustomer : BaseEntity, ITenantGuid, IOwnerGuid
    {
        [StringLength(50)]
        [Required]
        public string TenantGuid { get; set; }

        [StringLength(50)]
        [Required]
        public string OwnerGuid { get; set; }

        public int MerchantId { get; set; }

        [StringLength(200)]
        [Required]
        public string WebsiteName { get; set; }

        [StringLength(1000)]
        [Required]
        public string UserId { get; set; }

        public DateTime? RegisterDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalDeposit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalWithdraw { get; set; }

        public int? DepositNumber { get; set; }

        public int? WithdrawNumber { get; set; }

        public int? ActivityScore { get; set; }

        public string GroupName { get; set; }

        public DateTime? LastActivity { get; set; }

        public int? CardToCardAccountId { get; set; }

        public int? UserSegmentGroupId { get; set; }

        public int? WithdrawalAccountId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UserTotalSportbook { get; set; }

        public int? UserSportbookNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UserTotalCasino { get; set; }

        public int? UserCasinoNumber { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public bool? IsConfirmed { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public string ConfirmCode { get; set; }

        public DateTime? ConfirmCodeValidationEndDate { get; set; }

        public string PhoneNumberRelatedCustomers { get; set; }

        public int DifferentCardNumberCount { get; set; }

        public string DeviceRelatedCustomers { get; set; }

        public string CardNumberRelatedCustomers { get; set; }

        public int SmsVerificationType { get; set; }

        public int SmsVerificationTryCount { get; set; }

        [MaxLength(20)]
        public string ConfirmedPhoneNumber { get; set; }

        public int PardakhtPayDepositCount { get; set; }


        public decimal PardakhtPayDepositAmount { get; set; }

        public int PardakhtPayWithdrawalCount { get; set; }

        public decimal PardakhtPayWithdrawalAmount { get; set; }

        public int HamrahCardTryCount { get; set; }

        [MaxLength(20)]
        public string HamrahCardVerifiedPhoneNumber { get; set; }

        public bool IsHamrahCardPhoneVerified { get; set; }
    }

    public enum SmsVerificationType
    {
        SmsService = 1,
        AsanPardakht = 2,
        HamrahCard = 3,
        Sekeh = 4,
        Ses = 5,
        SadatPsp = 6,
        Mydigi = 7,
        IZMobile = 8,
        Payment780 = 9
    }
}

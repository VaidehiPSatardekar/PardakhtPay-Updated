using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class UserSegment : BaseEntity
    {
        public int UserSegmentGroupId { get; set; }

        public int UserSegmentCompareTypeId { get; set; }

        public int UserSegmentTypeId { get; set; }

        public string Value { get; set; }

        [NotMapped]
        public UserSegmentCompareType UserSegmentCompareType
        {
            get
            {
                return (UserSegmentCompareType)UserSegmentCompareTypeId;
            }
            set
            {
                UserSegmentCompareTypeId = (int)value;
            }
        }

        [NotMapped]
        public UserSegmentType UserSegmentType
        {
            get
            {
                return (UserSegmentType)UserSegmentTypeId;
            }
            set
            {
                UserSegmentTypeId = (int)value;
            }
        }
    }

    public enum UserSegmentCompareType
    {
        LessThan = 1,
        LessThanAndEquals = 2,
        Equals = 3,
        NotEqual = 4,
        MoreThanAndEquals = 5,
        MoreThan = 6
    }

    public enum UserSegmentType
    {
        TotalDepositCountPardakhtPay = 1,
        TotalDepositAmountPardakhtPay = 2,
        //WaitingPaymentTransactionCount = 3,
        //ExpiredTransactionCount = 4,
        TotalWithdrawalCountPardakhtPay = 5,
        TotalWithdrawalCountMerchant = 6,
        TotalWithdrawalAmountPardakhtPay = 7,
        TotalWithdrawalAmountMerchant = 8,
        TotalDepositCountMerchant = 9,
        TotalDepositAmountMerchant = 10,
        RegistrationDate = 11,
        GroupName = 12,
        ActivityScore = 13,
        LastActivity = 14,
        WebsiteName = 15,
        TotalSportbookAmount = 16,
        TotalSportbookCount = 17,
        TotalCasinoAmount = 18,
        TotalCasinoCount = 19
    }
}

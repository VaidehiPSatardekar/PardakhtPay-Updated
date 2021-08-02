using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class UserSegmentGroupDTO : BaseEntityDTO
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDefault { get; set; }

        public bool IsMalicious { get; set; }

        public DateTime CreateDate { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int Order { get; set; }

        public List<UserSegmentDTO> Items { get; set; }

        public UserSegmentGroupDTO()
        {
            Items = new List<UserSegmentDTO>();
        }
    }

    public class UserSegmentDTO : BaseEntityDTO
    {
        public int UserSegmentGroupId { get; set; }

        public int UserSegmentCompareTypeId { get; set; }

        public int UserSegmentTypeId { get; set; }

        public string Value { get; set; }
    }

    public class UserSegmentValues
    {
        /// <summary>
        /// 1
        /// </summary>
        public int TotalDepositCountPardakhtPay { get; set; }

        /// <summary>
        /// 2
        /// </summary>
        public decimal TotalDepositAmountPardakhtPay { get; set; }

        ///// <summary>
        ///// 3
        ///// </summary>
        //public int WaitingPaymentCount { get; set; }

        ///// <summary>
        ///// 4
        ///// </summary>
        //public int ExpiredDepositCount { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        public int TotalWithdrawalCountPardakhtPay { get; set; }

        /// <summary>
        /// 6
        /// </summary>
        public int TotalWithdrawalCountMerchant { get; set; }

        /// <summary>
        /// 7
        /// </summary>
        public decimal TotalWithdrawalAmountPardakhtPay { get; set; }

        /// <summary>
        /// 8
        /// </summary>
        public decimal TotalWithdrawalAmountMerchant { get; set; }

        /// <summary>
        /// 9
        /// </summary>
        public int TotalDepositCountMerchant { get; set; }

        /// <summary>
        /// 10
        /// </summary>
        public decimal TotalDepositAmountMerchant { get; set; }

        /// <summary>
        /// 11
        /// </summary>
        public DateTime? RegisterDate { get; set; }

        /// <summary>
        /// 12
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 13
        /// </summary>
        public int ActivityScore { get; set; }

        /// <summary>
        /// 14
        /// </summary>
        public DateTime? LastActivity { get; set; }

        /// <summary>
        /// 15
        /// </summary>
        public string WebsiteName { get; set; }

        /// <summary>
        /// 16
        /// </summary>
        public decimal TotalSportbookAmount { get; set; }

        /// <summary>
        /// 17
        /// </summary>
        public int TotalSportbookCount { get; set; }

        /// <summary>
        /// 18
        /// </summary>
        public decimal TotalCasinoAmount { get; set; }

        /// <summary>
        /// 19
        /// </summary>
        public int TotalCasinoCount { get; set; }
    }
}

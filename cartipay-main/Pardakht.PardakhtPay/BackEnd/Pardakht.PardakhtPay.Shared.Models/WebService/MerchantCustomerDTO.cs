using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class MerchantCustomerDTO : BaseEntityDTO
    {
        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int MerchantId { get; set; }

        public string WebsiteName { get; set; }

        public string UserId { get; set; }

        public DateTime? RegisterDate { get; set; }

        public decimal? TotalDeposit { get; set; }

        public decimal? TotalWithdraw { get; set; }

        public int? DepositNumber { get; set; }

        public int? WithdrawNumber { get; set; }

        public int? ActivityScore { get; set; }

        public string GroupName { get; set; }

        public DateTime? LastActivity { get; set; }

        public int? CardToCardAccountId { get; set; }

        public int TotalTransactionCount { get; set; }

        public int TotalCompletedTransactionCount { get; set; }

        public decimal? TotalDepositAmount { get; set; }

        public decimal? TotalWithdrawalAmount { get; set; }

        public int TotalWithdrawalCount { get; set; }

        public int TotalCompletedWithdrawalCount { get; set; }

        public int? UserSegmentGroupId { get; set; }

        public decimal? UserTotalSportbook { get; set; }

        public int? UserSportbookNumber { get; set; }

        public decimal? UserTotalCasino { get; set; }

        public int? UserCasinoNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneNumberRelatedCustomers { get; set; }

        public int DifferentCardNumberCount { get; set; }

        public string DeviceRelatedCustomers { get; set; }

        public string CardNumberRelatedCustomers { get; set; }

        public bool PhoneNumberIsBlocked { get; set; }
    }

    public class MerchantCustomerPhoneNumberDTO: BaseEntityDTO
    {
        public string WebsiteName { get; set; }

        public string UserId { get; set; }

        public string MerchantTitle { get; set; }
    }

    public class MerchantCustomerRelationDTO : BaseEntityDTO
    {
        public string WebsiteName { get; set; }

        public string UserId { get; set; }

        public string MerchantTitle { get; set; }

        public string RelationKey { get; set; }

        public string Value { get; set; }
    }

    public class MerchantCardNumbersDTO
    {
        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public int Count { get; set; }
    }

    public class MerchantCustomerSearchArgs : AgGridSearchArgs
    {
        public Dictionary<string, dynamic> FilterModel { get; set; }
        public string WebsiteName { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class PhoneNumbersResponse
    {
        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        public string WebSite { get; set; }
    }

    public class RegisteredPhoneNumbers
    {
        public string PhoneNumber { get; set; }
    }

    public class RegisteredPhoneNumbersList
    {
        public string[] PhoneNumber { get; set; }
    }
}

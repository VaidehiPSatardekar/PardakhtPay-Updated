using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TransactionSearchDTO
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        public string MerchantTitle { get; set; }

        public decimal TransactionAmount { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime? TransferredDate { get; set; }

        public string TransactionDateStr { get; set; }

        public string TransferredDateStr { get; set; }

        [Encrypt]
        public string AccountNumber { get; set; }

        [Encrypt]
        public string MerchantCardNumber { get; set; }

        [Encrypt]
        public string CustomerCardNumber { get; set; }

        [Encrypt]
        public string CardHolderName { get; set; }

        public string BankNumber { get; set; }

        public int Status { get; set; }

        public string TenantGuid { get; set; }

        public string UserId { get; set; }

        public string WebsiteName { get; set; }

        public string Reference { get; set; }

        public int? UserSegmentGroupId { get; set; }

        public int? MerchantCustomerId { get; set; }

        public int? PaymentType { get; set; }

        public int? ExternalId { get; set; }

        public string ExternalMessage { get; set; }

        public int? WithdrawalId { get; set; }

        public int ProcessId { get; set; }

        public string UserSegmentGroupName { get; set; }
    }

    public class TransactionSearchArgs : AgGridSearchArgs
    {
        public List<int> Statuses { get; set; }

        public List<string> Tenants { get; set; }

        public int? PaymentType { get; set; }

        public string DateRange { get; set; }

        public string Token { get; set; }

        public int? MerchantCustomerId { get; set; }

        public int? WithdrawalId { get; set; }

        public Dictionary<string, dynamic> FilterModel { get; set; }
    }

    public class TransactionReportSearchArgs
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TimeZoneInfoId { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeZoneInfoId))
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId);
                }

                return TimeZoneInfo.Utc;
            }
        }
    }
}

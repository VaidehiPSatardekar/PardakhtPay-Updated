using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class WithdrawalSearchDTO
    {
        public int Id { get; set; }

        public int? BankLoginId { get; set; }

        public int? BankAccountId { get; set; }

        public string AccountGuid { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int MerchantId { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpectedTransferDate { get; set; }

        public DateTime? TransferRequestDate { get; set; }

        public DateTime? TransferDate { get; set; }

        public string ExpectedTransferDateStr { get; set; }

        public string TransferRequestDateStr { get; set; }

        public string TransferDateStr { get; set; }

        public string CancelDateStr { get; set; }

        [Encrypt]
        public string FromAccountNumber { get; set; }

        [Encrypt]
        public string ToAccountNumber { get; set; }

        [Encrypt]
        public string ToIbanNumber { get; set; }

        [Encrypt]
        public string FirstName { get; set; }

        [Encrypt]
        public string LastName { get; set; }

        [Encrypt]
        public string CardHolderName { get; set; }

        public int TransferStatus { get; set; }

        public string TransferStatusDescription { get; set; }

        [Encrypt]
        public string CardNumber { get; set; }

        public int Priority { get; set; }

        public string TransferNotes { get; set; }

        public int TransferType { get; set; }

        public int WithdrawalStatus { get; set; }

        public string Reference { get; set; }

        public string UserId { get; set; }

        public string WebsiteName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string FriendlyName { get; set; }

        public string TrackingNumber { get; set; }

        public string Description { get; set; }

        public int? MerchantCustomerId { get; set; }

        public int? BankStatementItemId { get; set; }

        public int? TransactionId { get; set; }

        public int WithdrawalProcessType { get; set; }

        public int CardToCardTryCount { get; set; }

        public decimal RemainingAmount { get; set; }

        public decimal? PendingApprovalAmount { get; set; }

        public int? TransferId { get; set; }

        public bool IsLoginBlocked { get; set; }
    }

    public class WithdrawalSearchArgs : AgGridSearchArgs
    {
        public int? Status { get; set; }

        public Dictionary<string, dynamic> FilterModel { get; set; }

        //public string DateRange { get; set; }

        //public string UserId { get; set; }

        //public string WebsiteName { get; set; }

        //public string Reference { get; set; }

        //public string TrackingNumber { get; set; }

        public int? MerchantCustomerId { get; set; }

        //public int? Id { get; set; }

        public int[] Merchants { get; set; }

        //public string CardNumber { get; set; }
    }

    public class WithdrawalPaymentReportArgs
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
                    return System.TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId);
                }

                return TimeZoneInfo.Utc;
            }
        }
    }

    public enum WithdrawalStatusSearch
    {
        Pending = 1,
        Transfered = 2,
        Cancelled = 3,
        Sent = 4,
        Confirmed = 5,
        Refund = 6,
        PendingApproval = 7,
        PartialPaid = 8
    }
}

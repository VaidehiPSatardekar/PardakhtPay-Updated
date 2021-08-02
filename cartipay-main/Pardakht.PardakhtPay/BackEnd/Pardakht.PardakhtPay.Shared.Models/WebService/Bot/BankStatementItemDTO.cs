using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BankStatementItemDTO : BaseEntityDTO
    {
        public int RecordId { get; set; }

        public DateTime CreationDate { get; set; }

        public int AccountId { get; set; }

        public string AccountGuid { get; set; }

        public int LoginId { get; set; }

        public string LoginGuid { get; set; }

        public string TransactionNo { get; set; }

        public string CheckNo { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        public string Description { get; set; }

        public DateTime InsertDateTime { get; set; }

        public string Notes { get; set; }
        public int TransferRequestId { get; set; }

        public int WithdrawalId { get; set; }
    }

    public class BankStatementItemSearchDTO //: BaseEntityDTO
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public int AccountId { get; set; }

        public string AccountGuid { get; set; }

        public string TransactionNo { get; set; }

        public string CheckNo { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public string TransactionDateTimeStr { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        public string Description { get; set; }

        public DateTime InsertDateTime { get; set; }

        public DateTime? UsedDate { get; set; }

        public string UsedDateStr { get; set; }

        public int? TransactionId { get; set; }

        public string Notes { get; set; }

        public bool IsRisky { get; set; }

        public int? WithdrawalId { get; set; }
    }

    public class BankStatementSearchArgs : AgGridSearchArgs
    {
        public Dictionary<string, dynamic> FilterModel { get; set; }

        public int? StatementItemType { get; set; }

        public List<string> AccountGuids { get; set; }

        public bool? IsRisky { get; set; }
    }

    public class BankStatementReportSearchArgs
    {
        public List<string> AccountGuids { get; set; }

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

    public enum StatementItemTypes
    {
        Credit = 1,
        Debit = 2,
        Unconfirmed = 3,
        All = 4
    }
}

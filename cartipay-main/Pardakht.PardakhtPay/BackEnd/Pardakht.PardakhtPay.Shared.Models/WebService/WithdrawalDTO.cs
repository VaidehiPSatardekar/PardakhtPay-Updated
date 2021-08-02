using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class WithdrawalDTO : BaseEntityDTO
    {
        public decimal Amount { get; set; }

        //public int MerchantId { get; set; }

        //public int BankAccountId { get; set; }

        public string TenantGuid { get; set; }

        public int TransferAccountId { get; set; }

        public string FromAccountNumber { get; set; }

        public string ToAccountNumber { get; set; }

        public int Priority { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TransferRequestGuid { get; set; }

        public string TransferNotes { get; set; }

        public int TransferStatus { get; set; }

        //public int TransferType { get; set; }

        public string OwnerGuid { get; set; }

        public string Description { get; set; }
    }

    public class WithdrawRequestDTO
    {
        public int amount { get; set; }

        public string api_key { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string account_number { get; set; }

        public string iban { get; set; }

        [Encrypt]
        public string card_number { get; set; }

        public string user_id { get; set; }

        public string website_name { get; set; }

        public string return_url { get; set; }

        public int priority { get; set; }

        public string transfer_time { get; set; }

        public string reference { get; set; }

        public string user_register_date { get; set; }

        public long? user_total_deposit { get; set; }

        public long? user_total_withdraw { get; set; }

        public int? user_deposit_number { get; set; }

        public int? user_withdraw_number { get; set; }

        public int user_activity_score { get; set; }

        public string user_group_name { get; set; }

        public string user_last_activity { get; set; }

        public long? user_total_sportbook { get; set; }

        public int? user_sportbook_number { get; set; }

        public long? user_total_casino { get; set; }

        public int? user_casino_number { get; set; }

        public string description { get; set; }

        public int process_type { get; set; }
    }

    public class WithdrawRequestResponseDTO
    {
        public WithdrawRequestResult Result { get; set; }

        public string ResultDescription
        {
            get
            {
                return Result.GetDescription();
            }
        }

        public int Id { get; set; }
    }

    public class WithdrawCheckRequest
    {
        public int withdrawal_id { get; set; }

        public string api_key { get; set; }
    }

    public class WithdrawCancelRequest
    {
        public int withdrawal_id { get; set; }

        public string api_key { get; set; }
    }

    public class WithdrawCheckResponseDTO
    {
        public int Id { get; set; }

        public WithdrawRequestResult Result { get; set; }

        public string TransferNotes { get; set; }

        public string TrackingNumber { get; set; }

        public decimal Amount { get; set; }

        public string ReturnUrl { get; set; }

        public string Reference { get; set; }

        public List<WithdrawalTransactionItem> Payments { get; set; }

        public WithdrawCheckResponseDTO(WithdrawRequestResult result, List<WithdrawalTransactionItem> payments = null)
        {
            Result = result;
            Payments = new List<WithdrawalTransactionItem>();
        }

        public string ResultDescription
        {
            get
            {
                return Result.GetDescription();
            }
        }
    }

    public class WithdrawalTransactionItem
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public int Amount { get; set; }

        public string Name { get; set; }

        public string TransactionNumber { get; set; }

        public DateTime? Date { get; set; }
    }

    public class WithdrawCompletedResponse
    {
        public int withdrawal_id { get; set; }

        public int status { get; set; }

        public string transfer_notes { get; set; }

        public string tracking_number { get; set; }

        public string reference { get; set; }

        public int amount { get; set; }

        public List<WithdrawalTransactionItem> payments { get; set; }
    }

    public class WithdrawException : Exception
    {
        public WithdrawRequestResult Result { get; set; }

        public List<WithdrawalTransactionItem> Payments { get; set; }

        public WithdrawException(WithdrawRequestResult result, List<WithdrawalTransactionItem> payments = null)
        {
            Result = result;
            Payments = payments;
        }

        public int ResultCode
        {
            get
            {
                return (int)Result;
            }
        }

        public override string Message
        {
            get
            {
                return Result.GetDescription();
            }
        }
    }

    public class WithdrawSuccessResponse
    {
        public int status { get; set; }

        public int withdrawal_id { get; set; }
    }

    public class WithdrawErrorResponse
    {
        public int status { get; set; }

        public int errorCode { get; set; }

        public string errorDescription { get; set; }

        public string reference { get; set; }

        public int amount { get; set; }

        public List<WithdrawalTransactionItem> payments { get; set; }
    }

    public enum WithdrawRequestResult
    {
        Success = 1,
        ApiKeyIsNull = 101,
        ApiKeyIsInvalid = 101,
        WithdrawlIdIsNotValid = 200,
        TokenIsUsed = 200,
        AmountMustBiggerThanZero = 102,
        TokenIsNotVerified = 200,
        NotCompleted = 201,
        Cancelled = 202,
        CallbackUrlIsInvalid = 103,
        AccountNotFound = 206,
        InvalidAccountOrIban = 207,
        InvalidDateTime = 208,
        InvalidFirstOrLastName = 209,
        ReferenceRequired = 210,
        RequestSent = 211,
        Unsuccessful = 212,
        StatusNotSuitable = 213,
        PriorityInvalid = 214,
        Refund = 204,
        AwaitingConfirmation = 205,
        Invalid = 215,
        InvalidCardNumber = 216,
        PartialPaid = 217,
        TransferFailed = 218,
        UnknownError = 999
    }

    public class PendingWithdrawalAmount
    {
        public string AccountGuid { get; set; }

        public int TransferType { get; set; }

        public decimal? Amount { get; set; }
    }
}

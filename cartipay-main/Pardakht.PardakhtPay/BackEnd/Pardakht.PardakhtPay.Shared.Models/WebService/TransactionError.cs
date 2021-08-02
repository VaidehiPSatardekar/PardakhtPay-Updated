using System;
using Pardakht.PardakhtPay.Shared.Models.Extensions;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public enum TransactionResultEnum
    {
        Success = 1,
        ApiKeyIsNull = 101,
        ApiKeyIsInvalid = 101,
        TokenIsNotValid = 200,
        TokenIsExpired = 200,
        TokenIsUsed = 200,
        AmountMustBiggerThanZero = 102,
        TokenIsNotVerified = 200,
        TransactionNotConfirmed = 201,
        TransactionNotCompleted = 201,
        TransactionCancelled = 202,
        TransactionIsExpired = 210,
        CallbackUrlIsInvalid = 103,
        SameCardNumber = 203,
        TransactionReversed = 204,
        InvalidWebSite = 205,
        InvalidCardNumber = 206,
        AccountNotFound = 207,
        SmsConfirmationNeeded = 208,
        DeviceNotFound = 209,
        UserIdNotFound = 211,
        UnknownError = 999
    }

    public class TransactionException : Exception
    {
        public TransactionResultEnum Result { get; set; }

        public override string Message
        {
            get
            {
                return Result.GetDescription();
            }
        }

        public int ResultCode
        {
            get
            {
                return (int)Result;
            }
        }

        public TransactionException()
        {
            Result = TransactionResultEnum.Success;
        }

        public TransactionException(TransactionResultEnum result)
        {
            Result = result;
        }

        public TransactionResult CreateResult()
        {
            return new TransactionResult(Result);
        }

        public TransactionResult<T> CreateResult<T>()
        {
            return new TransactionResult<T>(Result);
        }
    }

    public class TransactionResult
    {
        public TransactionResultEnum Result { get; set; }

        public int ResultCode
        {
            get
            {
                return (int)Result;
            }
        }

        public string ErrorDescription
        {
            get
            {
                return Result.GetDescription();
            }
        }

        public TransactionResult()
        {
            Result = TransactionResultEnum.Success;
        }

        public TransactionResult(TransactionResultEnum result)
        {
            Result = result;
        }
    }

    public class TransactionCheckResult : TransactionResult
    {
        public int Amount { get; set; }

        public string BankNumber { get; set; }

        public string ReturnUrl { get; set; }

        public string CardNumber { get; set; }

        public int PaymentType { get; set; }

        public int Status { get; set; }

        public TransactionCheckResult(TransactionResultEnum result):base(result)
        {

        }
    }

    public class TransactionResult<T> : TransactionResult
    {

        public TransactionResult(T item):base()
        {
            Item = item;
        }

        public TransactionResult(TransactionResultEnum result):base(result)
        {
            Item = default(T);
        }

        public TransactionResult(TransactionResultEnum result, T item):base(result)
        {
            Item = item;
        }

        public T Item { get; set; }
    }
}

using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Models.Extensions
{
    public static class WithdrawExtentions
    {

        public static readonly int[] CallbackStatuses = new int[] {
            (int)TransferStatus.Complete,
            (int)TransferStatus.AwaitingConfirmation,
            (int)TransferStatus.Cancelled,
            (int)TransferStatus.RefundFromBank,
            (int)TransferStatus.CompletedWithNoReceipt,
            (int)TransferStatus.AccountNumberInvalid,
            (int)TransferStatus.FailedFromBank,
            (int)TransferStatus.InvalidIBANNumber };

        public static string GetDescription(this WithdrawRequestResult result)
        {
            switch (result)
            {
                case WithdrawRequestResult.Success:
                    return "Successful";
                case WithdrawRequestResult.ApiKeyIsNull:
                    return "Api key is empty or invalid";
                case WithdrawRequestResult.WithdrawlIdIsNotValid:
                    return "withdrawal Id is invalid";
                case WithdrawRequestResult.AmountMustBiggerThanZero:
                    return "Amount must be bigger then zero";
                case WithdrawRequestResult.NotCompleted:
                    return "Operation hasn't completed yet";
                case WithdrawRequestResult.Cancelled:
                    return "Request is cancelled";
                case WithdrawRequestResult.CallbackUrlIsInvalid:
                    return "Return url is invalid";
                case WithdrawRequestResult.AccountNotFound:
                    return "Account not found";
                case WithdrawRequestResult.InvalidAccountOrIban:
                    return "Invalid account number or iban";
                case WithdrawRequestResult.UnknownError:
                    return "Unhandled exception occurred";
                case WithdrawRequestResult.InvalidDateTime:
                    return "Invalid date time";
                case WithdrawRequestResult.InvalidFirstOrLastName:
                    return "Invalid first or last name";
                case WithdrawRequestResult.ReferenceRequired:
                    return "Reference field is required";
                case WithdrawRequestResult.RequestSent:
                    return "Transfer request has been sent to the bank";
                case WithdrawRequestResult.Unsuccessful:
                    return "Transfer is unsuccessful";
                case WithdrawRequestResult.StatusNotSuitable:
                    return "Status is not suitable for this operation";
                case WithdrawRequestResult.PriorityInvalid:
                    return "Priority must be between 1 and 3";
                case WithdrawRequestResult.Refund:
                    return "Refunded";
                case WithdrawRequestResult.AwaitingConfirmation:
                    return "Awaiting Confirmation";
                case WithdrawRequestResult.Invalid:
                    return "Invalid";
                case WithdrawRequestResult.InvalidCardNumber:
                    return "Invalid Card Number";
                case WithdrawRequestResult.TransferFailed:
                    return "Transfer Failed";
                default:
                    break;
            }

            return string.Empty;
        }

        public static bool NeedCallback(this Withdrawal withdrawal)
        {
            return CallbackStatuses.Contains(withdrawal.TransferStatus);
        }
    }
}

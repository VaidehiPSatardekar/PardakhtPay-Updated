using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Models.Extensions
{
    public static class TransactionExtensions
    {
        public static string GetDescription(this TransactionResultEnum result)
        {
            switch (result)
            {
                case TransactionResultEnum.Success:
                    return "Successful";
                case TransactionResultEnum.ApiKeyIsNull:
                    return "API key is null or invalid";
                case TransactionResultEnum.TokenIsNotValid:
                    return "Token is not valid";
                case TransactionResultEnum.AmountMustBiggerThanZero:
                    return "Amount must be bigger than zero";
                case TransactionResultEnum.TransactionNotConfirmed:
                    return "Transaction not confirmed";
                case TransactionResultEnum.UnknownError:
                    return "Unknown exception occured";
                case TransactionResultEnum.TransactionCancelled:
                    return "Transaction cancelled by user or bank";
                case TransactionResultEnum.TransactionIsExpired:
                    return "Transaction is expired";
                case TransactionResultEnum.CallbackUrlIsInvalid:
                    return "Callback url is invalid";
                case TransactionResultEnum.SameCardNumber:
                    return "Customer entered the same card number with the merchant";
                case TransactionResultEnum.TransactionReversed:
                    return "Transaction was reversed by the customer";
                case TransactionResultEnum.InvalidWebSite:
                    return "Invalid web site";
                case TransactionResultEnum.InvalidCardNumber:
                    return "Invalid card number";
                case TransactionResultEnum.AccountNotFound:
                    return "Account not found";
                case TransactionResultEnum.DeviceNotFound:
                    return "در حال حاضر این خدمت قابل ارايه نمی باشد";
                case TransactionResultEnum.UserIdNotFound:
                    return "UserId or website name not found";
                default:
                    return string.Empty;
            }
        }
    }
}

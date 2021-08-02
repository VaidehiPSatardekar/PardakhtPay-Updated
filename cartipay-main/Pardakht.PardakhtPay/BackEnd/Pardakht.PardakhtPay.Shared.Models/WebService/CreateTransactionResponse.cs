using Pardakht.PardakhtPay.Shared.Models.Extensions;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{

    public class TransactionResponse
    {

        public TransactionResultEnum ResultCode { get; set; }

        public string ResultDescription
        {
            get
            {
                return ResultCode.GetDescription();
            }
        }

        public TransactionResponse()
        {
            ResultCode = TransactionResultEnum.Success;
        }

        public TransactionResponse(TransactionResultEnum result)
        {
            ResultCode = result;
        }

        public TransactionResponse(int resultCode) : this((TransactionResultEnum)resultCode)
        {
        }
    }

    public class CreateTransactionResponse : TransactionResponse
    {

        public string Token { get; set; }

        public CreateTransactionResponse():base()
        {

        }

        public CreateTransactionResponse(TransactionResultEnum result):base(result)
        {

        }

        public CreateTransactionResponse(int resultCode):base(resultCode)
        {

        }

        public CreateTransactionResponse(int resultCode, string token):base(resultCode)
        {
            Token = token;
        }
    }

    public class TransactionPaymentInformationResponse : TransactionResponse
    {
        public TransactionPaymentInformationResponse(TransactionResultEnum result):base(result)
        {
        }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string CustomerCardNumber { get; set; }

        public int Amount { get; set; }

        public string ReturnUrl { get; set; }

        public string Token { get; set; }

        public int TransactionId { get; set; }

        public string CaptchaCode { get; set; }

        public string CaptchaImageData { get; set; }

        public string BeautiyCardNumber
        {
            get
            {
                if (string.IsNullOrEmpty(CardNumber))
                {
                    return string.Empty;
                }

                if(CardNumber.Length != 16)
                {
                    return CardNumber;
                }

                return CardNumber.Substring(0, 4) + " " + CardNumber.Substring(4, 4) + " " + CardNumber.Substring(8, 4) + " " + CardNumber.Substring(12, 4);
            }
        }
    }

    public class CompletePaymentResponse : TransactionResponse
    {
        public int Amount { get; set; }

        public string BankNumber { get; set; }

        public string CardNumber { get; set; }

        public string Token { get; set; }

        public string ReturnUrl { get; set; }

        public int PaymentType { get; set; }

        public string Message { get; set; }

        public bool DeviceDeactivated { get; set; }

        public int InProcess { get; set; }

        public bool IsProcessed { get; set; }

        public CompletePaymentResponse(TransactionResultEnum result):base(result)
        {

        }
    }

    public class CheckTransactionResponse : TransactionResponse
    {
        public int Amount { get; set; }

        public string BankNumber { get; set; }

        public CheckTransactionResponse(TransactionResultEnum result):base(result)
        {

        }
    }

    public class CancelTransactionResponse : TransactionResponse
    {
        public string ReturnUrl { get; set; }

        public string Token { get; set; }
    }
}

using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IPaymentTransactionService
    {
        Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress);

        Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request);

        Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request);

        Task<TransactionCheckResult> Check(string token);

        Task<TransactionCheckResult> Check(string token, string apiKey);

        Task<TransactionResult<Transaction>> CancelTransaction(string token);

        Task CheckPhoneNumber(Transaction transaction, string phoneNumber);

        Task<bool> SendOtp(Transaction transaction, string cardNumber, string captcha);

        Task<bool> CheckCardNumber(Transaction transaction, string customerCardNumber);
    }

    public interface ICardToCardPaymentService : IPaymentTransactionService
    {

    }

    public interface IMobilePaymentService: IPaymentTransactionService
    {

    }

    public interface IPaymentProxyApiService : IPaymentTransactionService
    {
        Task InitializePayment(Transaction transaction);

        Task Transfer(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response, Merchant merchant);
    }
}

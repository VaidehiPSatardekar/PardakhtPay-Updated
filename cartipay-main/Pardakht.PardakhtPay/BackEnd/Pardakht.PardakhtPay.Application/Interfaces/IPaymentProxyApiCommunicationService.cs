using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IPaymentProxyApiCommunicationService
    {
        Task<InitializeTransactionResponse> InitializeTransaction(InitializeTransactionRequest request, int transactionId);

        Task<SendOtpResponse> SendOtp(SendOtpRequest request, int transactionId);

        Task<SendPaymentResponse> SendPayment(SendPaymentRequest request, int transactionId);

        Task<ConfirmPaymentResponse> ConfirmPayment(ConfirmPaymentRequest request, int transactionId);
    }
}

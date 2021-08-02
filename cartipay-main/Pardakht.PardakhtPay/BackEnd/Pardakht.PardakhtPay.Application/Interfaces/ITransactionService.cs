using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ITransactionService : IServiceBase<Transaction, ITransactionManager>
    {
        Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress);

        Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request);

        Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request);

        Task<TransactionCheckResult> Check(string token);

        Task<TransactionCheckResult> Check(string token, string apiKey);

        Task<TransactionResult<Transaction>> CancelTransaction(string token);

        Task<WebResponse<ListSearchResponse<IEnumerable<TransactionSearchDTO>>>> Search(TransactionSearchArgs args);

        Task<WebResponse<IEnumerable<DailyAccountingDTO>>> SearchAccounting(AccountingSearchArgs args);

        Task<WebResponse<SmsPhoneNumberModel>> SendSms(SmsPhoneNumberModel model);

        Task<WebResponse<ExternalSendSmsConfirmationResult>> ExternalSendCustomerSms(ExternalSendSmsConfirmationRequest request);

        Task<WebResponse<ExternalSendSmsVerifyResult>> ExternalVerifyCustomerSms(ExternalSmsVerifiyRequest request);

        Task<WebResponse<SmsVerifyModel>> VerifySms(SmsVerifyModel model);

        Task<WebResponse> CompleteTransaction(int id);

        Task<WebResponse> ExpireTransaction(int id);

        Task<double> GetTimeoutDuration(string token);

        Task<MobileTransferResponse> SendOtpMessage(MobileTransferStartTransferModel model);

        Task<WebResponse<string>> TransactionCallbackToMerchant(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ITransactionManager : IBaseManager<Transaction>
    {
        Task<TransactionPaymentInformationWithTransaction> GetTransactionPaymentInformation(string token);

        Task<TransactionResult<Transaction>> CheckTransactionReadyForComplete(string token);

        Task<Transaction> GetTransactionByToken(string token);

        Transaction GetLastTransaction(int merchantCustomerId, int paymentType);

        Task<ListSearchResponse<IEnumerable<TransactionSearchDTO>>> Search(TransactionSearchArgs args);

        Task<IEnumerable<DailyAccountingDTO>> SearchAccounting(AccountingSearchArgs args);

        Task<Transaction> GetTransactionByReference(int merchantId, string reference);

        Task<List<WithdrawalTransactionItem>> GetCompletedWithdrawalTransactions(int withdrawalId);

        Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate);

        Task<List<Transaction>> GetUnconfirmedTransactions(DateTime startDate, DateTime endDate, int[] apiTypes);

        Task<decimal> GetTotalPaymentAmountForPaymentGateway(MobileTransferCardAccount account);

        Task<string> TransactionCallbackToMerchant(int id);

        string GetCacheKeyForMobileVerification(string token);

        void SetCacheForMobileVerification(string token);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IWithdrawalManager : IBaseManager<Withdrawal>
    {

        Task<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>> Search(WithdrawalSearchArgs args);

        Task<Withdrawal> GetOldItem(int merchantId, string reference);

        Task<Withdrawal> CheckWithdrawalStatus(int id);

        Task<Withdrawal> CheckWithdrawalStatus(Withdrawal item, bool force = false, BankStatementItemDTO statement=null);

        Task<List<Withdrawal>> GetUncompletedWithdrawals(DateTime date);

        Task<List<Withdrawal>> GetUnprocessedWithdrawals();

        Task ConfirmWithdrawals(DateTime startDate);

        Task<List<PendingWithdrawalAmount>> GetPendingWithdrawalAmounts();

        Task<List<PendingWithdrawalAmount>> GetPendingWithdrawalBalance();

        Task<Withdrawal> SetAsCompleted(int id);

        Task<Withdrawal> SetAsCompleted(int id, decimal amount);

        Task<Withdrawal> ChangeProcessType(int id, int processType);

        Task ChangeProcessType(WithdrawalSearchArgs args, int processType);

        Task<Withdrawal> Retry(int id, Transaction transaction);

        Task<List<WithdrawalTransferHistoryDTO>> GetWithdrawalTransferHistories(int withdrawalId);

        Task<WithdrawalTransferHistoryDTO> GetCompletedWithdrawalTransferHistory(int withdrawalId);

        Task InsertContentAsync(Withdrawal item);

        Task<string> WithdrawalCallbackToMerchant(int id, Transaction transaction);

        
    }
}

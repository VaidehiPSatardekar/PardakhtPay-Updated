using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IWithdrawalService : IServiceBase<Withdrawal, IWithdrawalManager>
    {
        //Task<WebResponse<List<WithdrawalDTO>>> GetAllItemsAsync();

        Task<WebResponse<WithdrawalDTO>> InsertAsync(WithdrawalDTO item);

        //Task<WebResponse<WithdrawalDTO>> UpdateAsync(WithdrawalDTO item);

        Task<WebResponse<WithdrawalDTO>> GetItemById(int id);

        Task<WebResponse<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>>> Search(WithdrawalSearchArgs args);

        Task<WithdrawRequestResponseDTO> CreateWithdrawRequest(WithdrawRequestDTO request);

        Task<WithdrawCheckResponseDTO> Check(WithdrawCheckRequest request);

        Task<WithdrawCheckResponseDTO> Cancel(WithdrawCancelRequest request);

        Task<WebResponse<Withdrawal>> Cancel(int id);

        Task<WebResponse<Withdrawal>> Retry(int id);

        Task<WebResponse<Withdrawal>> SetAsCompleted(int id);

        Task<WithdrawCheckResponseDTO> Check(int id);

        Task<WebResponse<BankBotTransferReceiptResponse>> GetTransferReceipt(int id);

        Task<WebResponse<Withdrawal>> CheckRefund(int transferRequestId);

        Task<System.Net.HttpStatusCode?> SendCallback(int id);

        Task<System.Net.HttpStatusCode?> SendCallback(WithdrawCheckResponseDTO result, int withdrawalId);

        Task CheckWithTransferRequestId(int transferRequestId, BankStatementItemDTO statement);

        Task<WebResponse<Withdrawal>> ChangeProcessType(int id, int processId);

        Task<WebResponse> ChangeProcessType(WithdrawalSearchArgs args, int processId);

        Task<WebResponse<List<WithdrawalTransferHistoryDTO>>> GetWithdrawalHistories(int id);

        Task<WebResponse<List<long>>> GetSuggestedWithdrawalAmounts(string apiKey, long amount);

        Task<WebResponse<string>> WithdrawalCallbackToMerchant(int id);
    }
}

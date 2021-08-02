using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IManualTransferManager: IBaseManager<ManualTransfer>
    {
        Task<ListSearchResponse<List<ManualTransferDTO>>> Search(ManualTransferSearchArgs args);

        Task Check(DateTime startDate);

        Task<List<ManualTransfer>> GetUnProcessedItemsAsync();

        Task Process(int id, List<BankBotAccountWithStatusDTO> accounts, List<BotBankInformation> banks, List<BotLoginInformation> logins);

        Task Process(ManualTransfer transfer, List<BankBotAccountWithStatusDTO> accounts, List<BotBankInformation> banks, List<BotLoginInformation> logins, bool processDetails = false);

        Task<List<ManualTransferDetail>> GetDetails(int id);

        Task<BankBotTransferReceiptResponse> GetTransferReceipt(int id);

        Task<ManualTransferDetail> GetDetail(int id);
        Task InsertSourceCardDetails(List<ManualTransferSourceCardDetails> sourceCardDetails);

        Task DeleteSourceCardDetails(int manualTransferId);

        
    }
}

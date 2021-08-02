using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.ExternalServices.Queue
{
    public interface ITransactionQueueService
    {
        Task InsertQueue(BotQueueItem queueItem);

        Task InsertQueue(BotQueueItem queueItem, TimeSpan delay);

        Task InsertCallbackQueueItem(CallbackQueueItem queueItem);

        Task InsertCallbackQueueItem(CallbackQueueItem queueItem, TimeSpan delay);

        Task InsertWithdrawalCallbackQueueItem(WithdrawalQueueItem queueItem);

        Task InsertWithdrawalCallbackQueueItem(WithdrawalQueueItem queueItem, TimeSpan delay);

        Task InsertMobileTransferQueueItem(MobileTransferQueueItem queueItem);

        Task InsertMobileTransferQueueItem(MobileTransferQueueItem queueItem, TimeSpan delay);
    }
}

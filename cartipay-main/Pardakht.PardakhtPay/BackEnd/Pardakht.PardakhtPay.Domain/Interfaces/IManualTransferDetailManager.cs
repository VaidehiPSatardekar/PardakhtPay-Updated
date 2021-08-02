using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IManualTransferDetailManager : IBaseManager<ManualTransferDetail>
    {
        Task<List<ManualTransferDetail>> GetUnProcessedItems();

        Task Process(ManualTransferDetail item, string accountGuid);

        Task Check(int id);

        Task Check(ManualTransferDetail item);

        Task<List<ManualTransferDetail>> GetUnCompletedItems(DateTime startDate);

        Task<ManualTransferDetail> Cancel(int id);

        Task<ManualTransferDetail> Cancel(ManualTransferDetail item);

        Task<ManualTransferDetail> Retry(int id);

        Task<ManualTransferDetail> Retry(ManualTransferDetail item);

        Task<ManualTransferDetail> SetAsCompleted(int id);

        Task<ManualTransferDetail> SetAsCompleted(ManualTransferDetail item);
    }
}

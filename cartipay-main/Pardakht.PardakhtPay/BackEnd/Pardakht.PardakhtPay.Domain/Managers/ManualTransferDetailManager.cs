using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.WebService.BankBot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class ManualTransferDetailManager : BaseManager<ManualTransferDetail, IManualTransferDetailRepository>, IManualTransferDetailManager
    {
        IBankBotService _BankBotService;
        ILogger _Logger;
        IManualTransferRepository _TransferRepository;
        IAesEncryptionService _EncryptionService;

        public ManualTransferDetailManager(IManualTransferDetailRepository repository,
            IBankBotService bankBotService,
            IManualTransferRepository transferRepository,
            IAesEncryptionService encryptionService,
            ILogger<ManualTransferDetailManager> logger):base(repository)
        {
            _BankBotService = bankBotService;
            _TransferRepository = transferRepository;
            _EncryptionService = encryptionService;
            _Logger = logger;
        }

        public async Task<List<ManualTransferDetail>> GetUnProcessedItems()
        {
            var cancelledRequests = _TransferRepository.GetQuery(t => t.Status == (int)ManualTransferStatus.Cancelled
            || t.Status == (int)ManualTransferStatus.CancelledByBlockedAccount
            || t.Status == (int)ManualTransferStatus.CancelledByDeletedAccount
            || t.ImmediateTransfer).Select(p => p.Id);

            return await Repository.GetItemsAsync(t => t.TransferStatus == (int)TransferStatus.NotSent && !cancelledRequests.Contains(t.ManualTransferId) && t.Amount > 0);
        }

        public async Task<List<ManualTransferDetail>> GetUnCompletedItems(DateTime startDate)
        {
            var statuses = new int[]{ (int)TransferStatus.NotSent, (int)TransferStatus.Complete, (int)TransferStatus.Cancelled, (int)TransferStatus.RejectedDueToBlokedAccount, (int)TransferStatus.CompletedWithNoReceipt };
            var cancelledRequests = _TransferRepository.GetQuery(t => t.Status == (int)ManualTransferStatus.Cancelled || t.Status == (int)ManualTransferStatus.CancelledByBlockedAccount || t.Status == (int)ManualTransferStatus.CancelledByDeletedAccount).Select(p => p.Id);

            return await Repository.GetItemsAsync(t => !statuses.Contains(t.TransferStatus) && !cancelledRequests.Contains(t.ManualTransferId) && t.TransferRequestDate.HasValue && t.TransferRequestDate >= startDate);
        }

        public async Task Process(ManualTransferDetail item, string accountGuid=null)
        {
            if(item.TransferStatus != (int)TransferStatus.NotSent)
            {
                return;
            }

            var transfer = await _TransferRepository.GetEntityByIdAsync(item.ManualTransferId);
            var account = await _BankBotService.GetAccountByGuid(accountGuid);

            var accountNo = _EncryptionService.DecryptToString(transfer.ToAccountNo);

            if(transfer.TransferType != (int)TransferType.Normal)
            {
                accountNo = _EncryptionService.DecryptToString(transfer.Iban);
            }

            var result = await _BankBotService.CreateTransferRequest(new BankBotTransferRequestDTO()
            {
                FirstName = _EncryptionService.DecryptToString(transfer.FirstName),
                LastName = _EncryptionService.DecryptToString(transfer.LastName),
                TransferBalance = Convert.ToInt64(item.Amount),
                TransferFromAccount = account.AccountNo,
                TransferPriority = transfer.Priority,
                TransferToAccount = accountNo,
                TransferType = transfer.TransferType
            });

            item.TransferGuid = result.TransferRequestGuid;
            item.TransferId = result.Id;
            item.TransferNotes = result.TransferNotes;
            item.TransferStatus = result.TransferStatus;
            item.UpdateDate = DateTime.UtcNow;

            await UpdateAsync(item);
            await SaveAsync();
        }

        public async Task Check(int id)
        {
            var item = await GetEntityByIdAsync(id);

            if(item == null)
            {
                throw new Exception($"Item could not be found with id {id}");
            }

            await Check(item);
        }

        public async Task Check(ManualTransferDetail item)
        {
            if (!item.TransferId.HasValue)
            {
                return;
            }

            if (item.TransferStatus == (int)TransferStatus.Complete)
            {
                return;
            }

            var response = await _BankBotService.GetTransferRequestWithStatus(item.TransferId.Value);

            if (response == null || response.TransferStatus == (int)TransferStatus.Cancelled)
            {
                item.TransferStatus = (int)TransferStatus.Cancelled;
                item.UpdateDate = DateTime.UtcNow;

                item = await UpdateAsync(item);
                await SaveAsync();
            }
            else
            {
                if (response.TransferStatus != item.TransferStatus)
                {
                    item.TransferStatus = response.TransferStatus;
                    item.TransferDate = DateTime.UtcNow;
                    item.TransferRequestDate = response.TransferRequestDate;
                    item.TransferGuid = response.TransferRequestGuid;
                    item.UpdateDate = DateTime.UtcNow;

                    item = await UpdateAsync(item);
                    await SaveAsync();

                    if (item.TransferStatus == (int)TransferStatus.Complete || item.TransferStatus == (int)TransferStatus.AwaitingConfirmation || item.TransferStatus == (int)TransferStatus.CompletedWithNoReceipt)
                    {
                        try
                        {
                            var history = await _BankBotService.GetTransferHistory(response.Id);

                            if (history != null)
                            {
                                item.TrackingNumber = history.TrackingNumber;
                                item.UpdateDate = DateTime.UtcNow;

                                item = await UpdateAsync(item);
                                await SaveAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            _Logger.LogError(ex, ex.Message);
                        }
                    }
                }
            }
        }

        public async Task<ManualTransferDetail> Cancel(int id)
        {
            var detail = await GetEntityByIdAsync(id);

            return await Cancel(detail);
        }

        public async Task<ManualTransferDetail> Cancel(ManualTransferDetail item)
        {
            if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.TransferId.HasValue)
            {
                var response = await _BankBotService.CancelTransferRequest(item.TransferId.Value);

                item.TransferStatus = response.TransferStatus;
            }
            else
            {
                item.TransferStatus = (int)TransferStatus.Cancelled;
            }

            await UpdateAsync(item);
            await SaveAsync();

            return item;
        }

        public async Task<ManualTransferDetail> Retry(int id)
        {
            var detail = await GetEntityByIdAsync(id);

            return await Retry(detail);
        }

        public async Task<ManualTransferDetail> Retry(ManualTransferDetail item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.TransferId.HasValue)
            {
                var response = await _BankBotService.RetryTransferRequest(item.TransferId.Value);

                item.TransferStatus = response.TransferStatus;
            }
            else
            {
                throw new Exception("This request hasn't been sent to the bank bot yet");
            }

            await UpdateAsync(item);
            await SaveAsync();

            return item;
        }

        public async Task<ManualTransferDetail> SetAsCompleted(int id)
        {
            var detail = await GetEntityByIdAsync(id);

            return await SetAsCompleted(detail);
        }

        public async Task<ManualTransferDetail> SetAsCompleted(ManualTransferDetail item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.TransferId.HasValue)
            {
                var response = await _BankBotService.SetAsCompletedTransferRequest(item.TransferId.Value);

                item.TransferStatus = response.TransferStatus;
            }
            else
            {
                throw new Exception("This request hasn't been sent to the bank bot yet");
            }

            await UpdateAsync(item);
            await SaveAsync();

            return item;
        }


    }
}

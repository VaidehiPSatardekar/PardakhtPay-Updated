using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BotAutoTransferService.CheckManagers
{
    public class CheckTransferManager : ICheckTransferManager
    {
        IAutoTransferManager _Manager;
        AutoTransferSettings _Settings;
        IBankBotService _BankBotService = null;
        ILogger<CheckTransferManager> _Logger;

        public CheckTransferManager(IAutoTransferManager manager,
            IOptions<AutoTransferSettings> autoTransferOptions,
            IBankBotService bankBotService,
            ILogger<CheckTransferManager> logger)
        {
            _Manager = manager;
            _Settings = autoTransferOptions.Value;
            _BankBotService = bankBotService;
            _Logger = logger;
        }

        public async Task Run()
        {
            var items = await _Manager.GetUncompletedTransfers();

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    var item = items[i];

                    BankBotTransferRequest transfer = null;

                    transfer = await _BankBotService.GetTransferRequestWithStatus(item.RequestId);

                    if (transfer == null)
                    {
                        await _Manager.Cancel(item);
                        await _Manager.SaveAsync();
                    }
                    else
                    {
                        bool transferred = transfer.TransferStatus == (int)TransferStatus.Complete || transfer.TransferStatus == (int)TransferStatus.CompletedWithNoReceipt || transfer.TransferStatus == (int)TransferStatus.BankSubmitted || transfer.TransferStatus == (int)TransferStatus.AwaitingConfirmation;
                        bool cancelled = transfer.TransferStatus == (int)TransferStatus.Cancelled;

                        _Logger.LogInformation($"{item.Id} {transfer.TransferStatus} {transferred}");

                        if (transferred)
                        {
                            item.TransferredDate = DateTime.UtcNow;
                            item.Status = transfer.TransferStatus;
                            item.StatusDescription = transfer.TransferStatusDescription;

                            await _Manager.UpdateAsync(item);
                            await _Manager.SaveAsync();
                        }
                        else if (cancelled)
                        {
                            item.Status = transfer.TransferStatus;
                            item.StatusDescription = transfer.TransferStatusDescription;

                            await _Manager.Cancel(item);
                            await _Manager.SaveAsync();
                        }
                        else
                        {

                            if (item.TransferRequestDate.Add(_Settings.TransferCancelInterval) <= DateTime.UtcNow && transfer.TransferStatus != (int)TransferStatus.AwaitingConfirmation)
                            {
                                _Logger.LogInformation($"Cancelling {item.Id}");
                                transfer = await _BankBotService.CancelTransferRequest(item.RequestId);

                                item.Status = transfer.TransferStatus;
                                item.StatusDescription = transfer.TransferStatusDescription;

                                await _Manager.Cancel(item);
                                await _Manager.SaveAsync();
                            }
                            else
                            {
                                item.Status = transfer.TransferStatus;
                                item.StatusDescription = transfer.TransferStatusDescription;

                                await _Manager.UpdateAsync(item);
                                await _Manager.SaveAsync();
                            }
                        }
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

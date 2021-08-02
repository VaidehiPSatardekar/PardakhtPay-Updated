using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class PardakhtPalOperationManager : IPardakhtPalOperationManager
    {
        MobileTransferConfiguration _Configuration = null;
        IMobileTransferService _MobileTransferService = null;
        ITransactionManager _TransactionManager = null;
        ILogger _Logger = null;
        ITransactionService _TransactionService = null;
        IAesEncryptionService _AesEncryptionManager = null;
        ICachedObjectManager _CachedObjectManager = null;
        ITransactionQueueService _TransactionQueueService = null;

        const int UnhandledExceptionCode = 555;

        public PardakhtPalOperationManager(IOptions<MobileTransferConfiguration> mobileOptions,
            IMobileTransferService mobileTransferService,
            ITransactionManager transactionManager,
            ILogger<PardakhtPalOperationManager> logger,
            ITransactionService transactionService,
            IAesEncryptionService aesEncryptionService,
            ICachedObjectManager cachedObjectManager,
            ITransactionQueueService transactionQueueService)
        {
            _Configuration = mobileOptions.Value;
            _MobileTransferService = mobileTransferService;
            _TransactionManager = transactionManager;
            _Logger = logger;
            _TransactionService = transactionService;
            _AesEncryptionManager = aesEncryptionService;
            _CachedObjectManager = cachedObjectManager;
            _TransactionQueueService = transactionQueueService;
        }

        public async Task Run()
        {
            try
            {
                var startDate = DateTime.UtcNow.Subtract(_Configuration.StartTimeInterval);
                var endDate = DateTime.UtcNow.Subtract(_Configuration.EndTimeInterval);

                var items = await _TransactionManager.GetUnconfirmedTransactions(startDate, endDate, _Configuration.ApiTypes);

                for (int i = 0; i < items.Count; i++)
                {
                    try
                    {
                        var response = await _MobileTransferService.CheckTransferStatusAsync(new Shared.Models.MobileTransfer.MobileTransferStartTransferModel()
                        {
                            TransactionToken = items[i].Token,
                            ApiType = items[i].ApiType,
                            MobileNo = items[i].MobileDeviceNumber,
                            UniqueId = items[i].ExternalMessage,
                            FromCardNo = _AesEncryptionManager.DecryptToString(items[i].CustomerCardNumber),
                            ToCardNo = _AesEncryptionManager.DecryptToString(items[i].CardNumber)
                        });

                        if (response.IsSuccess)
                        {
                            _Logger.LogInformation($"Transaction {items[i].Id} is completing {JsonConvert.SerializeObject(response)}");
                            await _TransactionService.CompleteTransaction(items[i].Id);
                            await InformBot(items[i]);
                        }
                        else if(response.Error.Code != UnhandledExceptionCode)
                        {
                            _Logger.LogInformation($"Transaction {items[i].Id} is expiring {JsonConvert.SerializeObject(response)}");
                            await _TransactionService.ExpireTransaction(items[i].Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task InformBot(Transaction transaction)
        {
            try
            {
                await AddToMobileTransferQueue(transaction);

            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                await AddToMobileTransferQueue(transaction);
            }
        }

        private async Task AddToMobileTransferQueue(Transaction transaction)
        {
            await _TransactionQueueService.InsertMobileTransferQueueItem(new MobileTransferQueueItem()
            {
                LastTryDateTime = null,
                TenantGuid = transaction.TenantGuid,
                TransactionCode = transaction.Token,
                TryCount = 0
            });
        }
    }
}

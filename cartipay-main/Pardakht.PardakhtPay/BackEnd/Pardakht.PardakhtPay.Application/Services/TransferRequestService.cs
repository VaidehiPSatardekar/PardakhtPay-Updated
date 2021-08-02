using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Extensions;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class TransferRequestService : ITransferRequestService
    {
        IAesEncryptionService _EncryptionService = null;
        static string _Token = string.Empty;
        IHttpClientFactory _HttpClientFactory = null;
        UserManagementSettings _UserManagementSettings = null;
        IWithdrawalRepository _WithdrawalRepository;
        IAutoTransferRepository _AutoTransferRepository;
        IManualTransferDetailRepository _ManualTransferDetailRepository;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferHistoryRepository;
        ITransactionQueueService _TransactionQueueService = null;
        ILogger<TransferRequestService> _Logger;
        IWithdrawalManager _WithdrawalManager;

        public TransferRequestService(
            ILogger<TransferRequestService> logger,
            IAesEncryptionService aesEncryptionService,
            IHttpClientFactory httpClientFactory,
            IOptions<UserManagementSettings> userManagementOptions,
            IWithdrawalRepository withdrawalRepository,
            IAutoTransferRepository autoTransferRepository,
            IManualTransferDetailRepository manualTransferDetailRepository,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository,
            ITransactionQueueService transactionQueueService,
            IWithdrawalManager withdrawalManager)
        {
            _Logger = logger;
            _EncryptionService = aesEncryptionService;
            _HttpClientFactory = httpClientFactory;
            _UserManagementSettings = userManagementOptions.Value;
            _WithdrawalRepository = withdrawalRepository;
            _AutoTransferRepository = autoTransferRepository;
            _ManualTransferDetailRepository = manualTransferDetailRepository;
            _WithdrawalTransferHistoryRepository = withdrawalTransferHistoryRepository;
            _TransactionQueueService = transactionQueueService;
            _WithdrawalManager = withdrawalManager;
        }
                

        public async Task<WebResponse> UpdateTransactionStatus(TransferRequestResponse transferRequestResponse)
        {
            try
            {

                var withdrwalData = await _WithdrawalRepository.GetItemAsync(t => t.TransferNotes == Convert.ToString(transferRequestResponse.Id));

                if (withdrwalData == null)
                {
                    var autoTransferData = await _AutoTransferRepository.GetItemAsync(t => t.RequestId == transferRequestResponse.Id);
                    if (autoTransferData == null)
                    {
                        var manualTransferData = await _ManualTransferDetailRepository.GetItemAsync(t => t.TransferId == transferRequestResponse.Id);
                        if (manualTransferData != null)
                        {
                            var manualTransferDetail = manualTransferData;
                            manualTransferDetail.TransferStatus = transferRequestResponse.TransferStatus;
                            await _ManualTransferDetailRepository.UpdateAsync(manualTransferDetail);
                            await _ManualTransferDetailRepository.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var autoTransfer = autoTransferData;
                        autoTransfer.Status = transferRequestResponse.TransferStatus;
                        await _AutoTransferRepository.UpdateAsync(autoTransfer);
                        await _AutoTransferRepository.SaveChangesAsync();
                    }
                }
                else
                {
                    var oldStatus = withdrwalData.TransferStatus;

                    var item = await _WithdrawalManager.CheckWithdrawalStatus(withdrwalData.Id);

                    if (oldStatus != item.TransferStatus && item.NeedCallback())
                    {
                        await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                        {
                            Id = item.Id,
                            LastTryDateTime = null,
                            TenantGuid = item.TenantGuid,
                            TryCount = 0
                        });
                    }
                }
                return new WebResponse();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        public static bool NeedCallback(Withdrawal withdrawal)
        {
            int[] CallbackStatuses = new int[] {
            (int)TransferStatus.Complete,
            (int)TransferStatus.AwaitingConfirmation,
            (int)TransferStatus.Cancelled,
            (int)TransferStatus.RefundFromBank,
            (int)TransferStatus.CompletedWithNoReceipt,
            (int)TransferStatus.AccountNumberInvalid,
            (int)TransferStatus.FailedFromBank,
            (int)TransferStatus.InvalidIBANNumber };

            return CallbackStatuses.Contains(withdrawal.TransferStatus);
        }

    }
}

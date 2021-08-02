using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Extensions;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BotAutoTransferService.CheckManagers
{
    public class CheckWithdrawalManager : ICheckWithdrawalManager
    {
        ILogger _Logger = null;
        IWithdrawalManager _WithdrawalManager = null;
        WithdrawalConfiguration _Configuration = null;
        IBankBotService _BankBotService = null;
        //IAuthenticationService _AuthenticationService = null;
        CurrentUser _CurrentUser = null;        
        ITransactionQueueService _TransactionQueueService = null;

        public CheckWithdrawalManager(ILogger<CheckWithdrawalManager> logger,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            //IAuthenticationService authenticationService,
            //ITenantCacheService tenantCacheService,
            IBankBotService bankBotService,
            CurrentUser currentUser,            
            ITransactionQueueService transactionQueueService,
            IWithdrawalManager withdrawalManager)
        {
            _Logger = logger;
            _WithdrawalManager = withdrawalManager;
            _Configuration = withdrawalOptions.Value;
            _BankBotService = bankBotService;
            //_AuthenticationService = authenticationService;
            _CurrentUser = currentUser;            
            _TransactionQueueService = transactionQueueService;
        }

        public async Task Run()
        {
            try
            {
                var date = DateTime.UtcNow.Subtract(_Configuration.ConfirmationDeadline);

                var items = await _WithdrawalManager.GetUncompletedWithdrawals(date);

                for (int i = 0; i < items.Count; i++)
                {
                    try
                    {
                        var oldStatus = items[i].TransferStatus;

                        var item = await _WithdrawalManager.CheckWithdrawalStatus(items[i]);

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
    }
}

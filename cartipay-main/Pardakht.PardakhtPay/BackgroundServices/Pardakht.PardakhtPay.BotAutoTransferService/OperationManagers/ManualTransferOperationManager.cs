using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class ManualTransferOperationManager : IManualTransferOperationManager
    {
        IManualTransferManager _Manager;
        IManualTransferDetailManager _DetailManager;
        ManualTransferConfiguration _Configuration;
        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        ILogger _Logger = null;

        public ManualTransferOperationManager(IManualTransferManager manager,
            IManualTransferDetailManager detailManager,
            IOptions<ManualTransferConfiguration> transferOptions,
            IBankBotService bankBotService,
            IAesEncryptionService encryptionService,
            ILogger<ManualTransferOperationManager> logger
            )
        {
            _Manager = manager;
            _DetailManager = detailManager;
            _Configuration = transferOptions.Value;
            _BankBotService = bankBotService;
            _EncryptionService = encryptionService;
            _Logger = logger;
        }

        public async Task Run()
        {

            try
            {
                await ProcessNewTransfers();

                await ProcessDetails();

                var date = DateTime.UtcNow.Subtract(_Configuration.ConfirmationDeadline);
                await _Manager.Check(date);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task ProcessNewTransfers()
        {
            try
            {
                var items = await _Manager.GetUnProcessedItemsAsync();
                var statuses = await _BankBotService.GetAccountsWithStatus();
                var logins = await _BankBotService.GetLogins();
                var banks = await _BankBotService.GetBanks();

                for (int i = 0; i < items.Count; i++)
                {
                    try
                    {
                        var transfer = items[i];

                        await _Manager.Process(transfer, statuses, banks, logins);
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError(ex, ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task ProcessDetails()
        {
            try
            {
                var items = await _DetailManager.GetUnProcessedItems();

                for (int i = 0; i < items.Count; i++)
                {
                    try
                    {
                        var detail = items[i];

                        await _DetailManager.Process(detail,null);
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

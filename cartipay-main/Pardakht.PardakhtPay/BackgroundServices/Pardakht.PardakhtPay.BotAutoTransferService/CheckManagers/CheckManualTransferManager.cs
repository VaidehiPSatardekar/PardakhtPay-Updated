using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BotAutoTransferService.CheckManagers
{
    public class CheckManualTransferManager : ICheckManualTransferManager
    {
        IManualTransferDetailManager _Manager;
        ManualTransferConfiguration _Configuration;
        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        ILogger _Logger = null;

        public CheckManualTransferManager(IManualTransferDetailManager manager,
            IOptions<ManualTransferConfiguration> transferOptions,
            IBankBotService bankBotService,
            IAesEncryptionService encryptionService,
            ILogger<ManualTransferOperationManager> logger
            )
        {
            _Manager = manager;
            _Configuration = transferOptions.Value;
            _BankBotService = bankBotService;
            _EncryptionService = encryptionService;
            _Logger = logger;
        }

        public async Task Run()
        {
            try
            {
                var date = DateTime.UtcNow.Subtract(_Configuration.ConfirmationDeadline);

                var items = await _Manager.GetUnCompletedItems(date);

                for (int i = 0; i < items.Count; i++)
                {
                    var detail = items[i];

                    await _Manager.Check(detail);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}

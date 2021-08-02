using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class CardHolderNameOperationManager : ICardHolderNameOperationManager
    {
        ICardHolderNameManager _Manager = null;
        CardHolderNameConfiguration _Configuration = null;
        ILogger _Logger;
        ITransactionRepository _TransactionRepository = null;
        IAesEncryptionService _EncryptionService = null;
        IWithdrawalRepository _WithdrawalRepository = null;

        public CardHolderNameOperationManager(ICardHolderNameManager manager,
            IOptions<CardHolderNameConfiguration> configurationOptions,
            ILogger<CardHolderNameOperationManager> logger,
            ITransactionRepository transactionRepository,
            IWithdrawalRepository withdrawalRepository,
            IAesEncryptionService encryptionService)
        {
            _Manager = manager;
            _Configuration = configurationOptions.Value;
            _Logger = logger;
            _TransactionRepository = transactionRepository;
            _EncryptionService = encryptionService;
            _WithdrawalRepository = withdrawalRepository;
        }

        public async Task Run()
        {
            try
            {
                var query = _Manager.GetQuery();

                var dateFrom = DateTime.UtcNow.AddDays(-30);

                var transactionQuery = _TransactionRepository.GetQuery(t => !string.IsNullOrEmpty(t.CustomerCardNumber));

                var items = (from t in transactionQuery
                             where t.CreationDate >= dateFrom && !query.Any(p => t.CustomerCardNumber == p.CardNumber)
                             && !string.IsNullOrEmpty(t.CustomerCardNumber)
                             select t.CustomerCardNumber).Distinct().Take(_Configuration.Count).ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var encrypted = items[i];

                    await _Manager.ProcessForCardNumber(encrypted);
                }

                var withdrawalQuery = _WithdrawalRepository.GetQuery(t => !string.IsNullOrEmpty(t.CardNumber));

                var withdrawalItems = (from t in withdrawalQuery
                                       where !query.Any(p => t.CardNumber == p.CardNumber)
                                           && !string.IsNullOrEmpty(t.CardNumber)
                                       select t.CardNumber).Distinct().Take(_Configuration.Count).ToList();

                for (int i = 0; i < withdrawalItems.Count; i++)
                {
                    var encrypted = withdrawalItems[i];

                    await _Manager.ProcessForCardNumber(encrypted);
                }

                var withdrawalIbanQuery = _WithdrawalRepository.GetQuery(t => !string.IsNullOrEmpty(t.ToIbanNumber));

                withdrawalItems = (from t in withdrawalIbanQuery
                                   where t.CreateDate >= dateFrom && !query.Any(p => t.ToIbanNumber == p.CardNumber)
                                   select t.ToIbanNumber).Distinct().Take(_Configuration.Count).ToList();

                for (int i = 0; i < withdrawalItems.Count; i++)
                {
                    var encrypted = withdrawalItems[i];

                    await _Manager.ProcessForIbanNumber(encrypted);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.BotAutoTransferService.CheckManagers
{
    public class CheckCardNumberManager : ICheckCardNumberManager
    {
        IBankBotService _BankBotService = null;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        IAesEncryptionService _EncryptionService = null;
        ILogger _Logger = null;

        public CheckCardNumberManager(IBankBotService bankBotService,
            ICardToCardAccountManager cardToCardAccountManager,
            IAesEncryptionService aesEncryptionService,
            ILogger<CheckCardNumberManager> logger)
        {
            _BankBotService = bankBotService;
            _CardToCardAccountManager = cardToCardAccountManager;
            _EncryptionService = aesEncryptionService;
            _Logger = logger;
        }

        public async Task Run()
        {
            try
            {
                var accounts = await _CardToCardAccountManager.GetAllAsync();

                var logins = await _BankBotService.GetLogins();

                accounts = accounts.Where(t => !logins.Any(p => p.LoginGuid == t.LoginGuid && p.IsDeleted)).ToList();

                for (int i = 0; i < accounts.Count; i++)
                {
                    try
                    {
                        var blockedCards = await _BankBotService.GetBlockedCardDetails(accounts[i].AccountGuid);

                        if (blockedCards.Count > 0)
                        {
                            var last = blockedCards.OrderByDescending(t => t.TimeStamp).First();

                            if (!string.IsNullOrEmpty(last.CardNumber))
                            {
                                var encrypted = _EncryptionService.EncryptToBase64(last.CardNumber);

                                if (accounts[i].CardNumber != encrypted)
                                {
                                    accounts[i].CardNumber = encrypted;

                                    await _CardToCardAccountManager.UpdateAsync(accounts[i]);
                                    await _CardToCardAccountManager.SaveAsync();

                                    _Logger.LogInformation($"Card number is replaced to : {last.CardNumber} Account No : {last.AccountNo}");
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
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}

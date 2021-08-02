using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class CardHolderNameManager : BaseManager<CardHolderName, ICardHolderNameRepository>, ICardHolderNameManager
    {

        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        IMobileTransferService _MobileTransferService = null;

        public CardHolderNameManager(ICardHolderNameRepository repository,
            IBankBotService bankBotService,
            IAesEncryptionService encryptionService,
            IMobileTransferService mobileTransferService) :base(repository)
        {
            _BankBotService = bankBotService;
            _EncryptionService = encryptionService;
            _MobileTransferService = mobileTransferService;
        }

        public async Task ProcessForCardNumber(string ecryptedCardnumber)
        {
            var clear = _EncryptionService.DecryptToString(ecryptedCardnumber);

            if (clear.CheckCardNumberIsValid())
            {
                //var response = await _BankBotService.CreateCardHolderRequest(new Shared.Models.WebService.BankBotCardHolderRequest()
                //{
                //    CardORIBANNumber = clear,
                //    RequestDate = DateTime.UtcNow,
                //    Type = (int)CardHolderNameType.CardNumber
                //});

                var response = await _MobileTransferService.GetCardOwnerNameAsync(new Shared.Models.MobileTransfer.MobileTransferStartTransferModel()
                {
                    ApiType = (int)ApiType.Sekeh,
                    ToCardNo = clear
                });

                var item = new CardHolderName();
                item.CardNumber = ecryptedCardnumber;
                item.RecordId = 0;
                item.CreateDate = DateTime.UtcNow;
                item.Name = response?.Result?.Msg;
                item.Type = (int)CardHolderNameType.CardNumber;
                item.Status = response.IsSuccess ? (int)CardHolderRequestStatus.Complete : (int)CardHolderRequestStatus.Failed;

                await Repository.InsertAsync(item);
                await Repository.SaveChangesAsync();
            }
            else
            {
                var item = new CardHolderName();
                item.CardNumber = ecryptedCardnumber;
                item.Status = (int)CardHolderRequestStatus.Incomplete;
                item.Type = (int)CardHolderNameType.CardNumber;

                await Repository.InsertAsync(item);
                await Repository.SaveChangesAsync();
            }
        }

        public async Task ProcessForIbanNumber(string ecryptedIbannumber)
        {
            var clear = _EncryptionService.DecryptToString(ecryptedIbannumber);

            if (clear.CheckIbanIsValid())
            {
                var response = await _BankBotService.CreateCardHolderRequest(new BotCardHolderRequest()
                {
                    CardORIBANNumber = clear,
                    RequestDate = DateTime.UtcNow,
                    Type = (int)CardHolderNameType.Iban
                });

                var item = new CardHolderName();
                item.CardNumber = ecryptedIbannumber;
                item.RecordId = response.Id;
                item.CreateDate = DateTime.UtcNow;
                item.Name = response.Name;
                item.Type = (int)CardHolderNameType.Iban;
                item.Status = response.Status;

                await Repository.InsertAsync(item);
                await Repository.SaveChangesAsync();
            }
            else
            {
                var item = new CardHolderName();
                item.CardNumber = ecryptedIbannumber;
                item.Status = (int)CardHolderRequestStatus.Incomplete;
                item.Type = (int)CardHolderNameType.Iban;

                await Repository.InsertAsync(item);
                await Repository.SaveChangesAsync();
            }
        }

        public async Task<CardHolderName> GetCardHolderName(string clearCardNo, string callbackUrl, string accountContext)
        {
            var encrypted = _EncryptionService.EncryptToBase64(clearCardNo);
            var item = await GetItemAsync(t => t.CardNumber == encrypted && t.Type == (int)CardHolderNameType.CardNumber);

            if(item != null)
            {
                if ((!string.IsNullOrEmpty(callbackUrl) && item.CallbackUrl != callbackUrl) 
                    || (!string.IsNullOrEmpty(accountContext) && item.AccountContext != accountContext))                
                {
                    item.CallbackUrl = callbackUrl;
                    item.AccountContext = accountContext;
                    await UpdateAsync(item);
                    await SaveAsync();
                   // return item;
                }

                if (item.Status == (int)CardHolderRequestStatus.Complete && !string.IsNullOrEmpty(item.Name)) {
                    return item;
                }                
            }

            //var request = await _BankBotService.CreateCardHolderRequest(new Shared.Models.WebService.BankBotCardHolderRequest()
            //{
            //    CardORIBANNumber = clearCardNo,
            //    Id  = 0,
            //    Name = string.Empty,
            //    RequestDate = DateTime.UtcNow,
            //    Type = (int)CardHolderNameType.CardNumber
            //});

            var response = await _MobileTransferService.GetCardOwnerNameAsync(new Shared.Models.MobileTransfer.MobileTransferStartTransferModel()
            {
                ApiType = (int)ApiType.Sekeh,
                ToCardNo = clearCardNo
            });

            if (!response.IsSuccess && string.IsNullOrEmpty(response?.Result?.Msg))
            {
                return new CardHolderName()
                {
                    CardNumber = encrypted,
                    Name = string.Empty,
                    Status= (int)CardHolderRequestStatus.Failed
            };
            }

            if (item == null)
            {
                item = new CardHolderName();
                item.CardNumber = encrypted;
                item.CreateDate = DateTime.UtcNow;
                item.Name = response?.Result?.Msg;
                item.RecordId = 0;
                item.Type = (int)CardHolderNameType.CardNumber;
                item.Status = (int)CardHolderRequestStatus.Complete;
                item.CallbackUrl = callbackUrl;
                item.AccountContext = accountContext;

                await AddAsync(item);
            }
            else
            {
                item.RecordId = 0;
                item.Status = (int)CardHolderRequestStatus.Complete;
                item.Name = response?.Result?.Msg;

                await UpdateAsync(item);
            }
            await SaveAsync();

            return item;
        }

        public async Task<CardHolderName> GetIbanName(string clearIbanNo, string callbackUrl, string accountContext)
        {
            var encrypted = _EncryptionService.EncryptToBase64(clearIbanNo);

            var item = await GetItemAsync(t => t.CardNumber == encrypted && t.Type == (int)CardHolderNameType.Iban);

            if (item != null)
            {
                if ((!string.IsNullOrEmpty(callbackUrl) && item.CallbackUrl != callbackUrl)
                    || (!string.IsNullOrEmpty(accountContext) && item.AccountContext != accountContext))
                {
                    item.CallbackUrl = callbackUrl;
                    item.AccountContext = accountContext;
                    await UpdateAsync(item);
                    await SaveAsync();
                }
                if (item.Status == (int)CardHolderRequestStatus.Complete && !string.IsNullOrEmpty(item.Name))
                {
                    return item;
                }
            }

            var request = await _BankBotService.CreateCardHolderRequest(new BotCardHolderRequest()
            {
                CardORIBANNumber = clearIbanNo,
                Id = 0,
                Name = string.Empty,
                RequestDate = DateTime.UtcNow,
                Type = (int)CardHolderNameType.Iban
            });

            if (request.Id == -1 && string.IsNullOrEmpty(request.Name))
            {
                return new CardHolderName()
                {
                    CardNumber = encrypted,
                    Name = string.Empty
                };
            }

            item = new CardHolderName();
            item.CardNumber = encrypted;
            item.CreateDate = DateTime.UtcNow;
            item.Name = request.Name;
            item.RecordId = request.Id;
            item.Type = request.Type;
            item.Status = request.Status;
            item.CallbackUrl = callbackUrl;
            item.AccountContext = accountContext;

            await AddAsync(item);
            await SaveAsync();

            return item;
        }
    }
}

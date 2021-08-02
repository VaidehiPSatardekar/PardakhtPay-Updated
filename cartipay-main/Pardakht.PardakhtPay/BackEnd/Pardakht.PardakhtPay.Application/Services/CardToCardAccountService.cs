using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Extensions;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class CardToCardAccountService : DatabaseServiceBase<CardToCardAccount, ICardToCardAccountManager>, ICardToCardAccountService
    {
        IAesEncryptionService _EncryptionService = null;
        IBankBotService _BankBotService = null;
        IOwnerBankLoginManager _OwnerBankManager = null;
        ICardToCardAccountGroupItemManager _CardToCardAccountGroupItemManager = null;

        public CardToCardAccountService(ICardToCardAccountManager manager,
            ILogger<CardToCardAccountService> logger,
            IAesEncryptionService aesEncryptionService,
            IOwnerBankLoginManager ownerBankLoginManager,
            IBankBotService bankBotService,
            ICardToCardAccountGroupItemManager cardToCardAccountGroupItemManager):base(manager, logger)
        {
            _EncryptionService = aesEncryptionService;
            _BankBotService = bankBotService;
            _OwnerBankManager = ownerBankLoginManager;
            _CardToCardAccountGroupItemManager = cardToCardAccountGroupItemManager;
        }

        public async Task<WebResponse<List<CardToCardAccountDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<CardToCardAccountDTO>>(result.Payload);

                var groupItems = await _CardToCardAccountGroupItemManager.GetAllAsync();

                var logins = await _OwnerBankManager.GetAllAsync();

                dtos.ForEach(t =>
                {
                    if (!string.IsNullOrEmpty(t.CardNumber))
                    {
                        t.CardNumber = _EncryptionService.DecryptToString(t.CardNumber);
                    }

                    if (!string.IsNullOrEmpty(t.SafeAccountNumber))
                    {
                        t.SafeAccountNumber = _EncryptionService.DecryptToString(t.SafeAccountNumber);
                    }

                    if (!string.IsNullOrEmpty(t.CardHolderName))
                    {
                        t.CardHolderName = _EncryptionService.DecryptToString(t.CardHolderName);
                    }

                    var items = groupItems.Where(p => p.CardToCardAccountId == t.Id);

                    if(items.Any(p => p.AllowWithdrawal) && items.Any(p => p.AllowCardToCard))
                    {
                        t.AccountType = (int)CardToCardAccountType.Both;
                    }
                    else if(items.Any(p => p.AllowWithdrawal))
                    {
                        t.AccountType = (int)CardToCardAccountType.Deposit;
                    }
                    else if (items.Any(p => p.AllowCardToCard))
                    {
                        t.AccountType = (int)CardToCardAccountType.Withdrawal;
                    }
                    else
                    {
                        t.AccountType = (int)CardToCardAccountType.None;
                    }
                });
                var accounts = await _BankBotService.GetAccountsAsync();

                dtos = (from d in dtos
                        join a in accounts on d.AccountGuid equals a.AccountGuid into xxAccounts
                        from a in xxAccounts.DefaultIfEmpty()
                        join o in logins on d.LoginGuid equals o.BankLoginGuid
                        where o.IsDeleted == false
                        select new CardToCardAccountDTO()
                        {
                            AccountGuid = d.AccountGuid,
                            AccountNo = a?.AccountNo,
                            CardHolderName = d.CardHolderName,
                            CardNumber = d.CardNumber,
                            Id = d.Id,
                            IsActive = d.IsActive,
                            IsTransferThresholdActive = d.IsTransferThresholdActive,
                            LoginGuid = d.LoginGuid,
                            SafeAccountNumber = d.SafeAccountNumber,
                            TransferThreshold = d.TransferThreshold,
                            TransferThresholdLimit = d.TransferThresholdLimit,
                            TenantGuid = d.TenantGuid,
                            OwnerGuid = d.OwnerGuid,
                            FriendlyName = o.FriendlyName,
                            LoginType = d.LoginType,
                            BankAccountId = a?.Id,
                            BankLoginId = a?.LoginId,
                            AccountType = d.AccountType
                        }).ToList();

                return new WebResponse<List<CardToCardAccountDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<CardToCardAccountDTO>>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<CardToCardAccountDTO>(result.Payload);

                if (!string.IsNullOrEmpty(dto.CardNumber))
                {
                    dto.CardNumber = _EncryptionService.DecryptToString(dto.CardNumber);
                }

                if (!string.IsNullOrEmpty(dto.SafeAccountNumber))
                {
                    dto.SafeAccountNumber = _EncryptionService.DecryptToString(dto.SafeAccountNumber);
                }

                if (!string.IsNullOrEmpty(dto.CardHolderName))
                {
                    dto.CardHolderName = _EncryptionService.DecryptToString(dto.CardHolderName);
                }

                return new WebResponse<CardToCardAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountDTO>> InsertAsync(CardToCardAccountDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<CardToCardAccount>(item);

                if (!string.IsNullOrEmpty(entity.CardNumber))
                {
                    entity.CardNumber = _EncryptionService.EncryptToBase64(entity.CardNumber.ConvertFarsiDigitCharsToEnglish());
                }

                if (!string.IsNullOrEmpty(entity.SafeAccountNumber))
                {
                    entity.SafeAccountNumber = _EncryptionService.EncryptToBase64(entity.SafeAccountNumber.ConvertFarsiDigitCharsToEnglish());
                }

                if (!string.IsNullOrEmpty(entity.CardHolderName))
                {
                    entity.CardHolderName = _EncryptionService.EncryptToBase64(entity.CardHolderName);
                }

                var login = _OwnerBankManager.GetLoginWithGuid(item.LoginGuid);

                entity.LoginType = login.LoginType;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<CardToCardAccountDTO>(result);

                if (!string.IsNullOrEmpty(dto.CardNumber))
                {
                    dto.CardNumber = _EncryptionService.DecryptToString(dto.CardNumber);
                }

                if (!string.IsNullOrEmpty(dto.SafeAccountNumber))
                {
                    dto.SafeAccountNumber = _EncryptionService.DecryptToString(dto.SafeAccountNumber);
                }

                if (!string.IsNullOrEmpty(dto.CardHolderName))
                {
                    dto.CardHolderName = _EncryptionService.DecryptToString(dto.CardHolderName);
                }

                return new WebResponse<CardToCardAccountDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountDTO>> UpdateAsync(CardToCardAccountDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<CardToCardAccount>(item);

                if (!string.IsNullOrEmpty(item.CardNumber))
                {
                    entity.CardNumber = _EncryptionService.EncryptToBase64(entity.CardNumber.ConvertFarsiDigitCharsToEnglish());
                }

                if (!string.IsNullOrEmpty(entity.SafeAccountNumber))
                {
                    entity.SafeAccountNumber = _EncryptionService.EncryptToBase64(entity.SafeAccountNumber.ConvertFarsiDigitCharsToEnglish());
                }

                if (!string.IsNullOrEmpty(entity.CardHolderName))
                {
                    entity.CardHolderName = _EncryptionService.EncryptToBase64(entity.CardHolderName);
                }

                var login = _OwnerBankManager.GetLoginWithGuid(item.LoginGuid);

                entity.LoginType = login.LoginType;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<CardToCardAccountDTO>(result);


                dto.CardNumber = _EncryptionService.DecryptToString(dto.CardNumber);
                if (!string.IsNullOrEmpty(dto.SafeAccountNumber))
                {
                    dto.SafeAccountNumber = _EncryptionService.DecryptToString(dto.SafeAccountNumber);
                }
                dto.CardHolderName = _EncryptionService.DecryptToString(dto.CardHolderName);

                return new WebResponse<CardToCardAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountDTO>(ex);
            }
        }
    }
}



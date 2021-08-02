using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.WebService.BankBot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class ManualTransferService : DatabaseServiceBase<ManualTransfer, IManualTransferManager>, IManualTransferService
    {
        ITransferAccountManager _TransferAccountManager = null;
        IAesEncryptionService _EncryptionService = null;
        IBankBotService _BankBotService = null;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        CurrentUser _CurrentUser = null;
        IManualTransferDetailManager _DetailManager = null;

        public ManualTransferService(
            IManualTransferManager manager,
            ITransferAccountManager transferAccountManager,
            IAesEncryptionService encryptionService,
            IBankBotService bankBotService,
            ICardToCardAccountManager cardToCardAccountManager,
            CurrentUser currentUser,
            IManualTransferDetailManager manualTransferDetailManager,
            ILogger<ManualTransferService> logger) : base(manager, logger)
        {
            _TransferAccountManager = transferAccountManager;
            _EncryptionService = encryptionService;
            _CardToCardAccountManager = cardToCardAccountManager;
            _BankBotService = bankBotService;
            _CurrentUser = currentUser;
            _DetailManager = manualTransferDetailManager;
        }

        public async Task<WebResponse<ListSearchResponse<List<ManualTransferDTO>>>> Search(ManualTransferSearchArgs args)
        {
            try
            {
                var result = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<List<ManualTransferDTO>>>(result);
            }
            catch (Exception ex)
            {
                return new WebResponse<ListSearchResponse<List<ManualTransferDTO>>>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<ManualTransferDTO>(result.Payload);

                dto.ToAccountNo = _EncryptionService.DecryptToString(dto.ToAccountNo);

                if (!string.IsNullOrEmpty(dto.FirstName))
                {
                    dto.FirstName = _EncryptionService.DecryptToString(dto.FirstName);
                }

                if (!string.IsNullOrEmpty(dto.LastName))
                {
                    dto.LastName = _EncryptionService.DecryptToString(dto.LastName);
                }

                if (!string.IsNullOrEmpty(dto.Iban))
                {
                    dto.Iban = _EncryptionService.DecryptToString(dto.Iban);
                }

                var details = await Manager.GetDetails(id);

                dto.Details = details.ConvertAll(t => AutoMapper.Mapper.Map<ManualTransferDetailDTO>(t));

                var accountGuids = dto.AccountGuid;

                var cardToCardAccountList = await _CardToCardAccountManager.GetItemsAsync(t => accountGuids.Contains(t.AccountGuid));

                if (cardToCardAccountList != null)
                {
                    foreach (var record in cardToCardAccountList) {
                        dto.CardToCardAccountIds.Add(record.Id);
                    }
                }

                return new WebResponse<ManualTransferDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDTO>> InsertAsync(ManualTransferDTO item)
        {
            try
            {

                var transferAccount = await _TransferAccountManager.GetEntityByIdAsync(item.TransferAccountId);

                if (transferAccount == null)
                {
                    throw new Exception("Transfer account could not be found");
                }

                //var cardToCardAccount = await _CardToCardAccountManager.GetEntityByIdAsync(item.CardToCardAccountId);
                string CardToCardAccountIds = String.Join(",", item.CardToCardAccountIds);
                var cardToCardAccountList = await _CardToCardAccountManager.GetItemsAsync(t => CardToCardAccountIds.Contains(t.Id.ToString()));

                if (cardToCardAccountList == null)
                {
                    throw new Exception("Bank account could not be found");
                }
                int i = 0;
                while (i < cardToCardAccountList.Count)
                {
                    var account = await _BankBotService.GetAccountStatus(cardToCardAccountList[i].AccountGuid);
                    if (account == null)
                    {
                        //throw new Exception("Account could not be found");
                        continue;
                    }

                    if (!account.IsOpen)
                    {
                        //throw new Exception("Account status is not valid");
                        continue;
                    }
                    item.AccountGuid += cardToCardAccountList[i].AccountGuid + ",";
                    i++;
                }

                item.AccountGuid = item.AccountGuid.Last() == ',' ? item.AccountGuid.Substring(0, item.AccountGuid.Length - 1) : item.AccountGuid;

                if (string.IsNullOrEmpty(item.AccountGuid))
                {
                    throw new Exception("Account could not be found / status invalid.");
                }

                if (item.ImmediateTransfer)
                {
                    item.ExpectedTransferDate = null;
                }
                else if (item.ExpectedTransferDate == null)
                {
                    throw new Exception("Please sent a valid date to schedule this transfer");
                }

                if (item.TransferWholeAmount)
                {
                    item.Amount = 0;
                }
                else if (item.Amount <= 0)
                {
                    throw new Exception("Amount must be bigger than 0");
                }

                await CheckTransferType(item, transferAccount);



                // var entity = AutoMapper.Mapper.Map<ManualTransfer>(item);
                ManualTransfer mappingEntity = new ManualTransfer();
                mappingEntity.CreationDate = item.CreationDate;
                mappingEntity.TenantGuid = item.TenantGuid;
                mappingEntity.OwnerGuid = item.OwnerGuid;
                mappingEntity.CardToCardAccountIds = new List<int> { 0 };
                mappingEntity.CardToCardAccountId = 0;
                mappingEntity.AccountGuid = item.AccountGuid;
                mappingEntity.TransferType = item.TransferType;
                mappingEntity.Amount = item.Amount;
                mappingEntity.TransferAccountId = item.TransferAccountId;
                mappingEntity.ToAccountNo = item.ToAccountNo;
                mappingEntity.Iban = item.Iban;
                mappingEntity.FirstName = item.FirstName;
                mappingEntity.LastName = item.LastName;
                mappingEntity.Status = item.Status;
                mappingEntity.Priority = item.Priority;
                mappingEntity.ProcessedDate = item.ProcessedDate;
                mappingEntity.CancelledDate = item.CancelledDate;
                mappingEntity.ImmediateTransfer = item.ImmediateTransfer;
                mappingEntity.ExpectedTransferDate = item.ExpectedTransferDate;
                mappingEntity.Comment = item.Comment;
                mappingEntity.CreatorId = item.CreatorId;
                mappingEntity.UpdaterId = item.UpdaterId;
                mappingEntity.CancellerId = item.CancellerId;
                mappingEntity.TransferWholeAmount = item.TransferWholeAmount;
                mappingEntity.ToAccountNo = transferAccount.AccountNo;
                mappingEntity.FirstName = transferAccount.AccountHolderFirstName;
                mappingEntity.LastName = transferAccount.AccountHolderLastName;
                mappingEntity.Iban = transferAccount.Iban;
                mappingEntity.CreationDate = DateTime.UtcNow;
                mappingEntity.CreatorId = _CurrentUser.IdentifierGuid;
                mappingEntity.Status = (int)ManualTransferStatus.Pending;

                var result = await Manager.AddAsync(mappingEntity);
                await Manager.SaveAsync();

                int j = 0;
                List<ManualTransferSourceCardDetails> sourceCardDetailList = new List<ManualTransferSourceCardDetails>();
                while (j < cardToCardAccountList.Count)
                {
                    ManualTransferSourceCardDetails sourceCardDetails = new ManualTransferSourceCardDetails();
                    sourceCardDetails.ManualTransferId = result.Id;
                    sourceCardDetails.CardToCardAccountId = cardToCardAccountList[j].Id;
                    sourceCardDetails.AccountGuid = cardToCardAccountList[j].AccountGuid;
                    sourceCardDetails.CreateDate = DateTime.UtcNow;
                    sourceCardDetailList.Add(sourceCardDetails);
                    j++;
                }
                await Manager.InsertSourceCardDetails(sourceCardDetailList);


                if (result.ImmediateTransfer)
                {
                    var accounts = await _BankBotService.GetAccountsWithStatus();
                    var logins = await _BankBotService.GetLogins();
                    var banks = await _BankBotService.GetBanks();

                    await Manager.Process(result, accounts, banks, logins, true);
                }

                var dto = AutoMapper.Mapper.Map<ManualTransferDTO>(result);

                dto.ToAccountNo = _EncryptionService.DecryptToString(dto.ToAccountNo);
                dto.FirstName = _EncryptionService.DecryptToString(dto.FirstName);
                dto.LastName = _EncryptionService.DecryptToString(dto.LastName);
                dto.Iban = _EncryptionService.DecryptToString(dto.Iban);

                return new WebResponse<ManualTransferDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDetailDTO>> CancelTransferDetail(int detailId)
        {
            try
            {
                var detail = await _DetailManager.Cancel(detailId);

                var dto = AutoMapper.Mapper.Map<ManualTransferDetailDTO>(detail);

                return new WebResponse<ManualTransferDetailDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDetailDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDetailDTO>> RetryTransferDetail(int detailId)
        {
            try
            {
                var detail = await _DetailManager.Retry(detailId);

                var dto = AutoMapper.Mapper.Map<ManualTransferDetailDTO>(detail);

                return new WebResponse<ManualTransferDetailDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDetailDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDetailDTO>> SetAsCompletedTransferDetail(int detailId)
        {
            try
            {
                var detail = await _DetailManager.SetAsCompleted(detailId);

                var dto = AutoMapper.Mapper.Map<ManualTransferDetailDTO>(detail);

                return new WebResponse<ManualTransferDetailDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDetailDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDTO>> UpdateAsync(ManualTransferDTO item)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(item.Id);

                if (oldEntity == null)
                {
                    throw new Exception($"Entity could not been found with id : {item.Id}");
                }

                if (oldEntity.Status != (int)ManualTransferStatus.Pending)
                {
                    throw new Exception("Status is not valid for updating");
                }

                var transferAccount = await _TransferAccountManager.GetEntityByIdAsync(item.TransferAccountId);

                if (transferAccount == null)
                {
                    throw new Exception("Transfer account could not be found");
                }

                ////var cardToCardAccount = await _CardToCardAccountManager.GetEntityByIdAsync(item.CardToCardAccountId);
                ////if (cardToCardAccount == null)
                ////{
                ////    throw new Exception("Bank account could not be found");
                ////}
                ////item.AccountGuid = cardToCardAccount.AccountGuid;

                ////var account = await _BankBotService.GetAccountStatus(item.AccountGuid);

                ////if (account == null)
                ////{
                ////    throw new Exception("Account could not be found");
                ////}

                ////if (!account.IsOpen)
                ////{
                ////    throw new Exception("Account status is not valid");
                ////}

                ////if (item.ImmediateTransfer)
                ////{
                ////    item.ExpectedTransferDate = null;
                ////}
                ////else if (item.ExpectedTransferDate == null)
                ////{
                ////    throw new Exception("Please sent a valid date to schedule this transfer");
                ////}

                ////if (item.TransferWholeAmount)
                ////{
                ////    item.Amount = 0;
                ////}
                ////else if (item.Amount <= 0)
                ////{
                ////    throw new Exception("Amount must be bigger than 0");
                ////}

                ////await CheckTransferType(item, transferAccount);

                string CardToCardAccountIds = String.Join(",", item.CardToCardAccountIds);
                var cardToCardAccountList = await _CardToCardAccountManager.GetItemsAsync(t => CardToCardAccountIds.Contains(t.Id.ToString()));

                if (cardToCardAccountList == null)
                {
                    throw new Exception("Bank account could not be found");
                }

                int i = 0;
                while (i < cardToCardAccountList.Count)
                {
                    var accountData = await _BankBotService.GetAccountStatus(cardToCardAccountList[i].AccountGuid);
                    if (accountData == null)
                    {
                        //throw new Exception("Account could not be found");
                        continue;
                    }

                    if (!accountData.IsOpen)
                    {
                        //throw new Exception("Account status is not valid");
                        continue;
                    }
                    item.AccountGuid += cardToCardAccountList[i].AccountGuid + ",";
                    i++;
                }

                item.AccountGuid = item.AccountGuid.Last() == ',' ? item.AccountGuid.Substring(0, item.AccountGuid.Length - 1) : item.AccountGuid;

                if (string.IsNullOrEmpty(item.AccountGuid))
                {
                    throw new Exception("Account could not be found / status invalid.");
                }

                if (item.ImmediateTransfer)
                {
                    item.ExpectedTransferDate = null;
                }
                else if (item.ExpectedTransferDate == null)
                {
                    throw new Exception("Please sent a valid date to schedule this transfer");
                }

                if (item.TransferWholeAmount)
                {
                    item.Amount = 0;
                }
                else if (item.Amount <= 0)
                {
                    throw new Exception("Amount must be bigger than 0");
                }

                await CheckTransferType(item, transferAccount);

                var entity = AutoMapper.Mapper.Map<ManualTransfer>(item);
                entity.ToAccountNo = transferAccount.AccountNo;
                entity.FirstName = transferAccount.AccountHolderFirstName;
                entity.LastName = transferAccount.AccountHolderLastName;
                entity.Iban = transferAccount.Iban;
                entity.Status = oldEntity.Status;
                entity.CreationDate = oldEntity.CreationDate;
                entity.CreatorId = oldEntity.CreatorId;
                entity.CancelledDate = oldEntity.CancelledDate;
                entity.CancellerId = oldEntity.CancellerId;
                entity.ProcessedDate = oldEntity.ProcessedDate;
                entity.UpdateDate = DateTime.UtcNow;
                entity.UpdaterId = _CurrentUser.IdentifierGuid;

                var result = await Manager.UpdateAsync(entity);
                await Manager.SaveAsync();

                await Manager.DeleteSourceCardDetails(entity.Id);
                int j = 0;
                List<ManualTransferSourceCardDetails> sourceCardDetailList = new List<ManualTransferSourceCardDetails>();
                while (j < cardToCardAccountList.Count)
                {
                    ManualTransferSourceCardDetails sourceCardDetails = new ManualTransferSourceCardDetails();
                    sourceCardDetails.ManualTransferId = result.Id;
                    sourceCardDetails.CardToCardAccountId = cardToCardAccountList[j].Id;
                    sourceCardDetails.AccountGuid = cardToCardAccountList[j].AccountGuid;
                    sourceCardDetails.CreateDate = DateTime.UtcNow;
                    sourceCardDetailList.Add(sourceCardDetails);
                    j++;
                }
                await Manager.InsertSourceCardDetails(sourceCardDetailList);


                if (result.ImmediateTransfer && result.Status == (int)ManualTransferStatus.Pending)
                {
                    var accounts = await _BankBotService.GetAccountsWithStatus();
                    var logins = await _BankBotService.GetLogins();
                    var banks = await _BankBotService.GetBanks();

                    await Manager.Process(result, accounts, banks, logins, true);
                }

                var dto = AutoMapper.Mapper.Map<ManualTransferDTO>(result);

                dto.ToAccountNo = _EncryptionService.DecryptToString(dto.ToAccountNo);
                dto.FirstName = _EncryptionService.DecryptToString(dto.FirstName);
                dto.LastName = _EncryptionService.DecryptToString(dto.LastName);
                dto.Iban = _EncryptionService.DecryptToString(dto.Iban);

                return new WebResponse<ManualTransferDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDTO>(ex);
            }
        }

        public async Task<WebResponse<ManualTransferDTO>> CancelAsync(int id)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(id);

                if (oldEntity == null)
                {
                    throw new Exception($"Entity could not been found with id : {id}");
                }

                if (oldEntity.Status != (int)ManualTransferStatus.Pending)
                {
                    throw new Exception("Status is not valid for cancelation");
                }

                oldEntity.CancelledDate = DateTime.UtcNow;
                oldEntity.CancellerId = _CurrentUser.IdentifierGuid;
                oldEntity.Status = (int)ManualTransferStatus.Cancelled;
                oldEntity.UpdateDate = DateTime.UtcNow;

                var result = await Manager.UpdateAsync(oldEntity);
                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<ManualTransferDTO>(result);

                dto.ToAccountNo = _EncryptionService.DecryptToString(dto.ToAccountNo);
                dto.FirstName = _EncryptionService.DecryptToString(dto.FirstName);
                dto.LastName = _EncryptionService.DecryptToString(dto.LastName);
                dto.Iban = _EncryptionService.DecryptToString(dto.Iban);

                return new WebResponse<ManualTransferDTO>(dto);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ManualTransferDTO>(ex);
            }
        }

        public async Task<WebResponse<List<ManualTransferDetailDTO>>> GetDetails(int id)
        {
            try
            {
                var items = await Manager.GetDetails(id);

                var dtos = items.ConvertAll(t => AutoMapper.Mapper.Map<ManualTransferDetailDTO>(t));

                return new WebResponse<List<ManualTransferDetailDTO>>(dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<ManualTransferDetailDTO>>(ex);
            }
        }

        public async Task<WebResponse<BankBotTransferReceiptResponse>> GetTransferReceipt(int id)
        {
            try
            {
                var receipt = await Manager.GetTransferReceipt(id);

                return new WebResponse<BankBotTransferReceiptResponse>(receipt);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<BankBotTransferReceiptResponse>(ex);
            }
        }

        private async Task CheckTransferType(ManualTransferDTO item, TransferAccount transferAccount)
        {
            var logins = await _BankBotService.GetLogins();

            var banks = await _BankBotService.GetBanks();

            var bankBotAccounts = await _BankBotService.GetMultipleAccountsWithStatus(item.AccountGuid, (TransferType)item.TransferType);

            //var bankBotAccount = bankBotAccounts.FirstOrDefault();

            foreach (var bankBotAccount in bankBotAccounts)
            {

                var login = logins.FirstOrDefault(t => t.LoginGuid == bankBotAccount.LoginGuid);

                if (login == null)
                {
                    // throw new Exception("Login could not be found");
                    item.AccountGuid.Replace(bankBotAccount.AccountGuid + ",", "");
                }

                if (login.IsDeleted)
                {
                    //throw new Exception("Login is deleted");
                    item.AccountGuid.Replace(bankBotAccount.AccountGuid + ",", "");
                }

                ////if (item.ImmediateTransfer && !item.TransferWholeAmount)
                ////{
                ////    if (item.Amount > bankBotAccount.WithdrawRemainedAmountForDay)
                ////    {
                ////        throw new Exception("The amount is bigger than daily limit");
                ////    }

                ////    if (item.Amount > bankBotAccount.WithdrawRemainedAmountForMonth)
                ////    {
                ////        throw new Exception("The amount is bigger than monthly limit");
                ////    }

                ////    if (item.Amount > bankBotAccount.WithdrawableBalance)
                ////    {
                ////         throw new Exception("The amount is bigger than balance");
                ////    }
                ////}

                var bank = banks.FirstOrDefault(t => t.Id == login.BankId);

                var iban = _EncryptionService.DecryptToString(transferAccount.Iban);

                var bankCode = iban.GetBankCodeFromIban();

                if (item.TransferType == (int)TransferType.Normal && bankCode != bank.BankCode)
                {
                    throw new Exception("Banks must be same for a normal transfer");
                }

                if (item.TransferType != (int)TransferType.Normal && bankCode == bank.BankCode)
                {
                    throw new Exception("Banks must be different for paya or satna transfers");
                }
            }
        }
    }
}




using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Mock
{
    public class BankBotMockService : IBankBotService
    {
        BankBotConfiguration _Configuration = null;
        ITransactionManager _TransactionManager = null;
        IAesEncryptionService _EncryptionService = null;
        ITransactionQueryHistoryManager _TransactionQueryHistoryManager = null;
        IWithdrawalQueryHistoryManager _WithdrawalQueryHistoryManager = null;

        public BankBotMockService(IOptions<BankBotConfiguration> options,
            ITransactionManager transactionManager,
            IAesEncryptionService encryptionService,
            ITransactionQueryHistoryManager transactionQueryHistoryManager,
            IWithdrawalQueryHistoryManager withdrawalQueryHistoryManager)
        {
            _Configuration = options.Value;
            _TransactionManager = transactionManager;
            _EncryptionService = encryptionService;
            _TransactionQueryHistoryManager = transactionQueryHistoryManager;
            _WithdrawalQueryHistoryManager = withdrawalQueryHistoryManager;
        }

        public int AccountReadCount { get; }

        public int BankReadCount { get; }

        public int LoginReadCount { get; }

        public Task<BotLoginInformation> ActivateLoginByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task<BotTransferStatusResponse> CheckTransferStatus(int id)
        {
            throw new NotImplementedException();
        }

        public async Task ClearAccountCache()
        {
            await Task.Run(() =>
            {

            });
        }

        public async Task ClearLoginCache()
        {
            await Task.Run(() =>
            {

            });
        }

        public async Task<BotConfirmationResponse> Confirm(BotConfirmRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BotConfirmationResponse> Confirm(string token)
        {
            throw new NotImplementedException();
        }

        public Task<BotLoginInformation> CreateLoginFromLoginRequest(CreateLoginFromLoginRequestDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<BotLoginInformation> CreateLoginRequest(BotLoginCreateInformation login)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> CreateTransferRequest(BotTransferRequestDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<BotLoginInformation> DeactivateLoginByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLogin(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> DeleteTransferRequest(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<BotAccountInformation> GetAccountByGuid(string accountGuid)
        {
            var accounts = await GetAccountsAsync();

            return accounts.FirstOrDefault(p => p.AccountGuid == accountGuid);
        }

        public async Task<List<BotAccountInformation>> GetAccountsAsync()
        {
            return await Task.Run(() =>
            {
                var accounts = new List<BotAccountInformation>();

                return accounts;
            });
        }

        public async Task<List<BotAccountInformation>> GetAccountsByLoginGuidAsync(string loginGuid)
        {
            var accounts = await GetAccountsAsync();

            return accounts.Where(t => t.LoginGuid == loginGuid).ToList();
        }

        public async Task<List<BotAccountInformation>> GetAccountsByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                var accounts = new List<BotAccountInformation>();

                return accounts;
            });
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatus()
        {
            return new List<BankBotAccountWithStatusDTO>()
            {

            };
        }

        public async Task<List<BotBankInformation>> GetBanks()
        {
            return await Task.Run(() =>
            {
                return new List<BotBankInformation>()
                {
                };
            });
        }

        public async Task<List<BotLoginInformation>> GetLogins()
        {
            return await Task.Run(() =>
            {
                return new List<BotLoginInformation>()
                {
                };
            });
        }

        public async Task<List<BotLoginSelect>> GetLoginSelect()
        {
            var items = await GetLogins();
            var banks = await GetBanks();

            var list = (from li in items
                        join b in banks on li.BankId equals b.Id into xxBank
                        from b in xxBank.DefaultIfEmpty()
                        select new BotLoginSelect()
                        {
                            Id = li.Id,
                            BankName = b.BankName,
                            LoginGuid = li.LoginGuid,
                            FriendlyName = li.Username,
                            BankId = li.BankId,
                            IsBlocked = li.IsBlocked,
                            IsActive = li.IsActive,
                            IsDeleted = li.IsDeleted
                        }).ToList();

            return list;
        }

        public Task<BankBotAccountStatus> GetAccountStatus(string accountGuid)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> GetTransferRequestWithStatus(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<BotAccountInformation> InsertAccountAsync(BankBotAccountCreateDTO item)
        {
            throw new NotImplementedException();
        }

        public async Task<BotLoginInformation> InsertLoginAsync(BotLoginCreateInformation login)
        {
            throw new NotImplementedException();
        }

        public async Task<BotWithdrawalResponse> Transfer(BotWithdrawalRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BotLoginInformation> UpdateLoginAsync(BankBotLoginUpdateInformation login)
        {
            throw new NotImplementedException();
        }

        public Task<BotTransferHistoryResponseDTO> GetTransferHistory(int requestId)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferReceiptResponse> GetTransferReceipt(BankBotTransferReceiptRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> CancelTransferRequest(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> CancelTransferRequest(string requestGuid)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> RetryTransferRequest(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> RetryTransferRequest(string requestGuid)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> SetAsCompletedTransferRequest(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BankBotTransferRequest> SetAsCompletedTransferRequest(string requestGuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<BankBotAccountWithStatusDTO>> GetSingleAccountsWithStatus(string accountGuid, TransferType? transferType = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMobileTransferInformation(MobileTransactionDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeIsBlockedCardFlag(BotLoginBlockCardChangeModel model)
        {
            throw new NotImplementedException();
        }

        public Task<List<BotBlockedCardDetail>> GetBlockedCardDetails(string accountGuid)
        {
            throw new NotImplementedException();
        }

        public Task<BotCardHolderRequest> CreateCardHolderRequest(BotCardHolderRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BotTransferStatus>> GetTransferStatusList()
        {
            return new List<BotTransferStatus>();
        }

        public Task<List<BotBlockedCardDetail>> GetBlockedCardDetails()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSatnaApplicable(int bankId)
        {
            throw new NotImplementedException();
        }

        public Task<BotLoginInformation> GetLoginByIdFromServer(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BotLoginSelect> CreateQRRegisterRequest(int loginId)
        {
            throw new NotImplementedException();
        }

        public async Task<QRRegisterLoginDTO> GetQRRegistrationDetailsByLoginId(int loginId)
        {
            throw new NotImplementedException();
        }

        public async Task<QRRegisterLoginDTO> RegisterQRCode(QrCodeRegistrationRequest qrRegisterDetails)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetLoginDeviceStatusByLoginId(int loginId)
        {
            throw new NotImplementedException();
        }
        public async Task<BotLoginInformation> GetDeviceStatusOfLogins(List<int> loginIds)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateOTPForRegisteredDevice(int bankLoginId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatuses(string[] accountGuids)
        {
            return new List<BankBotAccountWithStatusDTO>();
        }

        public async Task<BotLoginInformation> RegisterLoginRequest(RegisterLogin login)
        {
            throw new NotImplementedException();
        }
        public async Task<BotLoginInformation> SwitchBankConnectionProgram(int id)
        {
            throw new NotImplementedException();
        }
        public Task<List<BankBotAccountWithStatusDTO>> GetMultipleAccountsWithStatus(string accountGuid, TransferType? transferType = null)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Extensions;
using System.IO;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using System.Net;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class BankBotService : IBankBotService
    {
        BankBotConfiguration _Configuration = null;
        ITransactionManager _TransactionManager = null;
        IAesEncryptionService _EncryptionService = null;
        ITransactionQueryHistoryManager _TransactionQueryHistoryManager = null;
        IWithdrawalQueryHistoryManager _WithdrawalQueryHistoryManager = null;
        ICacheService _CacheService = null;
        static object _LockObject = new object();
        TimeSpan _CacheTimeout = TimeSpan.FromMinutes(30);
        static SemaphoreSlim _AccountsSemaphore = new SemaphoreSlim(1, 1);
        static SemaphoreSlim _BankSemaphore = new SemaphoreSlim(1, 1);
        static SemaphoreSlim _LoginSemaphore = new SemaphoreSlim(1, 1);
        static SemaphoreSlim _TransferStatusSemaphore = new SemaphoreSlim(1, 1);
        ILogger<BankBotService> _Logger;
        IMerchantManager _MerchantManager = null;
        //ICardToCardAccountGroupItemManager _MerchantCardToCardAccountRelationManager;
        ICachedObjectManager _CachedObjectManager = null;
        IHttpClientFactory _HttpClientFactory = null;
        GenericManagementTokenGenerator<TenantManagementSettings> _TokenGenerator = null;
        QRRegisterConfiguration _QRRegisterConfiguration = null;

        public int AccountReadCount { get; private set; }

        public int BankReadCount { get; private set; }

        public int LoginReadCount { get; set; }

        public BankBotService(IOptions<BankBotConfiguration> options,
            ITransactionManager transactionManager,
            IAesEncryptionService encryptionService,
            ITransactionQueryHistoryManager transactionQueryHistoryManager,
            IWithdrawalQueryHistoryManager withdrawalQueryHistoryManager,
            ILogger<BankBotService> logger,
            ICacheService cacheService,
            //ICardToCardAccountGroupItemManager cardToCardAccountGroupItemManager,
            ICachedObjectManager cachedObjectManager,
            IMerchantManager merchantManager,
            IHttpClientFactory httpClientFactory,
            GenericManagementTokenGenerator<TenantManagementSettings> tokenGenerator,
            IOptions<QRRegisterConfiguration> qrRegisterOptions
            )
        {
            _Configuration = options.Value;
            _TransactionManager = transactionManager;
            _EncryptionService = encryptionService;
            _TransactionQueryHistoryManager = transactionQueryHistoryManager;
            _WithdrawalQueryHistoryManager = withdrawalQueryHistoryManager;
            _CacheService = cacheService;
            _Logger = logger;
            _MerchantManager = merchantManager;
            //_MerchantCardToCardAccountRelationManager = cardToCardAccountGroupItemManager;
            _CachedObjectManager = cachedObjectManager;
            _HttpClientFactory = httpClientFactory;
            _TokenGenerator = tokenGenerator;
            _QRRegisterConfiguration = qrRegisterOptions.Value;
        }

        public async Task<BotTransferStatusResponse> CheckTransferStatus(int id)
        {
            var client = _HttpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/transferrequestwithcurrentstatus/{id}";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BotTransferStatusResponse>(responseContent);

                return item;
            }
        }

        public async Task<BotConfirmationResponse> Confirm(BotConfirmRequest request)
        {
            try
            {
                var accounts = await GetAccountsAsync();

                //var account = accounts.FirstOrDefault(t => t.AccountGuid == request.AccountGuid);

                var requestContentEncrypted = _EncryptionService.Encrypt(request);

                TransactionQueryHistory history = new TransactionQueryHistory();
                history.CreateDate = DateTime.UtcNow;
                history.IsCompleted = false;
                history.RequestContent = requestContentEncrypted;
                history.ResponseContent = string.Empty;
                history.TransactionId = request.TransactionId;


                await _TransactionQueryHistoryManager.AddAsync(history);
                await _TransactionQueryHistoryManager.SaveAsync();

                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                using (var content = new StringContent(JsonConvert.SerializeObject(
                    new
                    {
                        Amount = request.Amount,
                        SourceCardNumber = request.SourceCardNumber,
                        //DestinationAccountNumber = request.DestinationAccountNumber,
                        //DestinationCardNumber = request.DestinationCardNumber,
                        DateOfTransaction = request.DateOfTransaction,
                        Token = request.TransactionCode,
                        MinimumTransactionAmount = request.MinimumTransactionAmount,
                        CardToCardAccounts = request.CardToCardAccounts
                    }), Encoding.UTF8, "application/json"))
                {

                    string url = string.Empty;


                    if (string.IsNullOrEmpty(_Configuration.ConfirmUrl))
                    {
                        url = $"{_Configuration.Url}/api/statement/getStatementByCardNumber";
                    }
                    else
                    {
                        var urls = _Configuration.ConfirmUrl.Split(",", StringSplitOptions.RemoveEmptyEntries);

                        var random = new Random();
                        var index = random.Next(0, urls.Length);

                        if (index >= urls.Length)
                        {
                            index--;
                        }

                        url = $"{urls[index]}/api/statement/getStatementByCardNumber";
                    }

                    using (var response = await client.PostAsync(url, content))
                    {

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                        }

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var bankBotResponse = JsonConvert.DeserializeObject<BotConfirmationResponse>(responseContent);

                        history.IsCompleted = bankBotResponse.IsPresentInStatement;
                        history.ResponseContent = _EncryptionService.Encrypt(bankBotResponse);
                        history.UpdateDate = DateTime.UtcNow;

                        await _TransactionQueryHistoryManager.UpdateAsync(history);
                        await _TransactionQueryHistoryManager.SaveAsync();

                        return bankBotResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex, ex.GetExceptionDetailsWithStackTrace());
                return new BotConfirmationResponse()
                {
                    IsPresentInStatement = false
                };
            }

        }

        public async Task<BotConfirmationResponse> Confirm(string token)
        {
            var transaction = await _TransactionManager.GetTransactionByToken(token);

            if (transaction == null)
            {
                throw new Exception($"Invalid token: {token}");
            }

            if (transaction.TransactionStatus == TransactionStatus.Completed)
            {
                return new BotConfirmationResponse()
                {
                    TransactonNumberOfStatements = transaction.BankNumber == null ? new List<string>() : transaction.BankNumber.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
                    IsPresentInStatement = true,
                    Message = string.Empty,
                    TransactionDateTime = transaction.TransferredDate
                };
            }

            if (transaction.TransactionStatus != TransactionStatus.WaitingConfirmation)
            {
                throw new Exception($"Transaction status is not valid for confirmation. Token : {token}. Status : {transaction.Status}");
            }

            var merchant = await _MerchantManager.GetEntityByIdAsync(transaction.MerchantId);

            var groupItems = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            var accounts = groupItems.Where(t => t.CardToCardAccountGroupId == merchant.CardToCardAccountGroupId && t.AllowCardToCard == true).ToList();

            //accounts = accounts.Where(t => t.RelationStatus != CardToCardAccountGroupItemStatus.Blocked).ToList();

            List<BankBotConfirmAccount> accountList = new List<BankBotConfirmAccount>();
            var cardToCardAccounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

            for (int i = 0; i < accounts.Count; i++)
            {
                var account = accounts[i];

                var confirmAccount = new BankBotConfirmAccount();

                var cardToCardAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == account.CardToCardAccountId);
                confirmAccount.AccountGuid = cardToCardAccount.AccountGuid;
                confirmAccount.CardNumber = _EncryptionService.DecryptToString(cardToCardAccount.CardNumber);

                if (!string.IsNullOrEmpty(confirmAccount.CardNumber))
                {
                    var bankBotAccount = await GetAccountByGuid(cardToCardAccount.AccountGuid);

                    confirmAccount.AccountNumber = bankBotAccount.AccountNo;
                    confirmAccount.BlockedDate = account.RelationStatus != CardToCardAccountGroupItemStatus.Blocked ? (DateTime?)null : account.BlockedDate;

                    accountList.Add(confirmAccount);
                }
            }

            if (!accountList.Any(t => t.AccountGuid == transaction.AccountGuid))
            {
                accountList.Add(new BankBotConfirmAccount()
                {
                    AccountGuid = transaction.AccountGuid,
                    AccountNumber = _EncryptionService.DecryptToString(transaction.AccountNumber),
                    CardNumber = _EncryptionService.DecryptToString(transaction.CardNumber),
                    BlockedDate = null
                });
            }

            var real = accountList.FirstOrDefault(t => t.AccountGuid == transaction.AccountGuid);

            if (real != null)
            {
                accountList.Remove(real);
                accountList.Insert(0, real);
            }

            var request = new BotConfirmRequest();
            //request.DestinationAccountNumber = _EncryptionService.DecryptToString(transaction.AccountNumber);
            request.SourceCardNumber = _EncryptionService.DecryptToString(transaction.CustomerCardNumber);
            //request.DestinationCardNumber = _EncryptionService.DecryptToString(transaction.CardNumber);
            request.TransactionCode = token;
            request.DateOfTransaction = transaction.CreationDate;
            request.TransactionId = transaction.Id;
            request.Amount = transaction.TransactionAmount;
            request.MinimumTransactionAmount = merchant.MinimumTransactionAmount;
            request.CardToCardAccounts = accountList;

            var response = await Confirm(request);

            return response;
        }

        public async Task<BotAccountInformation> GetAccountByGuid(string accountGuid)
        {
            var accounts = await GetAccountsAsync();

            return accounts.FirstOrDefault(t => t.AccountGuid == accountGuid);
        }

        public async Task<List<BotAccountInformation>> GetAccountsAsync()
        {
            var cachedAccounts = _CacheService.Get<List<BotAccountInformation>>(BankBotCacheKeys.Accounts);

            if (cachedAccounts != null)
            {
                return cachedAccounts;
            }

            try
            {
                await _AccountsSemaphore.WaitAsync();
                cachedAccounts = _CacheService.Get<List<BotAccountInformation>>(BankBotCacheKeys.Accounts);

                if (cachedAccounts != null)
                {
                    return cachedAccounts;
                }

                AccountReadCount++;

                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/account";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var accounts = JsonConvert.DeserializeObject<List<BotAccountInformation>>(responseContent);
                    var logins = await GetLogins();

                    accounts.ForEach(account =>
                    {
                        var login = logins.FirstOrDefault(t => t.Id == account.LoginId);
                        if (login != null)
                        {
                            account.LoginGuid = login.LoginGuid;
                        }
                    });

                    _CacheService.Set(BankBotCacheKeys.Accounts, accounts, _CacheTimeout);

                    return accounts;
                }
            }
            finally
            {
                _AccountsSemaphore.Release();
            }
        }

        public Task<List<BotAccountInformation>> GetAccountsByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BotBankInformation>> GetBanks()
        {
            var cachedBanks = _CacheService.Get<List<BotBankInformation>>(BankBotCacheKeys.Banks);

            if (cachedBanks != null)
            {
                return cachedBanks;
            }

            try
            {
                await _BankSemaphore.WaitAsync();
                cachedBanks = _CacheService.Get<List<BotBankInformation>>(BankBotCacheKeys.Banks);

                if (cachedBanks != null)
                {
                    return cachedBanks;
                }

                BankReadCount++;

                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/bank/getbanks";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var banks = JsonConvert.DeserializeObject<List<BotBankInformation>>(responseContent);

                    banks.ForEach(bank =>
                    {
                        bank.BankName = bank.BankNameInFarsi;
                    });

                    _CacheService.Set(BankBotCacheKeys.Banks, banks);

                    return banks;
                }
            }
            finally
            {
                _BankSemaphore.Release();
            }
        }

        public async Task<List<BotLoginInformation>> GetLogins()
        {
             var cachedLogins = _CacheService.Get<List<BotLoginInformation>>(BankBotCacheKeys.Logins);

            if (cachedLogins != null)
            {
                return cachedLogins;
            }

            try
            {

                await _LoginSemaphore.WaitAsync();
                cachedLogins = _CacheService.Get<List<BotLoginInformation>>(BankBotCacheKeys.Logins);

                if (cachedLogins != null)
                {
                    return cachedLogins;
                }

                LoginReadCount++;

                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/login";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var logins = JsonConvert.DeserializeObject<List<BotLoginInformation>>(responseContent);

                    _CacheService.Set(BankBotCacheKeys.Logins, logins);

                    return logins;
                }
            }
            finally
            {
                _LoginSemaphore.Release();
            }
        }

        public async Task<List<BotLoginSelect>> GetLoginSelect()
        {
            var logins = await GetLogins();

            var banks = await GetBanks();

            var qrRegistrationDetails = await GetQRRegistrationDetails();

            var loginDeviceDetails = await GetLoginDeviceDetails();

            var loginRecentDetails = await GetLoginRecentDetails();

            var selects = new List<BotLoginSelect>();

            logins.ForEach(t =>
            {
                var bank = banks.FirstOrDefault(p => p.Id == t.BankId);
                var qrRegistrationDetail = qrRegistrationDetails.FirstOrDefault(q => q.LoginId == t.Id);
                var loginDeviceDetail = loginDeviceDetails.FirstOrDefault(q => q.LoginId == t.Id);
                var loginRecentDatail = loginRecentDetails.FirstOrDefault(q => q.Id == t.Id);

                selects.Add(new BotLoginSelect()
                {
                    Id = t.Id,
                    BankName = bank?.BankName,
                    LoginGuid = t.LoginGuid,
                    BankId = t.BankId,
                    IsBlocked = t.IsBlocked,
                    IsDeleted = t.IsDeleted,
                    IsBlockCard = t.IsBlockCard,
                    QRRegistrationId = qrRegistrationDetail == null ? 0 : qrRegistrationDetail.Id,
                    QRRegistrationStatus = qrRegistrationDetail == null ? "" : qrRegistrationDetail.QRRegistrationStatus,
                    QRRegistrationStatusId = qrRegistrationDetail == null ? 0 : qrRegistrationDetail.Status,
                    LoginDeviceStatusId = loginDeviceDetail == null ? 0 : loginDeviceDetail.Status,
                    MobileNumber = t.MobileNumber,
                    BankConnectionProgram = loginRecentDatail == null ? "" :loginRecentDatail.BankConnectionProgram
                });
            });

            return selects;
        }

        public async Task<BotLoginInformation> InsertLoginAsync(BotLoginCreateInformation login)
        {
            var client = _HttpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    bankId = login.BankId,
                    username = login.Username,
                    password = login.Password,
                    loginType = login.LoginType,
                    callbackUrl = login.CallbackUrl
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/login";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);
                }
            }
        }

        public async Task<BotWithdrawalResponse> Transfer(BotWithdrawalRequest request)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    TransferFromAccount = request.TransferFromAccount,
                    TransferToAccount = request.TransferToAccount,
                    TransferBalance = request.TransferBalance,
                    TransferPriority = request.TransferPriority,
                    TransferType = request.TransferType,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/transfer";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<BotWithdrawalResponse>(responseContent);
                }
            }
        }

        public async Task<BotLoginInformation> UpdateLoginAsync(BankBotLoginUpdateInformation login)
        {
            var client = _HttpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    loginId = login.Id,
                    bankId = login.BankId,
                    username = login.Username,
                    password = login.Password,
                    mobileusername = login.MobileUsername,
                    mobilepassword = login.MobilePassword,
                    secondPassword = login.SecondPassword,
                    mobileNumber = login.MobileNumber,
                    emailAddress = login.EmailAddress,
                    emailPassword = login.EmailPassword,
                    processCountIn24Hrs = login.ProcessCountIn24Hrs
                }), Encoding.UTF8, "application/json"))
            {
                var url = $"{_Configuration.Url}/api/login/changelogininformation";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);
                }
            }
        }

        public async Task<List<BotAccountInformation>> GetAccountsByLoginGuidAsync(string loginGuid)
        {
            var accounts = await GetAccountsAsync();

            return accounts.Where(t => t.LoginGuid == loginGuid).ToList();
        }

        public async Task ClearAccountCache()
        {
            await _CacheService.Remove(BankBotCacheKeys.Accounts);
        }

        public async Task ClearLoginCache()
        {
            await _CacheService.Remove(BankBotCacheKeys.Logins);
            await _CacheService.Remove(BankBotCacheKeys.LoginSelects);
        }

        public async Task<BotAccountInformation> InsertAccountAsync(BankBotAccountCreateDTO item)
        {
            var client = _HttpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    id = item.Id,
                    loginId = item.LoginId,
                    accountNo = item.AccountNo
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/account";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<BotAccountInformation>(responseContent);
                }
            }
        }

        public async Task<BotLoginInformation> CreateLoginRequest(BotLoginCreateInformation login)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    bankId = login.BankId,
                    username = login.Username,
                    password = login.Password,
                    mobileusername= login.MobileUsername,
                    mobilepassword= login.MobilePassword,
                    loginType = login.LoginType,
                    callbackUrl = login.CallbackUrl,
                    isMobileLogin = login.IsMobileLogin
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/loginrequest";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Bank bot returned unsuccess code : {error.Replace("\\", "")}");
                        }
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var info = JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);

                    return info;
                }
            }
        }

        public async Task<BotLoginInformation> CreateLoginFromLoginRequest(CreateLoginFromLoginRequestDTO model)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    loginRequestId = model.LoginRequestId,
                    accountNumber = model.AccountNumber,
                    loginType = model.LoginType,
                    loadPreviousStatements = model.LoadPreviousStatements,
                    isBlockCard = model.IsBlockCard,
                    secondPassword = model.SecondPassword,
                    mobileNumber = model.MobileNumber,
                    emailAddress = model.EmailAddress,
                    emailPassword = model.EmailPassword,
                    processCountIn24Hrs = model.ProcessCountIn24Hrs
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/loginrequest/createlogin";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);
                }
            }
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatus()
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/account/getwithstatus";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var accounts = JsonConvert.DeserializeObject<List<BankBotAccountWithStatusDTO>>(responseContent);

                return accounts;
            }
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetAccountsWithStatuses(string[] accountGuids)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/account/getwithstatuses";
            using (var content = new StringContent(JsonConvert.SerializeObject(accountGuids), Encoding.UTF8, "application/json"))
            {
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var accounts = JsonConvert.DeserializeObject<List<BankBotAccountWithStatusDTO>>(responseContent);

                    return accounts;
                }
            }
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetSingleAccountsWithStatus(string accountGuid, TransferType? transferType = null)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/account/getsingleaccountwithstatus/{accountGuid}";

            if (transferType.HasValue)
            {
                url += $"?transferType={(int)transferType.Value}";
            }

            using (var response = await client.GetAsync(url))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var accounts = JsonConvert.DeserializeObject<List<BankBotAccountWithStatusDTO>>(responseContent);

                return accounts;
            }
        }

        public async Task<BankBotAccountStatus> GetAccountStatus(string guid)
        {
            var account = await GetAccountByGuid(guid);

            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/accountstatus/" + account.AccountNo;

            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotAccountStatus>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> CreateTransferRequest(BotTransferRequestDTO model)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var url = $"{_Configuration.Url}/api/transfer";
            using (var response = await client.PostAsync(url, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    var request = JsonConvert.SerializeObject(model);
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode} Request :  {request} Response : {message}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);
            }
        }

        public async Task<BankBotTransferRequest> GetTransferRequestWithStatus(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/transferrequestwithcurrentstatus/{id}";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> DeleteTransferRequest(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer?requestId={id}";
            using (var response = await client.DeleteAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> CancelTransferRequest(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/cancelbyid/{id}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> CancelTransferRequest(string requestGuid)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/cancelbyguid/{requestGuid}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BotLoginInformation> DeactivateLoginByUsername(string username)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/login/deactivateLoginByUserName?username={username}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);
            }
        }

        public async Task DeleteLogin(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Login/{id}";
            using (var response = await client.DeleteAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }
            }
        }

        public async Task<BotLoginInformation> ActivateLoginByUsername(string username)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/login/activateLoginByUserName?username={username}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);
            }
        }

        public async Task<BotTransferHistoryResponseDTO> GetTransferHistory(int requestId)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/transfer/transferhistory/{requestId}";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BotTransferHistoryResponseDTO>(responseContent);
            }
        }

        public async Task<BankBotTransferReceiptResponse> GetTransferReceipt(BankBotTransferReceiptRequest request)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/transfer/tranfer_reciept/{request.TrackingNumber}";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var stream = await response.Content.ReadAsStreamAsync();

                var receipt = new BankBotTransferReceiptResponse();

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    receipt.Data = ms.ToArray();
                }

                receipt.ContentType = response.Content.Headers.ContentType.MediaType;
                receipt.TrackingNumber = request.TrackingNumber;

                return receipt;
            }
        }

        public async Task<BankBotTransferRequest> RetryTransferRequest(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/retrybyid/{id}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> RetryTransferRequest(string requestGuid)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/retrybyguid/{requestGuid}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> SetAsCompletedTransferRequest(int id)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/setascompletedbyid/{id}";
            using (var response = await client.PostAsync(url, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<BankBotTransferRequest> SetAsCompletedTransferRequest(string requestGuid)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/Transfer/setascompletedbyguid/{requestGuid}";
            using (var response = await client.PostAsync(url, null))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<BankBotTransferRequest>(responseContent);

                return item;
            }
        }

        public async Task<bool> SendMobileTransferInformation(MobileTransactionDTO model)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                using (var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))
                {

                    var url = $"{_Configuration.Url}/api/mobiletransaction";
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            _Logger.LogError($"Bank bot returned unsuccess code : {response.StatusCode} {message}");
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> ChangeIsBlockedCardFlag(BotLoginBlockCardChangeModel model)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                using (var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))
                {

                    var url = $"{_Configuration.Url}/api/login/changeisblockedcard";
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            _Logger.LogError($"Bank bot returned unsuccess code : {response.StatusCode} {message}");
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<List<BotBlockedCardDetail>> GetBlockedCardDetails()
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/bankcards";
            using (var response = await client.GetAsync(url))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var details = JsonConvert.DeserializeObject<List<BotBlockedCardDetail>>(responseContent);

                return details;
            }
        }

        public async Task<List<BotBlockedCardDetail>> GetBlockedCardDetails(string accountGuid)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/bankcards/GetBlockedCardListByAccountGUID/{accountGuid}";
            using (var response = await client.GetAsync(url))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var details = JsonConvert.DeserializeObject<List<BotBlockedCardDetail>>(responseContent);

                return details;
            }
        }

        public async Task<BotCardHolderRequest> CreateCardHolderRequest(BotCardHolderRequest request)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                using (var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"))
                {
                    var url = $"{_Configuration.Url}/api/cardholder";
                    using (var response = await client.PostAsync(url, content))
                    {

                        var message = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            _Logger.LogError($"Bank bot returned unsuccess code : {response.StatusCode} {message}");
                            return new BotCardHolderRequest()
                            {
                                Id = -1,
                                Name = string.Empty
                            };
                            //throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode} {message}");
                        }

                        return JsonConvert.DeserializeObject<BotCardHolderRequest>(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<BotCustomerDetail> GetCustomerDetailByCardNo(string cardNo)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/cardholder/getCardHolderNameByCardNumber/{cardNo}";
            using (var response = await client.GetAsync(url))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var details = JsonConvert.DeserializeObject<BotCustomerDetail>(responseContent);

                return details;
            }
        }

        public async Task<List<BotTransferStatus>> GetTransferStatusList()
        {
            var cachedStatuses = _CacheService.Get<List<BotTransferStatus>>(BankBotCacheKeys.TransferStatuses);

            if (cachedStatuses != null)
            {
                return cachedStatuses;
            }

            try
            {

                await _TransferStatusSemaphore.WaitAsync();
                cachedStatuses = _CacheService.Get<List<BotTransferStatus>>(BankBotCacheKeys.TransferStatuses);

                if (cachedStatuses != null)
                {
                    return cachedStatuses;
                }


                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/transfer/gettransferstatuslist";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var statuses = JsonConvert.DeserializeObject<List<BotTransferStatus>>(responseContent);

                    _CacheService.Set(BankBotCacheKeys.TransferStatuses, statuses);

                    return statuses;
                }
            }
            finally
            {
                _TransferStatusSemaphore.Release();
            }
        }

        public async Task<bool> IsSatnaApplicable(int bankId)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/Transfer/issatnaapplicable/{bankId}";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();


                    var item = JsonConvert.DeserializeObject<SatnaApplicableResponse>(responseContent);

                    return item.issatnaapplicable;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<BotLoginInformation> GetLoginByIdFromServer(int id)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/login/{id}";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();


                    var item = JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);

                    return item;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<BotLoginSelect> CreateQRRegisterRequest(int loginId)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    loginId = loginId
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/loginotprequest";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var info = JsonConvert.DeserializeObject<BotLoginSelect>(responseContent);

                    return info;
                }
            }
        }

        public async Task<QRRegisterLoginDTO> GetQRRegistrationDetailsByLoginId(int loginId)
        {

            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/loginotprequest/{loginId}";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return null;
                        }

                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var qRRegisterLoginDetails = JsonConvert.DeserializeObject<QRRegisterLoginDTO>(responseContent);

                    return qRRegisterLoginDetails;
                }
            }
            finally
            {
            }
        }
        public async Task<List<QRRegisterLoginDTO>> GetQRRegistrationDetails()
        {

            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/loginotprequest/getloginotprequests";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var qRRegisterLoginDetails = JsonConvert.DeserializeObject<List<QRRegisterLoginDTO>>(responseContent);

                    // _CacheService.Set(BankBotCacheKeys.Logins, qRRegisterLoginDetails);

                    return qRRegisterLoginDetails;
                }
            }
            finally
            {
                //  _LoginSemaphore.Release();
            }
        }
        public async Task<QRRegisterLoginDTO> RegisterQRCode(QrCodeRegistrationRequest qrRegisterDetails)
        {
            int registrationResult = 0;
            string url = "";
            var qRRegisterLoginDetails = new QRRegisterLoginDTO();
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

            var qrRegistrationDetails = await GetQRRegistrationDetailsByLoginId(qrRegisterDetails.BankLoginId);
            string base64String = await GenerateBase64FromImage(qrRegisterDetails.BankLoginId); // Convert.ToBase64String(System.IO.File.ReadAllBytes(qrRegistrationDetails.QRImagePath));

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    FileBase64 = base64String,
                    SmsCode = qrRegisterDetails.OTP,
                    UniqueKey = qrRegisterDetails.BankLoginId,
                    BankType = 1
                }), Encoding.UTF8, "application/json"))
            {
                url = $"{_QRRegisterConfiguration.Url}api/Device/RegisterDeviceQrCodeBase64";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var otpException = await response.Content.ReadAsStringAsync();
                        _Logger.LogError($"OTP service returned unsuccess code : {otpException}");
                        throw new Exception($"OTP service returned unsuccess code.");
                    }
                }
            }
            _Logger.LogInformation($"{qrRegisterDetails.BankLoginId} device is registered.");


            var clientBankbot = _HttpClientFactory.CreateClient();
            clientBankbot.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);
            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    LoginId = qrRegisterDetails.BankLoginId,
                    TransferOTP = registrationResult
                }), Encoding.UTF8, "application/json"))
            {

                url = $"{_Configuration.Url}/api/loginotprequest/updateloginotprequest/";
                using (var response = await clientBankbot.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var bankBotUpdateOTPException = await response.Content.ReadAsStringAsync();
                        _Logger.LogError($"Bank bot returned unsuccess code {bankBotUpdateOTPException}.");
                        throw new Exception($"Bank bot returned unsuccess code.");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    qRRegisterLoginDetails = JsonConvert.DeserializeObject<QRRegisterLoginDTO>(responseContent);

                }
            }

            return qRRegisterLoginDetails;
        }
        public async Task<string> GetLoginDeviceStatusByLoginId(int loginId)
        {
            try
            {
                string deviceStatus = "";
                var client = _HttpClientFactory.CreateClient();
                List<int> LoginIds = new List<int> { loginId };
                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                // var url = $"{_Configuration.Url}/api/Transfer/getloginswithdevicestatus/{loginId}";
                using (var content = new StringContent(JsonConvert.SerializeObject(
                   new
                   {
                       LoginIds = LoginIds
                   }), Encoding.UTF8, "application/json"))
                {

                    var url = $"{_Configuration.Url}/api/Transfer/getloginswithdevicestatus/";
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                        }

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<List<LoginDeviceResult>>(responseContent);

                        switch (result[0].DeviceStatus)
                        {
                            case (int)LoginDeviceStatuses.Active:
                                deviceStatus = "The device is active.";
                                break;
                            case (int)LoginDeviceStatuses.InActive:
                                deviceStatus = "The device is Inactive.";
                                break;
                            case (int)LoginDeviceStatuses.MobileNotConfigured:
                                deviceStatus = "Mobile number is not configured to this device.";
                                break;
                            case (int)LoginDeviceStatuses.Error:
                                deviceStatus = "Error while checking device status.";
                                break;
                            default:
                                deviceStatus = "Error while checking device status.";
                                break;

                        }

                        return deviceStatus;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public async Task<BotLoginInformation> GetDeviceStatusOfLogins(List<int> loginIds)
        {
            try
            {
                string deviceStatus = "";
                var client = _HttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);
                using (var content = new StringContent(JsonConvert.SerializeObject(
                   new
                   {
                       LoginIds = loginIds
                   }), Encoding.UTF8, "application/json"))
                {

                    var url = $"{_Configuration.Url}/api/Transfer/getloginswithdevicestatus/";
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                        }

                        var responseContent = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);

                        //return "Device is active.";

                        return result; //[0].Status == true ? "Device is active." : "Presently some problem in getting sms from device.";
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public async Task<List<LoginDeviceStatus>> GetLoginDeviceDetails()
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/Transfer/getlogindevicedetails";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var loginDeviceDetails = JsonConvert.DeserializeObject<List<LoginDeviceStatus>>(responseContent);

                    // _CacheService.Set(BankBotCacheKeys.Logins, qRRegisterLoginDetails);

                    return loginDeviceDetails;
                }
            }
            finally
            {
                //  _LoginSemaphore.Release();
            }
        }
        public async Task<string> GenerateBase64FromImage(int loginId)
        {
            var client = _HttpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/loginotprequest/generatebase64fromimage/{loginId}";
            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }

                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var qRRegisterLoginDetails = JsonConvert.DeserializeObject(responseContent);

                return qRRegisterLoginDetails.ToString();
            }

            return "";
        }
        public async Task<string> GenerateOTPForRegisteredDevice(int bankLoginId)
        {
            int registrationResult = 0;
            string url = "", otp = "";
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

            var qrRegistrationDetails = await GetQRRegistrationDetailsByLoginId(bankLoginId);
            if (qrRegistrationDetails != null && qrRegistrationDetails.Status == (int)QRRegisterRequestStatus.Complete)
            {
                using (var content = new StringContent(JsonConvert.SerializeObject(
                    new
                    {
                        UniqueKey = bankLoginId
                    }), Encoding.UTF8, "application/json"))
                {
                    url = $"{_QRRegisterConfiguration.Url}api/Generate";
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var otpException = await response.Content.ReadAsStringAsync();
                            _Logger.LogError($"OTP service returned unsuccess code : {otpException}");
                            throw new Exception($"OTP service returned unsuccess code.");
                        }
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var deviceOTP = JsonConvert.DeserializeObject<DeviceOTP>(responseContent);
                        otp = Convert.ToString(deviceOTP.GeneratedCode);
                    }
                }
                return otp;
            }
            else
            {
                return "Device is not registered.";
            };
        }

        public async Task<BotLoginInformation> RegisterLoginRequest(RegisterLogin login)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            using (var content = new StringContent(JsonConvert.SerializeObject(
                new
                {
                    bankId = login.BankId,
                    loginRequestId = login.LoginRequestId,
                    otp = login.Otp
                }), Encoding.UTF8, "application/json"))
            {

                var url = $"{_Configuration.Url}/api/loginrequest/registerloginwithotp";
                using (var response = await client.PostAsync(url, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Bank bot returned unsuccess code : {error.Replace("\\", "")}");
                        }
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var info = JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);

                    return info;
                }
            }
        }

        public async Task<List<BankBotAccountWithStatusDTO>> GetMultipleAccountsWithStatus(string accountGuid, TransferType? transferType = null)
        {
            var client = _HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

            var url = $"{_Configuration.Url}/api/account/getaccountandtransferthresholddetails/{accountGuid}";

            if (transferType.HasValue)
            {
                url += $"?transferType={(int)transferType.Value}";
            }

            using (var response = await client.GetAsync(url))
            {

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var accounts = JsonConvert.DeserializeObject<List<BankBotAccountWithStatusDTO>>(responseContent);

                return accounts;
            }
        }

        public async Task<BotLoginInformation> SwitchBankConnectionProgram(int id)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/login/updateconnectionprogramforlogin/{id}";
                using (var response = await client.PostAsync(url, null))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var item = JsonConvert.DeserializeObject<BotLoginInformation>(responseContent);

                    return item;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<List<BankBotLoginRecentDetails>> GetLoginRecentDetails()
        {

            try
            {
                var client = _HttpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Add("ApiKey", _Configuration.ApiKey);

                var url = $"{_Configuration.Url}/api/login/getloginrecentdetails";
                using (var response = await client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Bank bot returned unsuccess code : {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var loginRecentDetails = JsonConvert.DeserializeObject<List<BankBotLoginRecentDetails>>(responseContent);

                    return loginRecentDetails;
                }
            }
            finally
            {
                //  _LoginSemaphore.Release();
            }
        }
    }



    static class BankBotCacheKeys
    {
        public const string Banks = "[Bank Bot] Banks";
        public const string Accounts = "[Bank Bot] Accounts";
        public const string Logins = "[Bank Bot] Logins";
        public const string LoginSelects = "[Bank Bot] Login Selects";
        public const string TransferStatuses = "[Bank Bot] Transfer Statuses";
    }

    public class SatnaApplicableResponse
    {
        public bool issatnaapplicable { get; set; }
    }

}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Transactions;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Extensions;
using Newtonsoft.Json;
using System.Net.Http;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Models.Extensions;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class WithdrawalService : DatabaseServiceBase<Withdrawal, IWithdrawalManager>, IWithdrawalService
    {
        IAesEncryptionService _AesEncryptionService = null;
        //IMerchantBankAccountManager _MerchantBankAccountManager = null;
        IMerchantManager _MerchantManager = null;
        IBankBotService _BankBotService = null;
        ITransferAccountManager _TransferAccountManager = null;
        WithdrawalConfiguration _WithdrawalConfiguration = null;
        IMerchantCustomerManager _MerchantCustomerManager = null;
        ITransactionQueueService _TransactionQueueService = null;
        IHttpClientFactory _HttpClientFactory = null;
        ITransactionManager _TransactionManager = null;
        ITransactionWithdrawalRelationManager _TransactionWithdrawalRelationManager = null;
        ICachedObjectManager _CachedObjectManager = null;

        public WithdrawalService(IWithdrawalManager manager,
            ILogger<WithdrawalService> logger,
            IAesEncryptionService aesEncryptionService,
            IBankBotService bankBotService,
            IMerchantManager merchantManager,
            IMerchantCustomerManager merchantCustomerManager,
            //IMerchantBankAccountManager merchantBankAccountManager,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            ITransactionQueueService transactionQueueService,
            ITransferAccountManager transferAccountManager,
            IHttpClientFactory httpClientFactory,
            ITransactionManager transactionManager,
            ITransactionWithdrawalRelationManager transactionWithdrawalRelationManager,
            ICachedObjectManager cachedObjectManager) : base(manager, logger)
        {
            _AesEncryptionService = aesEncryptionService;
            //_MerchantBankAccountManager = merchantBankAccountManager;
            _MerchantManager = merchantManager;
            _BankBotService = bankBotService;
            _TransferAccountManager = transferAccountManager;
            _WithdrawalConfiguration = withdrawalOptions.Value;
            _TransactionQueueService = transactionQueueService;
            _MerchantCustomerManager = merchantCustomerManager;
            _HttpClientFactory = httpClientFactory;
            _TransactionManager = transactionManager;
            _TransactionWithdrawalRelationManager = transactionWithdrawalRelationManager;
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<WithdrawCheckResponseDTO> Cancel(WithdrawCancelRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.api_key))
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsNull);
                }

                if (request.withdrawal_id == 0)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                var encryptedApi = _AesEncryptionService.EncryptToBase64(request.api_key);

                var merchant = await _MerchantManager.GetMerchantByApiKey(encryptedApi);

                if (merchant == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsInvalid);
                }

                var withdraw = await Manager.CheckWithdrawalStatus(request.withdrawal_id);

                if (withdraw == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                if (merchant.Id != withdraw.MerchantId)
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsInvalid);
                }

                if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
                {
                    return new WithdrawCheckResponseDTO(WithdrawRequestResult.Success)
                    {
                        Id = withdraw.Id,
                        Amount = withdraw.Amount
                    };
                }

                if (withdraw.TransferId.HasValue)
                {
                    await _BankBotService.CancelTransferRequest(withdraw.TransferId.Value);
                }

                withdraw.CancelDate = DateTime.UtcNow;
                withdraw.WithdrawalStatus = (int)WithdrawalStatus.CancelledByUser;
                withdraw.TransferStatusDescription = "Cancelled By User";
                withdraw.TransferStatus = (int)TransferStatus.Cancelled;

                await Manager.UpdateAsync(withdraw);
                await Manager.SaveAsync();

                return new WithdrawCheckResponseDTO(WithdrawRequestResult.Success)
                {
                    Id = withdraw.Id,
                    Amount = withdraw.Amount
                };

                throw new WithdrawException(WithdrawRequestResult.NotCompleted);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WithdrawCheckResponseDTO(ex.Result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WithdrawCheckResponseDTO(WithdrawRequestResult.UnknownError);
            }
        }

        public async Task<WebResponse<Withdrawal>> Cancel(int id)
        {
            try
            {

                var withdraw = await Manager.CheckWithdrawalStatus(id);

                if (withdraw == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
                {
                    return new WebResponse<Withdrawal>(withdraw);
                }

                if (withdraw.TransferId.HasValue)
                {
                    await _BankBotService.CancelTransferRequest(withdraw.TransferId.Value);
                }

                withdraw.CancelDate = DateTime.UtcNow;
                withdraw.WithdrawalStatus = (int)WithdrawalStatus.CancelledByUser;
                withdraw.TransferStatusDescription = "Cancelled By User";
                withdraw.TransferStatus = (int)TransferStatus.Cancelled;

                await Manager.UpdateAsync(withdraw);
                await Manager.SaveAsync();

                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdraw.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdraw.TenantGuid,
                    TryCount = 0
                });

                return new WebResponse<Withdrawal>(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
        }

        public async Task<WebResponse<Withdrawal>> Retry(int id)
        {
            try
            {

                var withdraw = await Manager.Retry(id, null);

                return new WebResponse<Withdrawal>(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
        }

        public async Task<WebResponse<Withdrawal>> SetAsCompleted(int id)
        {
            try
            {

                var withdraw = await Manager.SetAsCompleted(id);

                return new WebResponse<Withdrawal>(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
        }

        public async Task<WebResponse<Withdrawal>> ChangeProcessType(int id, int processId)
        {
            try
            {
                var withdraw = await Manager.ChangeProcessType(id, processId);

                return new WebResponse<Withdrawal>(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<Withdrawal>(ex);
            }
        }

        public async Task<WebResponse> ChangeProcessType(WithdrawalSearchArgs args, int processId)
        {
            try
            {
                await Manager.ChangeProcessType(args, processId);

                return new WebResponse();
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        public async Task<WithdrawCheckResponseDTO> Check(WithdrawCheckRequest request)
        {
            Withdrawal withdraw = null;
            try
            {
                if (string.IsNullOrEmpty(request.api_key))
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsNull);
                }

                if (request.withdrawal_id == 0)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                var encryptedApi = _AesEncryptionService.EncryptToBase64(request.api_key);

                var merchant = await _MerchantManager.GetMerchantByApiKey(encryptedApi);

                if (merchant == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsInvalid);
                }

                withdraw = await Manager.CheckWithdrawalStatus(request.withdrawal_id);

                if (withdraw == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                if (merchant.Id != withdraw.MerchantId)
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsInvalid);
                }

                return await CheckWithdrawalStatus(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                var response = new WithdrawCheckResponseDTO(ex.Result, ex.Payments);

                if (withdraw != null)
                {
                    response.Amount = withdraw.Amount - withdraw.RemainingAmount;
                    response.Reference = withdraw.Reference;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WithdrawCheckResponseDTO(WithdrawRequestResult.UnknownError);
            }
        }

        public async Task<WithdrawCheckResponseDTO> Check(int id)
        {
            Withdrawal withdraw = null;
            try
            {
                withdraw = await Manager.GetEntityByIdAsync(id);

                if (withdraw == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
                }

                return await CheckWithdrawalStatus(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                var response = new WithdrawCheckResponseDTO(ex.Result, ex.Payments);

                if (withdraw != null)
                {
                    response.ReturnUrl = withdraw.ReturnUrl;
                    response.Reference = withdraw.Reference;
                    response.TrackingNumber = withdraw.TrackingNumber;
                    response.TransferNotes = withdraw.TransferNotes;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                var response = new WithdrawCheckResponseDTO(WithdrawRequestResult.UnknownError);

                if (withdraw != null)
                {
                    response.ReturnUrl = withdraw.ReturnUrl;
                    response.Reference = withdraw.Reference;
                    response.TrackingNumber = withdraw.TrackingNumber;
                    response.TransferNotes = withdraw.TransferNotes;
                }

                return response;
            }
        }

        public async Task CheckWithTransferRequestId(int transferRequestId, BankStatementItemDTO statement)
        {
            Withdrawal withdraw = null;
            try
            {
                withdraw = await Manager.GetItemAsync(t => t.TransferId == transferRequestId);

                if (withdraw == null)
                {
                    return;
                }

                var oldStatus = withdraw.TransferStatus;

                var withdrawal = await Manager.CheckWithdrawalStatus(withdraw, true , statement);

                if (withdrawal != null && withdrawal.TransferStatus != oldStatus && withdrawal.NeedCallback())
                {
                    await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                    {
                        Id = withdrawal.Id,
                        LastTryDateTime = null,
                        TenantGuid = withdrawal.TenantGuid,
                        TryCount = 0
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        public async Task<WithdrawRequestResponseDTO> CreateWithdrawRequest(WithdrawRequestDTO request)
        {
            try
            {
                DateTime? dateTime = null;

                if (string.IsNullOrEmpty(request.api_key))
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsNull);
                }

                if (request.amount <= 0)
                {
                    throw new WithdrawException(WithdrawRequestResult.AmountMustBiggerThanZero);
                }

                if (string.IsNullOrEmpty(request.reference))
                {
                    throw new WithdrawException(WithdrawRequestResult.ReferenceRequired);
                }

                if (string.IsNullOrEmpty(request.account_number) || string.IsNullOrEmpty(request.iban))
                {
                    throw new WithdrawException(WithdrawRequestResult.InvalidAccountOrIban);
                }

                if (!request.iban.CheckIbanIsValid())
                {
                    throw new WithdrawException(WithdrawRequestResult.InvalidAccountOrIban);
                }

                if (string.IsNullOrEmpty(request.card_number) || !request.card_number.CheckCardNumberIsValid())
                {
                    throw new WithdrawException(WithdrawRequestResult.InvalidCardNumber);
                }

                if (string.IsNullOrEmpty(request.first_name) || string.IsNullOrEmpty(request.last_name))
                {
                    throw new WithdrawException(WithdrawRequestResult.InvalidFirstOrLastName);
                }

                if (request.priority < 1 || request.priority > 3)
                {
                    throw new WithdrawException(WithdrawRequestResult.PriorityInvalid);
                }

                if (!string.IsNullOrEmpty(request.transfer_time))
                {
                    if (DateTime.TryParseExact(request.transfer_time, Helper.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        dateTime = dt;
                    }
                    else
                    {
                        throw new WithdrawException(WithdrawRequestResult.InvalidDateTime);
                    }
                }

                if (string.IsNullOrEmpty(request.return_url))
                {
                    throw new WithdrawException(WithdrawRequestResult.CallbackUrlIsInvalid);
                }

                var encryptedApi = _AesEncryptionService.EncryptToBase64(request.api_key);

                var merchant = await _MerchantManager.GetMerchantByApiKey(encryptedApi);

                if (merchant == null)
                {
                    throw new WithdrawException(WithdrawRequestResult.ApiKeyIsInvalid);
                }

                var oldItem = await Manager.GetOldItem(merchant.Id, request.reference);

                if (oldItem != null)
                {
                    return new WithdrawRequestResponseDTO()
                    {
                        Id = oldItem.Id,
                        Result = WithdrawRequestResult.Success
                    };
                }

                var customer = new MerchantCustomer();
                customer.ActivityScore = request.user_activity_score;
                customer.GroupName = request.user_group_name;

                if (DateTime.TryParseExact(request.user_last_activity, Helper.DateTimeFormats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime activityDate))
                {
                    customer.LastActivity = activityDate;
                }

                customer.MerchantId = merchant.Id;
                customer.OwnerGuid = merchant.OwnerGuid;

                if (DateTime.TryParseExact(request.user_register_date, Helper.DateTimeFormats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime registerDate))
                {
                    customer.RegisterDate = registerDate;
                }


                customer.MerchantId = merchant.Id;
                customer.TenantGuid = merchant.TenantGuid;
                customer.TotalDeposit = request.user_total_deposit;
                customer.TotalWithdraw = request.user_total_withdraw;
                customer.UserId = request.user_id;
                customer.WebsiteName = request.website_name;
                customer.DepositNumber = request.user_deposit_number;
                customer.WithdrawNumber = request.user_withdraw_number;
                customer.UserSportbookNumber = request.user_sportbook_number;
                customer.UserTotalSportbook = request.user_total_sportbook;
                customer.UserCasinoNumber = request.user_casino_number;
                customer.UserTotalCasino = request.user_total_casino;

                if (!string.IsNullOrEmpty(customer.UserId) && !string.IsNullOrEmpty(customer.WebsiteName))
                {
                    customer = await _MerchantCustomerManager.AddOrUpdateCustomer(customer);
                }

                var accountNumber = request.iban.Trim().GetAccountNumberFromIban();

                Withdrawal withdrawal = new Withdrawal();
                withdrawal.Amount = request.amount;
                withdrawal.RemainingAmount = request.amount;
                withdrawal.FirstName = _AesEncryptionService.EncryptToBase64(request.first_name);
                withdrawal.LastName = _AesEncryptionService.EncryptToBase64(request.last_name);
                withdrawal.OwnerGuid = merchant.OwnerGuid;
                withdrawal.Priority = request.priority;
                withdrawal.TenantGuid = merchant.TenantGuid;
                withdrawal.ToAccountNumber = string.IsNullOrEmpty(accountNumber) ? _AesEncryptionService.EncryptToBase64(request.account_number.ConvertFarsiDigitCharsToEnglish()) : _AesEncryptionService.EncryptToBase64(accountNumber);
                withdrawal.ToIbanNumber = _AesEncryptionService.EncryptToBase64(request.iban.Trim());
                withdrawal.CardNumber = _AesEncryptionService.EncryptToBase64(request.card_number);
                withdrawal.TransferDate = null;
                withdrawal.TransferNotes = string.Empty;
                withdrawal.TransferStatus = (int)TransferStatus.Incomplete;
                withdrawal.TransferStatusDescription = "Incomplete. Waiting for processing";
                withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Pending;
                withdrawal.ExpectedTransferDate = dateTime ?? DateTime.UtcNow;
                withdrawal.MerchantId = merchant.Id;
                withdrawal.MerchantCustomerId = customer?.Id;
                withdrawal.Reference = request.reference;
                withdrawal.CreateDate = DateTime.UtcNow;
                withdrawal.Description = request.description;
                withdrawal.ReturnUrl = request.return_url;

                if (Enum.IsDefined(typeof(WithdrawalProcessType), request.process_type))
                {
                    withdrawal.WithdrawalProcessType = request.process_type;
                }
                else
                {
                    withdrawal.WithdrawalProcessType = (int)WithdrawalProcessType.Transfer;
                }

                withdrawal.RequestContent = JsonConvert.SerializeObject(request);

                var item = await Manager.AddAsync(withdrawal);
                await Manager.SaveAsync();

                await Manager.InsertContentAsync(item);

                var result = new WithdrawRequestResponseDTO()
                {
                    Id = item.Id,
                    Result = WithdrawRequestResult.Success
                };

                return result;
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                var response = new WithdrawRequestResponseDTO();
                response.Id = 0;
                response.Result = ex.Result;

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WithdrawRequestResponseDTO()
                {
                    Id = 0,
                    Result = WithdrawRequestResult.UnknownError
                };
            }
        }

        public async Task<WebResponse<WithdrawalDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<WithdrawalDTO>(result.Payload);

                return new WebResponse<WithdrawalDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<WithdrawalDTO>(ex);
            }
        }

        public async Task<WebResponse<BankBotTransferReceiptResponse>> GetTransferReceipt(int id)
        {
            try
            {
                var withdrawal = await Manager.GetEntityByIdAsync(id);

                if (withdrawal == null)
                {
                    throw new Exception("Withdrawal not found");
                }

                if (string.IsNullOrEmpty(withdrawal.TrackingNumber))
                {
                    throw new Exception("Tracking number not found");
                }

                var receipt = await _BankBotService.GetTransferReceipt(new BankBotTransferReceiptRequest()
                {
                    TrackingNumber = withdrawal.TrackingNumber
                });

                return new WebResponse<BankBotTransferReceiptResponse>(receipt);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<BankBotTransferReceiptResponse>(ex);
            }
        }

        public async Task<WebResponse<WithdrawalDTO>> InsertAsync(WithdrawalDTO item)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entity = AutoMapper.Mapper.Map<Withdrawal>(item);

                    var account = _TransferAccountManager.GetEntityById(item.TransferAccountId);

                    if (account == null || !account.IsActive)
                    {
                        throw new Exception($"Bank account could not be found with id : {item.TransferAccountId}");
                    }

                    if (account.TenantGuid != item.TenantGuid)
                    {
                        throw new Exception($"Merchant and account are mismatch. Merchant: {item.TenantGuid} Account Merchant : {account.TenantGuid}");
                    }

                    if (item.Amount <= 0)
                    {
                        throw new Exception($"Amount must be bigger then zero. Amount : {item.Amount}");
                    }

                    //var fromAccount = await _BankBotService.GetAccountByGuid(_WithdrawalConfiguration.AccountGuid);

                    //if(fromAccount == null)
                    //{
                    //    throw new Exception($"From account with this guid is not found : {_WithdrawalConfiguration.AccountGuid}");
                    //}

                    //entity.FromAccountNumber = _AesEncryptionService.EncryptToBase64(fromAccount.AccountNo);
                    entity.ToAccountNumber = account.AccountNo;
                    entity.TransferRequestDate = DateTime.UtcNow;
                    entity.FirstName = account.AccountHolderFirstName;
                    entity.LastName = account.AccountHolderLastName;

                    var result = await Manager.AddAsync(entity);

                    await Manager.SaveAsync();

                    var request = new BotWithdrawalRequest();
                    request.WithdrawalId = entity.Id;
                    request.FirstName = _AesEncryptionService.DecryptToString(entity.FirstName);
                    request.LastName = _AesEncryptionService.DecryptToString(entity.LastName);
                    request.TransferBalance = entity.Amount;
                    request.TransferFromAccount = _AesEncryptionService.DecryptToString(entity.FromAccountNumber);
                    request.TransferPriority = entity.Priority;
                    request.TransferToAccount = _AesEncryptionService.DecryptToString(entity.ToAccountNumber);
                    request.TransferType = entity.TransferType;

                    var response = await _BankBotService.Transfer(request);

                    entity.TransferStatus = response.TransferStatus;
                    entity.TransferId = response.Id;

                    await Manager.UpdateAsync(entity);
                    await Manager.SaveAsync();

                    var dto = AutoMapper.Mapper.Map<WithdrawalDTO>(result);

                    scope.Complete();

                    return new WebResponse<WithdrawalDTO>(true, string.Empty, dto);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return new WebResponse<WithdrawalDTO>(ex);
                }
            }
        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>>> Search(WithdrawalSearchArgs args)
        {
            try
            {
                var response = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>>(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>>()
                {
                    Exception = ex,
                    Payload = null,
                    Success = false
                };
            }
        }

        public async Task<WebResponse<Withdrawal>> CheckRefund(int transferRequestId)
        {
            try
            {
                var withdrawal = await Manager.GetItemAsync(t => t.TransferId == transferRequestId);

                if (withdrawal == null)
                {
                    return new WebResponse<Withdrawal>(false, "Not found", null);
                }

                //withdrawal.TransferStatus = (int)TransferStatus.RefundFromBank;
                //withdrawal.TransferStatusDescription = "Refunded";
                //withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Refunded;

                //await Manager.UpdateAsync(withdrawal);
                //await Manager.SaveAsync();

                //await SendCallback(withdrawal.Id);

                await Manager.CheckWithdrawalStatus(withdrawal, true);

                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdrawal.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdrawal.TenantGuid,
                    TryCount = 0
                });

                return new WebResponse<Withdrawal>(withdrawal);
            }
            catch (Exception ex)
            {
                return new WebResponse<Withdrawal>(ex);
            }
        }

        public async Task<System.Net.HttpStatusCode?> SendCallback(int id)
        {
            var result = await Check(id);

            if (string.IsNullOrEmpty(result.ReturnUrl))
            {
                Logger.LogCritical($"Withdrawal check completed return url is empty : {JsonConvert.SerializeObject(result)}");

                return null;
            }
            else
            {
                return await SendCallback(result, id);
            }
        }

        public async Task<System.Net.HttpStatusCode?> SendCallback(WithdrawCheckResponseDTO result, int withdrawalId)
        {
            try
            {
                Logger.LogInformation($"Withdrawal informing the return url {JsonConvert.SerializeObject(result)} Id:{withdrawalId}");
                var client = _HttpClientFactory.CreateClient();

                string payments = string.Empty;

                if (result.Payments != null)
                {
                    payments = JsonConvert.SerializeObject(result.Payments) ?? string.Empty;
                }

                Dictionary<string, string> log = new Dictionary<string, string>();

                using (MultipartFormDataContent multi = new MultipartFormDataContent())
                {
                    if (result.Result == WithdrawRequestResult.Success)
                    {

                        multi.Add(new StringContent(withdrawalId.ToString()), "withdrawal_id");
                        multi.Add(new StringContent("1"), "status");
                        multi.Add(new StringContent(result.TransferNotes ?? ""), "transfer_notes");
                        multi.Add(new StringContent(result.TrackingNumber ?? ""), "tracking_number");
                        multi.Add(new StringContent(result.Reference ?? ""), "reference");
                        multi.Add(new StringContent(Convert.ToInt32(result.Amount).ToString()), "amount");
                        multi.Add(new StringContent(payments), "payments");

                        log.Add("withdrawal_id", withdrawalId.ToString());
                        log.Add("status", "1");
                        log.Add("transfer_notes", result.TransferNotes ?? "");
                        log.Add("tracking_number", result.TrackingNumber ?? "");
                        log.Add("reference", result.Reference ?? "");
                        log.Add("amount", Convert.ToInt32(result.Amount).ToString());
                        log.Add("payments", payments);
                    }
                    else
                    {
                        multi.Add(new StringContent(withdrawalId.ToString()), "withdrawal_id");
                        multi.Add(new StringContent("0"), "status");
                        multi.Add(new StringContent(result.TransferNotes ?? ""), "transfer_notes");
                        multi.Add(new StringContent(Convert.ToInt32(result.Amount).ToString()), "amount");
                        multi.Add(new StringContent(""), "tracking_number");
                        multi.Add(new StringContent(result.Reference ?? ""), "reference");
                        multi.Add(new StringContent(((int)result.Result).ToString() ?? ""), "errorCode");
                        multi.Add(new StringContent(result.ResultDescription), "errorDescription");
                        multi.Add(new StringContent(payments), "payments");

                        log.Add("withdrawal_id", withdrawalId.ToString());
                        log.Add("status", "0");
                        log.Add("transfer_notes", result.TransferNotes ?? "");
                        log.Add("tracking_number", "");
                        log.Add("reference", result.Reference ?? "");
                        log.Add("amount", Convert.ToInt32(result.Amount).ToString());
                        log.Add("errorCode", ((int)result.Result).ToString() ?? "");
                        log.Add("errorDescription", result.ResultDescription);
                        log.Add("payments", payments);
                    }



                    Logger.LogInformation($"Withdrawal informing the return url response. Content {JsonConvert.SerializeObject(log)} Id:{withdrawalId}");

                    using (var response = await client.PostAsync(result.ReturnUrl, multi))
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Logger.LogInformation($"Withdrawal informing the operator. Response Code : {response.StatusCode} Response : {responseContent} Id:{withdrawalId}");

                        return response.StatusCode;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return null;
            }
        }

        public async Task<WebResponse<List<WithdrawalTransferHistoryDTO>>> GetWithdrawalHistories(int id)
        {
            try
            {
                var items = await Manager.GetWithdrawalTransferHistories(id);

                return new WebResponse<List<WithdrawalTransferHistoryDTO>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return new WebResponse<List<WithdrawalTransferHistoryDTO>>(ex);
            }
        }

        public async Task<WebResponse<List<long>>> GetSuggestedWithdrawalAmounts(string apiKey, long amount)
        {
            var encryptedKey = _AesEncryptionService.EncryptToBase64(apiKey);

            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();

            var merchant = merchants.FirstOrDefault(t => t.ApiKey == encryptedKey);

            if (merchant == null || !merchant.IsActive || merchant.IsDeleted)
            {
                throw new Exception("Merchant is not found");
            }

            var items = 
                amount == 0 ? await _TransactionWithdrawalRelationManager.GetSuggestedWithdrawalAmountsWithZeroAmount(merchant) :
                await _TransactionWithdrawalRelationManager.GetSuggestedWithdrawalAmounts(amount, merchant);

            return new WebResponse<List<long>>(items);
        }

        private async Task<WithdrawCheckResponseDTO> CheckWithdrawalStatus(Withdrawal withdraw)
        {
            var payments = await _TransactionManager.GetCompletedWithdrawalTransactions(withdraw.Id);

            if (withdraw.TransferId.HasValue && withdraw.TransferStatus == (int)TransferStatus.Complete)
            {
                var history = await Manager.GetCompletedWithdrawalTransferHistory(withdraw.Id);

                if (history != null)
                {
                    payments.Add(new WithdrawalTransactionItem()
                    {
                        Id = history.TransferId,
                        TransactionNumber = history.TransferNotes,
                        Amount = Convert.ToInt32(history.Amount),
                        Destination = _AesEncryptionService.DecryptToString(withdraw.ToIbanNumber),
                        Source = _AesEncryptionService.DecryptToString(withdraw.FromAccountNumber),
                        Type = (int)WithdrawalProcessType.Transfer,
                        Date = history.LastCheckDate
                    });
                }
            }

            if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.Sent && withdraw.Amount != withdraw.RemainingAmount))
            {
                return new WithdrawCheckResponseDTO(WithdrawRequestResult.PartialPaid)
                {
                    Id = withdraw.Id,
                    TransferNotes = withdraw.TransferNotes,
                    TrackingNumber = withdraw.TrackingNumber,
                    Amount = withdraw.Amount - withdraw.RemainingAmount,
                    ReturnUrl = withdraw.ReturnUrl,
                    Reference = withdraw.Reference,
                    Payments = payments
                };
            }

            if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.Refunded || withdraw.TransferStatus == (int)TransferStatus.RefundFromBank)
            {
                throw new WithdrawException(WithdrawRequestResult.Refund, payments);
            }

            if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
            {
                throw new WithdrawException(WithdrawRequestResult.Cancelled, payments);
            }

            if (withdraw.TransferStatus == (int)TransferStatus.AwaitingConfirmation)
            {
                throw new WithdrawException(WithdrawRequestResult.AwaitingConfirmation, payments);
            }

            if (withdraw.TransferStatus == (int)TransferStatus.Invalid)
            {
                throw new WithdrawException(WithdrawRequestResult.Invalid, payments);
            }

            if (withdraw.TransferStatus == (int)TransferStatus.AccountNumberInvalid || withdraw.TransferStatus == (int)TransferStatus.InvalidIBANNumber)
            {
                throw new WithdrawException(WithdrawRequestResult.InvalidAccountOrIban, payments);
            }

            if (withdraw.TransferStatus == (int)TransferStatus.FailedFromBank)
            {
                throw new WithdrawException(WithdrawRequestResult.TransferFailed, payments);
            }

            if (withdraw.TransferStatus == (int)TransferStatus.Complete || withdraw.WithdrawalStatus == (int)WithdrawalStatus.Confirmed)
            {
                return new WithdrawCheckResponseDTO(WithdrawRequestResult.Success)
                {
                    Id = withdraw.Id,
                    TransferNotes = withdraw.TransferNotes,
                    TrackingNumber = withdraw.TrackingNumber,
                    Amount = withdraw.Amount,
                    ReturnUrl = withdraw.ReturnUrl,
                    Reference = withdraw.Reference,
                    Payments = payments
                };
            }

            throw new WithdrawException(WithdrawRequestResult.NotCompleted);
        }

        public async Task<WebResponse<string>> WithdrawalCallbackToMerchant(int id)
        {
            try
            {

                var withdraw = await Manager.WithdrawalCallbackToMerchant(id, null);

                return new WebResponse<string>(withdraw);
            }
            catch (WithdrawException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<string>(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<string>(ex);
            }
        }
    }
}


using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public abstract class BasePaymentTransactionService : IPaymentTransactionService
    {
        protected ILogger Logger { get; set; }

        protected IAesEncryptionService EncryptionService { get; set; }

        protected ICachedObjectManager CachedObjectManager { get; set; }

        protected ITransactionManager Manager { get; set; }

        protected IMerchantManager MerchantManager { get; set; }

        protected IMerchantCustomerManager MerchantCustomerManager { get; set; }

        protected ITransactionQueueService TransactionQueueService { get; set; }

        protected IServiceProvider ServiceProvider { get; set; }

        protected IApplicationSettingService ApplicationSettingService { get; set; }

        public BasePaymentTransactionService(ILogger logger,
            IAesEncryptionService encryptionService,
            ICachedObjectManager cachedObjectManager,
            ITransactionManager manager,
            IMerchantManager merchantManager,
            IMerchantCustomerManager merchantCustomerManager,
            ITransactionQueueService queueService,
            IServiceProvider provider,
            IApplicationSettingService applicationSettingService)
        {
            Logger = logger;
            EncryptionService = encryptionService;
            CachedObjectManager = cachedObjectManager;
            Manager = manager;
            MerchantManager = merchantManager;
            MerchantCustomerManager = merchantCustomerManager;
            TransactionQueueService = queueService;
            ServiceProvider = provider;
            ApplicationSettingService = applicationSettingService;
        }

        protected async virtual Task<Merchant> CheckTransactionCreating(CreateTransactionRequest request)
        {
            string requestJson = JsonConvert.SerializeObject(request);
            Logger.LogInformation($"Transaction starting {requestJson}");
            if (string.IsNullOrEmpty(request.ApiKey))
            {
                Logger.LogWarning($"Null Api {requestJson}");
                throw new TransactionException(TransactionResultEnum.ApiKeyIsNull);
            }

            if (request.Amount <= 0)
            {
                throw new TransactionException(TransactionResultEnum.AmountMustBiggerThanZero);
            }

            if (string.IsNullOrEmpty(request.ReturnUrl))
            {
                throw new TransactionException(TransactionResultEnum.CallbackUrlIsInvalid);
            }

            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.WebsiteName))
            {
                throw new TransactionException(TransactionResultEnum.UserIdNotFound);
            }

            var encryptedKey = EncryptionService.EncryptToBase64(request.ApiKey);

            var merchants = await CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();

            var merchant = merchants.FirstOrDefault(t => t.ApiKey == encryptedKey);

            if (merchant == null || !merchant.IsActive)
            {
                if(merchant == null)
                {
                    Logger.LogWarning($"Null Merchant {requestJson}");
                }
                else
                {
                    Logger.LogWarning($"Invalid Merchant {JsonConvert.SerializeObject(merchant)} {requestJson}"); 
                }
                throw new TransactionException(TransactionResultEnum.ApiKeyIsInvalid);
            }

            return merchant;
        }

        protected async Task<TransactionPaymentInformationWithTransaction> CheckPaymentInformationConditions(PaymentInformationRequest request)
        {
            var response = await Manager.GetTransactionPaymentInformation(request.Token);

            if (!string.IsNullOrEmpty(request.DeviceKey) && response.Transaction.MerchantCustomerId.HasValue)
            {
                var deviceMerchantCustomerMap = new DeviceMerchantCustomerRelation();
                deviceMerchantCustomerMap.CreateDate = DateTime.UtcNow;
                deviceMerchantCustomerMap.DeviceKey = request.DeviceKey;
                deviceMerchantCustomerMap.MerchantCustomerId = response.Transaction.MerchantCustomerId.Value;
                deviceMerchantCustomerMap.OwnerGuid = response.Transaction.OwnerGuid;
                deviceMerchantCustomerMap.TenantGuid = response.Transaction.TenantGuid;
                deviceMerchantCustomerMap.TransactionId = response.Transaction.Id;

                await MerchantCustomerManager.AddDeviceMerchantCustomerRelation(deviceMerchantCustomerMap);
            }

            if (response.Result == TransactionResultEnum.Success)
            {
                response.Transaction.TransactionStatus = TransactionStatus.TokenValidatedFromWebSite;
                response.Transaction.UpdatedDate = DateTime.UtcNow;
                response.Transaction.IpAddress = request.IpAddress;

                await Manager.UpdateAsync(response.Transaction);
                await Manager.SaveAsync();

                await CheckMaliciousUser(response.Transaction);
            }

            return response;
        }

        protected async Task AddToCallbackQueue(Transaction transaction)
        {
            await TransactionQueueService.InsertCallbackQueueItem(new CallbackQueueItem()
            {
                LastTryDateTime = null,
                TransactionCode = transaction.Token,
                TryCount = 0,
                TenantGuid = transaction.TenantGuid
            });
        }

        /// <summary>
        /// Checks the transaction status from database and returns the status.
        /// This method does not check payment status from Bank Bot
        /// </summary>
        /// <param name="token">The token of the transaction which will be checked</param>
        /// <returns></returns>
        public virtual async Task<TransactionCheckResult> Check(string token)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(token);

                if (transaction == null)
                {
                    throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
                }

                var cardNumber = string.Empty;

                if (!string.IsNullOrEmpty(transaction.CustomerCardNumber))
                {
                    cardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);//.MaskCardNumber();
                }

                if (transaction.TransactionStatus == TransactionStatus.Completed)
                {
                    return new TransactionCheckResult(TransactionResultEnum.Success)
                    {
                        Amount = Convert.ToInt32(transaction.TransactionAmount),
                        BankNumber = transaction.BankNumber,
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Cancelled)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionCancelled)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Expired)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionIsExpired)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.WaitingConfirmation)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionNotCompleted)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Fraud)
                {
                    return new TransactionCheckResult(TransactionResultEnum.SameCardNumber)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Reversed)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionReversed)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = cardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                return new TransactionCheckResult(TransactionResultEnum.TransactionNotConfirmed)
                {
                    ReturnUrl = transaction.ReturnUrl,
                    CardNumber = cardNumber,
                    PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                    Status = transaction.Status
                };
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionCheckResult(ex.Result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionCheckResult(TransactionResultEnum.UnknownError);
            }
        }

        /// <summary>
        /// Cancels the transaction with given token.
        /// If Is the transaction status completed, waiting confirmation or expired, it is not possible to cancel transaction
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<TransactionResult<Transaction>> CancelTransaction(string token)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(token);
                if (transaction == null)
                {
                    throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
                }

                if (transaction.TransactionStatus == TransactionStatus.Completed || transaction.TransactionStatus == TransactionStatus.WaitingConfirmation || transaction.TransactionStatus == TransactionStatus.Expired)
                {
                    throw new TransactionException(TransactionResultEnum.TokenIsUsed);
                }

                transaction.TransactionStatus = TransactionStatus.Cancelled;
                transaction.UpdatedDate = DateTime.UtcNow;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                return new TransactionResult<Transaction>(TransactionResultEnum.TransactionCancelled, transaction);
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return ex.CreateResult<Transaction>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionResult<Transaction>(TransactionResultEnum.UnknownError);
            }
        }

        public virtual async Task<TransactionCheckResult> Check(string token, string apiKey)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(token);

                if (transaction == null)
                {
                    throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
                }

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new TransactionException(TransactionResultEnum.ApiKeyIsNull);
                }

                var encryptedKey = EncryptionService.EncryptToBase64(apiKey);
                var merchant = await MerchantManager.GetMerchantByApiKey(encryptedKey);

                if (merchant == null || transaction.MerchantId != merchant.Id)
                {
                    throw new TransactionException(TransactionResultEnum.ApiKeyIsInvalid);
                }

                string maskedCardNumber = string.Empty;

                if (!string.IsNullOrEmpty(transaction.CustomerCardNumber))
                {
                    var cardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);
                    maskedCardNumber = cardNumber;//.MaskCardNumber();
                }

                if (transaction.TransactionStatus == TransactionStatus.Completed)
                {
                    return new TransactionCheckResult(TransactionResultEnum.Success)
                    {
                        Amount = Convert.ToInt32(transaction.TransactionAmount),
                        BankNumber = transaction.BankNumber,
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = maskedCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Cancelled)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionCancelled)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = maskedCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Expired)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionIsExpired)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = maskedCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.WaitingConfirmation)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionNotCompleted)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = maskedCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Fraud)
                {
                    return new TransactionCheckResult(TransactionResultEnum.SameCardNumber)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = transaction.CustomerCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                if (transaction.TransactionStatus == TransactionStatus.Reversed)
                {
                    return new TransactionCheckResult(TransactionResultEnum.TransactionReversed)
                    {
                        ReturnUrl = transaction.ReturnUrl,
                        CardNumber = transaction.CustomerCardNumber,
                        PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                        Status = transaction.Status
                    };
                }

                return new TransactionCheckResult(TransactionResultEnum.TransactionNotConfirmed)
                {
                    ReturnUrl = transaction.ReturnUrl,
                    CardNumber = maskedCardNumber,
                    PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile,
                    Status = transaction.Status
                };
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionCheckResult(ex.Result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionCheckResult(TransactionResultEnum.UnknownError);
            }
        }

        public virtual async Task CheckPhoneNumber(Transaction transaction, string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var blockedPhoneNumbers = await CachedObjectManager.GetCachedItems<BlockedPhoneNumber, IBlockedPhoneNumberRepository>();

                if(blockedPhoneNumbers.Any(t => t.PhoneNumber == phoneNumber))
                {
                    transaction.IsPhoneNumberBlocked = true;

                    var configuration = await ApplicationSettingService.Get<MaliciousCustomerSettings>();

                    transaction.CardNumber = EncryptionService.EncryptToBase64(configuration.FakeCardNumber);
                    transaction.CardHolderName = EncryptionService.EncryptToBase64(configuration.FakeCardHolderName);
                }
            }
        }

        public virtual async Task<bool> CheckCardNumber(Transaction transaction, string customerCardNumber)
        {
            if (!string.IsNullOrEmpty(customerCardNumber))
            {
                var blockedCardNumbers = await CachedObjectManager.GetCachedItems<BlockedCardNumber, IBlockedCardNumberRepository>();

                if(blockedCardNumbers.Any(t => t.CardNumber == customerCardNumber))
                {
                    transaction.TransactionStatus = TransactionStatus.Expired;

                    transaction.ExternalMessage = "کارت شما برای انتقال کارت به کارت مجاز نمی‌باشد";

                    return false;
                }
            }

            return true;
        }

        public virtual async Task CheckMaliciousUser(Transaction transaction)
        {
            if (transaction.IsMaliciousCustomer && transaction.MerchantCustomerId.HasValue)
            {
                var customer = await MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                if(customer != null && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                {
                    var blockedPhoneNumberManager = ServiceProvider.GetRequiredService<IBlockedPhoneNumberManager>();

                    var blockedPhoneNumber = await blockedPhoneNumberManager.GetItemAsync(t => t.PhoneNumber == customer.ConfirmedPhoneNumber);

                    if(blockedPhoneNumber == null)
                    {
                        blockedPhoneNumber = new BlockedPhoneNumber();
                        blockedPhoneNumber.BlockedDate = DateTime.UtcNow;
                        blockedPhoneNumber.InsertUserId = string.Empty;
                        blockedPhoneNumber.PhoneNumber = customer.ConfirmedPhoneNumber;

                        await blockedPhoneNumberManager.AddAsync(blockedPhoneNumber);
                        await blockedPhoneNumberManager.SaveAsync();
                    }
                }
            }
        }

        public abstract Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request);

        public abstract Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress);

        public abstract Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request);

        public abstract Task<bool> SendOtp(Transaction transaction, string cardNumber, string captcha);
    }
}

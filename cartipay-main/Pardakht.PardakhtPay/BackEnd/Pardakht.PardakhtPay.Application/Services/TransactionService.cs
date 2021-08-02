using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using System.Diagnostics;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class TransactionService : DatabaseServiceBase<Transaction, ITransactionManager>, ITransactionService
    {
        IAesEncryptionService _EncryptionService = null;
        ITransactionQueueService _TransactionQueueService = null;
        IMerchantCustomerManager _MerchantCustomerManager = null;
        AppSettings _AppSettings = null;
        ISmsService _SmsService = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IServiceProvider _Provider = null;
        IMobileTransferService _MobileTransferService = null;
        IMobileTransferDeviceManager _MobileTransferDeviceManager = null;
        IWithdrawalManager _WithdrawalManager = null;
        CurrentUser _CurrentUser = null;
        ISekehDeviceManager _SekehDeviceManager = null;
        ISesDeviceManager _SesDeviceManager = null;
        ISadadPspDeviceManager _SadadPspDeviceManager = null;
        IMydigiDeviceManager _MydigiDeviceManager = null;
        IMerchantManager _MerchantManager = null;
        ICachedObjectManager _CachedObjectManager = null;
        IIZMobileDeviceManager _IZMobileDeviceManager = null;
        IPayment780DeviceManager _Payment780DeviceManager = null;

        public TransactionService(ITransactionManager manager,
            ILogger<TransactionService> logger,
            IAesEncryptionService encryptionService,
            ITransactionQueueService transactionQueueService,
            IOptions<AppSettings> appSettingOptions,
            IApplicationSettingService applicationSettingService,
            ISmsService smsService,
            IMerchantCustomerManager merchantCustomerManager,
            IMobileTransferService mobileTransferService,
            IMobileTransferDeviceManager mobileTransferDeviceManager,
            IWithdrawalManager withdrawalManager,
            CurrentUser currentUser,
            IServiceProvider provider,
            ISekehDeviceManager sekehDeviceManager,
            ISesDeviceManager sesDeviceManager,
            ISadadPspDeviceManager sadadPspDeviceManager,
            IMydigiDeviceManager mydigiDeviceManager,
            IMerchantManager merchantManager,
            ICachedObjectManager cachedObjectManager,
            IIZMobileDeviceManager iZMobileDeviceManager,
            IPayment780DeviceManager payment780DeviceManager) : base(manager, logger)
        {
            _EncryptionService = encryptionService;
            _TransactionQueueService = transactionQueueService;
            _MerchantCustomerManager = merchantCustomerManager;
            _AppSettings = appSettingOptions.Value;
            _SmsService = smsService;
            _ApplicationSettingService = applicationSettingService;
            _Provider = provider;
            _MobileTransferService = mobileTransferService;
            _MobileTransferDeviceManager = mobileTransferDeviceManager;
            _WithdrawalManager = withdrawalManager;
            _CurrentUser = currentUser;
            _SekehDeviceManager = sekehDeviceManager;
            _SesDeviceManager = sesDeviceManager;
            _SadadPspDeviceManager = sadadPspDeviceManager;
            _MydigiDeviceManager = mydigiDeviceManager;
            _MerchantManager = merchantManager;
            _CachedObjectManager = cachedObjectManager;
            _IZMobileDeviceManager = iZMobileDeviceManager;
            _Payment780DeviceManager = payment780DeviceManager;
        }

        public async Task<WebResponse<SmsPhoneNumberModel>> SendSms(SmsPhoneNumberModel model)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(model.InvoiceKey);

                if (transaction == null)
                {
                    throw new Exception($"Transaction could not be found with Token : {model.InvoiceKey}");
                }

                if (!transaction.MerchantCustomerId.HasValue)
                {
                    throw new Exception($"Customer id is empty for this token {model.InvoiceKey}");
                }

                if (transaction.PaymentType == (int)PaymentType.Novin)
                {
                    return new WebResponse<SmsPhoneNumberModel>(new SmsPhoneNumberModel()
                    {
                        InvoiceKey = "NoSMSNovin",
                        PhoneNumber = model.PhoneNumber                         
                    });
                }

                var customer = await _MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                if (customer == null)
                {
                    throw new Exception($"Customer could not be found for Id {transaction.MerchantCustomerId.Value} Token {model.InvoiceKey}");
                }

                await SendCustomerSms(model, customer);

                return new WebResponse<SmsPhoneNumberModel>(new SmsPhoneNumberModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    PhoneNumber = model.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<SmsPhoneNumberModel>(ex);
            }
        }

        public async Task<WebResponse<ExternalSendSmsConfirmationResult>> ExternalSendCustomerSms(ExternalSendSmsConfirmationRequest request)
        {
            try
            {
                var merchant = await _MerchantManager.GetMerchantByClearApiKey(request.ApiKey);

                //if(merchant == null)
                //{
                //    throw new Exception("Merchant could not be found");
                //}

                var customer = await _MerchantCustomerManager.AddOrUpdateCustomer(new MerchantCustomer()
                {
                    PhoneNumber = request.PhoneNumber,
                    WebsiteName = request.WebsiteName,
                    UserId = request.UserId,
                    OwnerGuid = merchant?.OwnerGuid,
                    TenantGuid = merchant?.TenantGuid
                });

                await SendCustomerSms(new SmsPhoneNumberModel()
                {
                    InvoiceKey = string.Empty,
                    PhoneNumber = request.PhoneNumber
                }, customer);

                return new WebResponse<ExternalSendSmsConfirmationResult>(new ExternalSendSmsConfirmationResult()
                {
                    MerchantCustomerId = customer.Id,
                    ConfirmationEndDate = customer.ConfirmCodeValidationEndDate
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ExternalSendSmsConfirmationResult>(ex);
            }
        }

        public async Task<WebResponse<ExternalSendSmsVerifyResult>> ExternalVerifyCustomerSms(ExternalSmsVerifiyRequest request)
        {
            try
            {
                var merchant = await _MerchantManager.GetMerchantByClearApiKey(request.ApiKey);

                //if (merchant == null)
                //{
                //    throw new Exception("Merchant could not be found");
                //}

                var customer = await _MerchantCustomerManager.GetCustomerAsync(merchant?.OwnerGuid, request.WebsiteName, request.UserId);

                var response = await VerifyCustomerSms(new SmsVerifyModel()
                {
                    VerifyCode = request.Code
                }, customer);

                if (response.Success)
                {
                    return new WebResponse<ExternalSendSmsVerifyResult>(new ExternalSendSmsVerifyResult()
                    {
                        IsWrongCode = response.Payload.IsWrongCode
                    });
                }
                else
                {
                    return new WebResponse<ExternalSendSmsVerifyResult>()
                    {
                        Success = false,
                        Payload = new ExternalSendSmsVerifyResult()
                        {
                            IsWrongCode = response.Payload == null ? response.Payload.IsWrongCode : false
                        },
                        Message = response.Message,
                        Exception = response.Exception
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ExternalSendSmsVerifyResult>(ex);
            }
        }

        private async Task SendCustomerSms(SmsPhoneNumberModel model, MerchantCustomer customer)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();
            var mobileConfiguration = await _ApplicationSettingService.Get<MobileApiConfiguration>();

            var mobileItems = mobileConfiguration.GetItems();

            if (
                mobileConfiguration.DeviceRegistrationApi != 0)
            {
                var conf = mobileItems.FirstOrDefault(t => (int)t.ApiType == mobileConfiguration.DeviceRegistrationApi);

                if (conf != null)
                {
                    mobileItems.Remove(conf);
                    mobileItems.Insert(0, conf);
                }
                else if (mobileConfiguration.DeviceRegistrationApi == (int)ApiType.IZMobile)
                {
                    mobileItems.Insert(0, new MobileApiItem()
                    {
                        ApiType = ApiType.IZMobile,
                        UseForWithdrawals = false,
                        InUse = false,
                        Order = int.MaxValue,
                        WithdrawalOrder = int.MaxValue
                    });
                }
            }

            bool sended = false;

            for (int i = 0; i < mobileItems.Count; i++)
            {
                var mobileItem = mobileItems[i];

                if (mobileItem.ApiType == ApiType.AsanPardakht)
                {
                    var devices = await _MobileTransferDeviceManager.GetItemsAsync(t => t.PhoneNumber == model.PhoneNumber);

                    if (!devices.Any(t => t.Status == (int)MobileTransferDeviceStatus.PhoneNumberVerified))
                    {
                        if (!devices.Any(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber && t.TryCount >= configuration.MaximumTryCountForRegisteringDevice))
                        {
                            var device = devices.FirstOrDefault(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber);

                            await SendAsanpardakhtSms(model, customer, device);
                            sended = true;
                            break;
                        }
                    }
                }
                else if (mobileItem.ApiType == ApiType.Sekeh)
                {
                    var devices = await _SekehDeviceManager.GetItemsAsync(t => t.PhoneNumber == model.PhoneNumber);

                    if (!devices.Any(t => t.IsRegistered))
                    {
                        if (!devices.Any(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber && t.TryCount >= mobileConfiguration.SekehMaximumTryCount))
                        {
                            var device = devices.FirstOrDefault(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber);

                            await SendSekehSms(model, customer, device);
                            sended = true;
                            break;
                        }
                    }
                }
                else if (mobileItem.ApiType == ApiType.SadadPsp)
                {
                    var devices = await _SadadPspDeviceManager.GetItemsAsync(t => t.PhoneNumber == model.PhoneNumber);

                    if (!devices.Any(t => t.IsRegistered))
                    {
                        if (!devices.Any(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber && t.TryCount >= mobileConfiguration.SadadPspMaxTryCount))
                        {
                            var device = devices.FirstOrDefault(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber);

                            await SendSadatPspSms(model, customer, device);
                            sended = true;
                            break;
                        }
                    }
                }
                else if (mobileItem.ApiType == ApiType.Mydigi)
                {
                    var devices = await _MydigiDeviceManager.GetItemsAsync(t => t.PhoneNumber == model.PhoneNumber);

                    if (!devices.Any(t => t.IsRegistered))
                    {
                        if (!devices.Any(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber && t.TryCount >= mobileConfiguration.MydigiMaxTryCount))
                        {
                            var device = devices.FirstOrDefault(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber);

                            await SendMydigiSms(model, customer, device);
                            sended = true;
                            break;
                        }
                    }
                }
                else if (mobileItem.ApiType == ApiType.IZMobile
                    && customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                {
                    var device = await _IZMobileDeviceManager.GetItemAsync(t => t.PhoneNumber == customer.ConfirmedPhoneNumber);

                    if (device == null || (!device.IsRegistered && device.TryCount < configuration.MaximumTryCountForRegisteringDevice))
                    {
                        await SendIZMobileSms(model, customer, device);
                        sended = true;
                        break;
                    }
                }
                else if (mobileItem.ApiType == ApiType.Payment780)
                {
                    var devices = await _Payment780DeviceManager.GetItemsAsync(t => t.PhoneNumber == model.PhoneNumber);

                    if (!devices.Any(t => t.IsRegistered))
                    {
                        if (!devices.Any(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber && t.TryCount >= mobileConfiguration.Payment780MaximumTryCount))
                        {
                            var device = devices.FirstOrDefault(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == model.PhoneNumber);

                            await SendPayment780Sms(model, customer, device);
                            sended = true;
                            break;
                        }
                    }
                }
            }

            if (!sended)
            {
                await SendSmsViaService(model, customer);
            }
        }

        public async Task<WebResponse<SmsVerifyModel>> VerifySms(SmsVerifyModel model)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(model.InvoiceKey);

                if (transaction == null)
                {
                    throw new Exception($"Transaction could not be found with Token : {model.InvoiceKey}");
                }

                if (!transaction.MerchantCustomerId.HasValue)
                {
                    throw new Exception($"Customer id is empty for this token {model.InvoiceKey}");
                }

                var customer = await _MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                if (customer == null)
                {
                    throw new Exception($"Customer could not be found for Id {transaction.MerchantCustomerId.Value} Token {model.InvoiceKey}");
                }

                WebResponse<SmsVerifyModel> response = await VerifyCustomerSms(model, customer);

                if (response.Success)
                {
                    var service = await GetService(transaction);

                    await service.CheckPhoneNumber(transaction, customer.ConfirmedPhoneNumber);

                    if (transaction.IsPhoneNumberBlocked)
                    {
                        await Manager.UpdateAsync(transaction);

                        await Manager.SaveAsync();
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<SmsVerifyModel>(ex);
            }
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyCustomerSms(SmsVerifyModel model, MerchantCustomer customer)
        {
            if (customer.ConfirmCodeValidationEndDate < DateTime.UtcNow)
            {
                throw new Exception($"Sms verification code has been expired");
            }

            WebResponse<SmsVerifyModel> response = null;

            if (customer.SmsVerificationType == (int)SmsVerificationType.SmsService)
            {
                response = await VerifyViaSmsService(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.AsanPardakht)
            {
                response = await VerifyViaMobileDevices(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.Sekeh)
            {
                response = await VerifyViaSekeh(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.SadatPsp)
            {
                response = await VerifyViaSadatPsp(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.Mydigi)
            {
                response = await VerifyViaMydigi(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.IZMobile)
            {
                response = await VerifyViaIZMobile(model, customer);
            }
            else if (customer.SmsVerificationType == (int)SmsVerificationType.Payment780)
            {
                response = await VerifyViaPayment780(model, customer);
            }

            if (response.Success)
            {
                Manager.SetCacheForMobileVerification(model.InvoiceKey);
            }

            return response;
        }

        public async Task<double> GetTimeoutDuration(string token)
        {
            try
            {
                var transaction = await Manager.GetTransactionByToken(token);

                if (transaction == null)
                {
                    throw new Exception($"Transaction could not be found with Token : {token}");
                }

                if (!transaction.MerchantCustomerId.HasValue)
                {
                    throw new Exception($"Customer id is empty for this token {token}");
                }

                var customer = await _MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                if (customer == null)
                {
                    throw new Exception($"Customer could not be found for Id {transaction.MerchantCustomerId.Value} Token {token}");
                }

                if (customer.ConfirmCodeValidationEndDate == null)
                {
                    return 60;
                }

                return customer.ConfirmCodeValidationEndDate.Value.Subtract(DateTime.UtcNow).TotalSeconds;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return 60;
            }
        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<TransactionSearchDTO>>>> Search(TransactionSearchArgs args)
        {
            try
            {
                var response = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<IEnumerable<TransactionSearchDTO>>>(response);
            }
            catch (Exception ex)
            {
                return new WebResponse<ListSearchResponse<IEnumerable<TransactionSearchDTO>>>()
                {
                    Exception = ex,
                    Payload = null,
                    Success = false
                };
            }
        }

        public async Task<WebResponse<IEnumerable<DailyAccountingDTO>>> SearchAccounting(AccountingSearchArgs args)
        {
            try
            {
                var response = await Manager.SearchAccounting(args);

                return new WebResponse<IEnumerable<DailyAccountingDTO>>(response);
            }
            catch (Exception ex)
            {
                return new WebResponse<IEnumerable<DailyAccountingDTO>>()
                {
                    Exception = ex,
                    Payload = null,
                    Success = false
                };
            }
        }

        public async Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress)
        {
            var service = GetService(request.PaymentType);

            return await service.CreateTransaction(request, ipAddress);
        }

        public async Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request)
        {
            var transaction = await Manager.GetTransactionByToken(request.Token);

            var service = await GetService(transaction);

            return await service.GetPaymentInformation(request);
        }

        public async Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request)
        {
            var watch = new Stopwatch();

            watch.Start();

            var transaction = await Manager.GetTransactionByToken(request.Token);

            var service = await GetService(transaction);

            var cardNumber = request.CustomerCardNumber;

            if (!string.IsNullOrEmpty(transaction.CustomerCardNumber))
            {
                cardNumber = _EncryptionService.DecryptToString(transaction.CustomerCardNumber);
            }

            if (!await service.CheckCardNumber(transaction, cardNumber))
            {
                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();
                return new CompletePaymentResponse(TransactionResultEnum.TransactionNotConfirmed)
                {
                    Amount = Convert.ToInt32(transaction.TransactionAmount),
                    CardNumber = cardNumber,
                    InProcess = 0,
                    Message = transaction.ExternalMessage,
                    PaymentType = transaction.PaymentType,
                    ReturnUrl = transaction.ReturnUrl,
                    Token = transaction.Token
                };
            }

            var response = await service.CompletePayment(request);

            watch.Stop();

            transaction = await Manager.GetTransactionByToken(request.Token);

            transaction.ProcessDurationInMiliseconds = Convert.ToInt32(watch.ElapsedMilliseconds);

            await Manager.UpdateAsync(transaction);
            await Manager.SaveAsync();

            return response;
        }

        public async Task<TransactionCheckResult> Check(string token)
        {
            var transaction = await Manager.GetTransactionByToken(token);

            var service = await GetService(transaction);

            return await service.Check(token);
        }

        public async Task<TransactionCheckResult> Check(string token, string apiKey)
        {
            var transaction = await Manager.GetTransactionByToken(token);

            var service = await GetService(transaction);

            return await service.Check(token, apiKey);
        }

        public async Task<TransactionResult<Transaction>> CancelTransaction(string token)
        {
            var transaction = await Manager.GetTransactionByToken(token);

            var service = await GetService(transaction);

            return await service.CancelTransaction(token);
        }

        public async Task<WebResponse> CompleteTransaction(int id)
        {
            try
            {
                var transaction = await Manager.GetItemAsync(t => t.Id == id);

                if (transaction == null)
                {
                    throw new Exception("Transaction could not be found");
                }

                if (transaction.PaymentType != (int)PaymentType.Mobile)
                {
                    throw new Exception("Payment type has to be mobile to set as completed");
                }

                if (transaction.TransactionStatus != TransactionStatus.Expired && transaction.TransactionStatus != TransactionStatus.WaitingConfirmation)
                {
                    throw new Exception("Transaction status has to be expired or waiting confirmation to set as completed");
                }

                transaction.TransactionStatus = TransactionStatus.Completed;
                transaction.UpdatedDate = DateTime.UtcNow;
                transaction.UpdateUserId = _CurrentUser.IdentifierGuid;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                //await _MerchantCustomerManager.TransactionConfirmed(transaction.MerchantCustomerId, transaction.TransactionAmount);

                await _TransactionQueueService.InsertCallbackQueueItem(new CallbackQueueItem()
                {
                    LastTryDateTime = null,
                    TenantGuid = transaction.TenantGuid,
                    TransactionCode = transaction.Token,
                    TryCount = 0
                });

                if (transaction.WithdrawalId.HasValue)
                {
                    await _WithdrawalManager.SetAsCompleted(transaction.WithdrawalId.Value, 0);
                }

                return new WebResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse> ExpireTransaction(int id)
        {
            try
            {
                var transaction = await Manager.GetItemAsync(t => t.Id == id);

                if (transaction == null)
                {
                    throw new Exception("Transaction could not be found");
                }

                if (transaction.PaymentType != (int)PaymentType.Mobile)
                {
                    throw new Exception("Payment type has to be mobile to set as completed");
                }

                if (transaction.TransactionStatus != TransactionStatus.Expired && transaction.TransactionStatus != TransactionStatus.WaitingConfirmation)
                {
                    throw new Exception("Transaction status has to be expired or waiting confirmation to set as completed");
                }

                transaction.TransactionStatus = TransactionStatus.Expired;
                transaction.UpdatedDate = DateTime.UtcNow;
                transaction.UpdateUserId = _CurrentUser.IdentifierGuid;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                if (transaction.WithdrawalId.HasValue)
                {
                    await _WithdrawalManager.Retry(transaction.WithdrawalId.Value, transaction);
                }

                return new WebResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        private IPaymentTransactionService GetService(int? paymentType)
        {
            if (paymentType == null)
            {
                return _Provider.GetRequiredService<ICardToCardPaymentService>();
            }

            var paymentTypeEnum = (PaymentType)paymentType;

            switch (paymentTypeEnum)
            {
                case PaymentType.CardToCard:
                    return _Provider.GetRequiredService<ICardToCardPaymentService>();
                case PaymentType.Mobile:
                    return _Provider.GetRequiredService<IMobilePaymentService>();
            }

            throw new NotImplementedException($"Payment transaction service has not been implemented for type {paymentType}");
        }

        private async Task<IPaymentTransactionService> GetService(Transaction transaction, bool otp = false)
        {
            var paymentTypeEnum = (PaymentType)transaction.PaymentType;

            if (!otp)
            {
                switch (paymentTypeEnum)
                {
                    case PaymentType.CardToCard:
                        return _Provider.GetRequiredService<ICardToCardPaymentService>();
                    case PaymentType.Mobile:
                    case PaymentType.SamanBank:
                    case PaymentType.MeliBank:
                    case PaymentType.Zarinpal:
                    case PaymentType.Mellat:
                    case PaymentType.Novin:
                        {
                            return _Provider.GetRequiredService<IMobilePaymentService>();
                        }
                }
            }
            else
            {
                if (paymentTypeEnum == PaymentType.Mobile
                    && transaction.ProxyPaymentAccountId.HasValue)
                {
                    var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

                    var account = accounts.FirstOrDefault(t => t.Id == transaction.ProxyPaymentAccountId.Value);

                    if (account != null)
                    {
                        paymentTypeEnum = (PaymentType)account.PaymentProviderType;
                    }
                }

                switch (paymentTypeEnum)
                {
                    case PaymentType.CardToCard:
                        return _Provider.GetRequiredService<ICardToCardPaymentService>();
                    case PaymentType.SamanBank:
                    case PaymentType.MeliBank:
                    case PaymentType.Zarinpal:
                    case PaymentType.Mellat:
                    case PaymentType.Mobile:
                        {
                            return _Provider.GetRequiredService<IMobilePaymentService>();
                        }
                    case PaymentType.Novin:
                        {
                            return _Provider.GetRequiredService<IPaymentProxyApiService>();
                        }
                }
            }

            throw new NotImplementedException($"Payment transaction service has not been implemented for type {transaction.PaymentType} {transaction.Id} {transaction.MobileTransferAccountId}");
        }

        private async Task SendSmsViaService(SmsPhoneNumberModel model, MerchantCustomer customer)
        {
            var verifyCode = new Random().Next(111111, 999999).ToString();

            var response = await _SmsService.SendSms(new SmsServiceRequest()
            {
                Message = verifyCode,
                PhoneNumber = model.PhoneNumber,
                SecretKey = string.Empty,
                TemplateId = string.Empty,
                UserApiKey = string.Empty
            });

            if (!response.IsSent)
            {
                throw new Exception("Sms could not be sended");
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.ConfirmCode = verifyCode;
            customer.ConfirmCodeValidationEndDate = response.ValidationEndDate;
            customer.SmsVerificationType = (int)SmsVerificationType.SmsService;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
        }

        private async Task SendAsanpardakhtSms(SmsPhoneNumberModel model, MerchantCustomer customer, MobileTransferDevice device)
        {
            if (device == null)
            {
                device = new MobileTransferDevice();
                device.IsActive = true;
                device.PhoneNumber = model.PhoneNumber;
                device.Status = (int)MobileTransferDeviceStatus.Created;
                device.TenantGuid = customer.TenantGuid;
                device.MerchantCustomerId = customer.Id;
                device.TryCount = 1;

                await _MobileTransferDeviceManager.AddAsync(device);
                await _MobileTransferDeviceManager.SaveAsync();
            }
            else
            {
                device.IsActive = true;
                device.Status = (int)MobileTransferDeviceStatus.Created;
                device.LastBlockDate = null;
                device.TryCount++;
            }

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = device.PhoneNumber,
                ApiType = (int)ApiType.AsanPardakht
            });

            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            customer.PhoneNumber = model.PhoneNumber;
            customer.ConfirmCode = string.Empty;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.Add(configuration.ExpireTime);
            customer.SmsVerificationTryCount++;
            customer.SmsVerificationType = (int)SmsVerificationType.AsanPardakht;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            response.CheckError();

            device.Status = (int)MobileTransferDeviceStatus.VerifyCodeSended;
            device.VerifyCodeSendDate = DateTime.UtcNow;
            device.ExternalId = response.Result.Id;
            device.ExternalStatus = response.Result.Msg;

            var result = await _MobileTransferDeviceManager.UpdateAsync(device);
            await _MobileTransferDeviceManager.SaveAsync();
        }

        private async Task SendHamrahCardSms(SmsPhoneNumberModel model, MerchantCustomer customer)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            customer.PhoneNumber = model.PhoneNumber;
            customer.ConfirmCode = string.Empty;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(1);
            customer.HamrahCardTryCount++;
            customer.SmsVerificationType = (int)SmsVerificationType.HamrahCard;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.HamrahCard
            });

            response.CheckError();
        }

        private async Task SendSekehSms(SmsPhoneNumberModel model, MerchantCustomer customer, SekehDevice device)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            if (device == null)
            {
                device = new SekehDevice();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _SekehDeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _SekehDeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.Sekeh;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _SekehDeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.Sekeh
            });

            response.CheckError();

            //device.IsRegistered = true;
            //device.RegistrationDate = DateTime.UtcNow;
            device.ExternalId = response.Result.Id;

            await _SekehDeviceManager.UpdateAsync(device);
            await _SekehDeviceManager.SaveAsync();
        }

        private async Task SendPayment780Sms(SmsPhoneNumberModel model, MerchantCustomer customer, Payment780Device device)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            if (device == null)
            {
                device = new Payment780Device();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _Payment780DeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _Payment780DeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.Payment780;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _Payment780DeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.Payment780
            });

            response.CheckError();

            //device.IsRegistered = true;
            //device.RegistrationDate = DateTime.UtcNow;
            device.ExternalId = response.Result.Id;

            await _Payment780DeviceManager.UpdateAsync(device);
            await _Payment780DeviceManager.SaveAsync();
        }

        private async Task SendSesSms(SmsPhoneNumberModel model, MerchantCustomer customer, SesDevice device)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            if (device == null)
            {
                device = new SesDevice();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _SesDeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _SesDeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.Ses;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _SesDeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.Ses
            });

            response.CheckError();

            //device.IsRegistered = true;
            //device.RegistrationDate = DateTime.UtcNow;
            device.ExternalId = response.Result.Id;

            await _SesDeviceManager.UpdateAsync(device);
            await _SesDeviceManager.SaveAsync();
        }

        private async Task SendSadatPspSms(SmsPhoneNumberModel model, MerchantCustomer customer, SadadPspDevice device)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            if (device == null)
            {
                device = new SadadPspDevice();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _SadadPspDeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _SadadPspDeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.SadatPsp;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _SadadPspDeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.SadadPsp
            });

            response.CheckError();

            device.ExternalId = response.Result.Id;

            await _SadadPspDeviceManager.UpdateAsync(device);
            await _SadadPspDeviceManager.SaveAsync();
        }

        private async Task SendIZMobileSms(SmsPhoneNumberModel model, MerchantCustomer customer, IZMobileDevice device)
        {
            if (device == null)
            {
                device = new IZMobileDevice();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _IZMobileDeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _IZMobileDeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.IZMobile;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _IZMobileDeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.IZMobile
            });

            response.CheckError();

            device.ExternalId = response.Result.Id;

            await _IZMobileDeviceManager.UpdateAsync(device);
            await _IZMobileDeviceManager.SaveAsync();
        }

        private async Task SendMydigiSms(SmsPhoneNumberModel model, MerchantCustomer customer, MydigiDevice device)
        {
            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

            if (device == null)
            {
                device = new MydigiDevice();
                device.IsRegistered = false;
                device.PhoneNumber = model.PhoneNumber;
                device.TryCount = 1;
                device.MerchantCustomerId = customer.Id;

                await _MydigiDeviceManager.AddAsync(device);
            }
            else
            {
                device.TryCount++;

                await _MydigiDeviceManager.UpdateAsync(device);
            }

            customer.PhoneNumber = model.PhoneNumber;
            customer.SmsVerificationType = (int)SmsVerificationType.Mydigi;
            customer.ConfirmCodeValidationEndDate = DateTime.UtcNow.AddMinutes(4);

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();
            await _MydigiDeviceManager.SaveAsync();

            var response = await _MobileTransferService.SendSMSAsync(new Shared.Models.MobileTransfer.MobileTransferSendSmsModel()
            {
                MobileNo = model.PhoneNumber,
                ApiType = (int)ApiType.Mydigi
            });

            response.CheckError();

            device.ExternalId = response.Result.Id;

            await _MydigiDeviceManager.UpdateAsync(device);
            await _MydigiDeviceManager.SaveAsync();
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaSmsService(SmsVerifyModel model, MerchantCustomer customer)
        {
            if (customer.ConfirmCode != model.VerifyCode)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Success = false,
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    }
                };
            }

            customer.ConfirmDate = DateTime.UtcNow;
            customer.IsConfirmed = true;
            customer.ConfirmedPhoneNumber = customer.PhoneNumber;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaMobileDevices(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.AsanPardakht
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _MobileTransferDeviceManager.GetItemAsync(t => t.PhoneNumber == customer.PhoneNumber && t.MerchantCustomerId == customer.Id);

            device.Status = (int)MobileTransferDeviceStatus.PhoneNumberVerified;
            device.VerifiedDate = DateTime.UtcNow;
            device.VerificationCode = model.VerifyCode;
            device.LastBlockDate = null;

            var result = await _MobileTransferDeviceManager.UpdateAsync(device);
            await _MobileTransferDeviceManager.SaveAsync();

            customer.ConfirmDate = DateTime.UtcNow;
            customer.IsConfirmed = true;
            customer.ConfirmedPhoneNumber = customer.PhoneNumber;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaHamrahCard(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.HamrahCard
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            customer.IsHamrahCardPhoneVerified = true;
            customer.HamrahCardVerifiedPhoneNumber = customer.PhoneNumber;
            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.HamrahCard;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaSekeh(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.Sekeh
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _SekehDeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _SekehDeviceManager.UpdateAsync(device);
            await _SekehDeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.Sekeh;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 60,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaSes(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.Ses
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _SesDeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _SesDeviceManager.UpdateAsync(device);
            await _SesDeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.Ses;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 240,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaSadatPsp(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.SadadPsp
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _SadadPspDeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _SadadPspDeviceManager.UpdateAsync(device);
            await _SadadPspDeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.SadatPsp;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 240,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaIZMobile(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.IZMobile
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _IZMobileDeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _IZMobileDeviceManager.UpdateAsync(device);
            await _IZMobileDeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.IZMobile;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 240,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaPayment780(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.Payment780
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _Payment780DeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _Payment780DeviceManager.UpdateAsync(device);
            await _Payment780DeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.Payment780;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 60,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        private async Task<WebResponse<SmsVerifyModel>> VerifyViaMydigi(SmsVerifyModel model, MerchantCustomer customer)
        {
            var response = await _MobileTransferService.ActivateDeviceAsync(new Shared.Models.MobileTransfer.MobileTransferActivateDeviceModel()
            {
                MobileNo = customer.PhoneNumber,
                VerificationCode = model.VerifyCode,
                ApiType = (int)ApiType.Mydigi
            });

            if (!response.IsSuccess)
            {
                return new WebResponse<SmsVerifyModel>()
                {
                    Message = string.Empty,
                    Payload = new SmsVerifyModel()
                    {
                        InvoiceKey = model.InvoiceKey,
                        IsWrongCode = true,
                        VerifyCode = model.VerifyCode
                    },
                    Success = false
                };
            }

            var device = await _MydigiDeviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.PhoneNumber == customer.PhoneNumber);

            device.IsRegistered = true;
            device.ExternalId = response.Result.Id;
            device.RegistrationDate = DateTime.UtcNow;

            await _MydigiDeviceManager.UpdateAsync(device);
            await _MydigiDeviceManager.SaveAsync();

            customer.ConfirmedPhoneNumber = customer.PhoneNumber;
            customer.IsConfirmed = true;
            customer.SmsVerificationType = (int)SmsVerificationType.Mydigi;

            await _MerchantCustomerManager.UpdateAsync(customer);
            await _MerchantCustomerManager.SaveAsync();

            return new WebResponse<SmsVerifyModel>()
            {
                Success = true,
                Payload = new SmsVerifyModel()
                {
                    InvoiceKey = model.InvoiceKey,
                    IsWrongCode = false,
                    Seconds = 240,
                    VerifyCode = model.VerifyCode
                }
            };
        }

        public async Task<MobileTransferResponse> SendOtpMessage(MobileTransferStartTransferModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FromCardNo) || !model.FromCardNo.CheckCardNumberIsValid())
                {
                    throw new Exception("Card number is not valid");
                }

                if (string.IsNullOrEmpty(model.Captcha))
                {
                    throw new Exception("Captcha number is not valid");
                }

                var transaction = await Manager.GetTransactionByToken(model.TransactionToken);

                if (transaction == null)
                {
                    throw new Exception("Transaction not found");
                }

                if (!transaction.MerchantCustomerId.HasValue)
                {
                    throw new Exception("Customer info not found");
                }

                var service = await GetService(transaction, true);

                var response = await service.SendOtp(transaction, model.FromCardNo, model.Captcha);

                if (response)
                {
                    return new MobileTransferResponse()
                    {
                        Result = new MobileTransferResult()
                        {
                            Id = 1
                        }
                    };
                }
                else
                {
                    return new MobileTransferResponse()
                    {
                        Error = new MobileTransferError()
                        {
                            Code = 0,
                            Desc = "Could not send otp"
                        }
                    };
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new MobileTransferResponse()
                {
                    Error = new MobileTransferError()
                    {
                        Desc = "System Error"
                    }
                };
            }
        }

        public async Task<WebResponse<string>> TransactionCallbackToMerchant(int id)
        {
            try
            {

                var transaction = await Manager.TransactionCallbackToMerchant(id);

                return new WebResponse<string>(transaction);
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

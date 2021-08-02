using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class PaymentProxyApiService : MobilePaymentService, IPaymentProxyApiService
    {
        public PaymentProxyApiService(ILogger<MobilePaymentService> logger,
            IAesEncryptionService encryptionService,
            ICachedObjectManager cachedObjectManager,
            ITransactionManager manager,
            IMerchantManager merchantManager,
            IMerchantCustomerManager merchantCustomerManager,
            ITransactionQueueService queueService,
            ITransactionQueryHistoryManager transactionQueryHistoryManager,
            IApplicationSettingService applicationSettingService,
            IServiceProvider provider) 
            : base(logger, encryptionService, cachedObjectManager, manager, merchantManager, merchantCustomerManager, queueService, transactionQueryHistoryManager, applicationSettingService, provider)
        {

        }

        public override async Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request)
        {
           try
            {
                var response = await CheckPaymentInformationConditions(request);

                var transaction = response.Transaction;

                if(transaction.TransactionStatus == TransactionStatus.TokenValidatedFromWebSite && string.IsNullOrEmpty(transaction.ExternalReference))
                {
                    await InitializePayment(transaction);
                }

                var info = new TransactionPaymentInformation()
                {
                    Amount = response.Amount,
                    TransactionId = transaction.Id,
                    ReturnUrl = transaction.ReturnUrl,
                    CreationDate = transaction.CreationDate,
                    PaymentType = PaymentType.Mobile,
                    Status = transaction.Status
                };

                if (!string.IsNullOrEmpty(response.Transaction.CustomerCardNumber))
                {
                    info.CustomerCardNumber = EncryptionService.DecryptToString(response.Transaction.CustomerCardNumber);
                }

                return new TransactionResult<TransactionPaymentInformation>(response.Result, info);
            }
            catch (TransactionException ex)
            {
                Logger.LogError(ex, ex.Message);
                return ex.CreateResult<TransactionPaymentInformation>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new TransactionResult<TransactionPaymentInformation>(TransactionResultEnum.UnknownError);
            }
        }

        public async Task InitializePayment(Transaction transaction)
        {
            try
            {
                var mobileTransferAccounts = await CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

                var mobileTransferAccount = mobileTransferAccounts.FirstOrDefault(t => t.Id == transaction.ProxyPaymentAccountId);

                if (mobileTransferAccount == null)
                {
                    throw new Exception($"Account is not found by id {transaction.MobileTransferAccountId}");
                }

                if (mobileTransferAccount.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
                {
                    throw new Exception("Please use PardakhtPal service");
                }

                if (!Enum.IsDefined(typeof(PaymentProviderTypes), mobileTransferAccount.PaymentProviderType))
                {
                    throw new Exception($"Could not found any provider for id {mobileTransferAccount.PaymentProviderType}");
                }

                var settings = ServiceProvider.GetRequiredService<IOptions<ProxyPaymentApiSettings>>().Value;

                var initializeResponse = await InitializeTransaction(transaction, mobileTransferAccount, settings);

                transaction.ExternalReference = initializeResponse.PaymentGuid;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        public async Task Transfer(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response, Merchant merchant)
        {
            await TransferMoney(request, transaction, response, merchant);
        }

        protected async override Task TransferMoney(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response, Merchant merchant)
        {
            var mobileTransferAccounts = await CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            var mobileTransferAccount = mobileTransferAccounts.FirstOrDefault(t => t.Id == transaction.ProxyPaymentAccountId);

            if(mobileTransferAccount == null)
            {
                throw new Exception($"Account is not found by id {transaction.MobileTransferAccountId}");
            }

            if(mobileTransferAccount.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
            {
                throw new Exception("Please use PardakhtPal service");
            }

            if(!Enum.IsDefined(typeof(PaymentProviderTypes), mobileTransferAccount.PaymentProviderType))
            {
                throw new Exception($"Could not found any provider for id {mobileTransferAccount.PaymentProviderType}");
            }

            var settings = ServiceProvider.GetRequiredService<IOptions<ProxyPaymentApiSettings>>().Value;

            transaction.CardNumber = EncryptionService.EncryptToBase64(mobileTransferAccount.MerchantId);
            transaction.AccountNumber = EncryptionService.EncryptToBase64(mobileTransferAccount.Title);
            transaction.PaymentType = mobileTransferAccount.PaymentProviderType;
            transaction.ApiType = 0;

            if (string.IsNullOrEmpty(transaction.ExternalReference))
            {
                var initializeResponse = await InitializeTransaction(transaction, mobileTransferAccount, settings);

                transaction.ExternalReference = initializeResponse.PaymentGuid;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();
            }

            var paymentResponse = await SendPayment(request, transaction, mobileTransferAccount, settings);

            if (!paymentResponse.Success)
            {
                transaction.TransactionStatus = TransactionStatus.Expired;
                transaction.ExternalMessage = paymentResponse.Message;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                response.ReturnUrl = transaction.ReturnUrl;
                response.Token = transaction.Token;
                response.Message = transaction.ExternalMessage;
                response.ResultCode = TransactionResultEnum.TransactionNotConfirmed;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Completed;
                transaction.BankNumber = paymentResponse.PaymentReference;
                transaction.ExternalMessage = paymentResponse.PaymentReference;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                await AddToCallbackQueue(transaction);

                response.ReturnUrl = transaction.ReturnUrl;
                response.Token = transaction.Token;
                response.Amount = Convert.ToInt32(transaction.TransactionAmount);
                response.BankNumber = transaction.BankNumber;
                response.InProcess = 0;
                response.PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile;
                response.CardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);
                response.ResultCode = TransactionResultEnum.Success;
                response.Message = paymentResponse.PaymentReference;

                //var confirmResponse = await ConfirmPayment(transaction, mobileTransferAccount, settings);

                //if (confirmResponse.Success)
                //{
                //    transaction.TransactionStatus = TransactionStatus.Completed;

                //    await Manager.UpdateAsync(transaction);
                //    await Manager.SaveAsync();

                //    response.ReturnUrl = transaction.ReturnUrl;
                //    response.Token = transaction.Token;
                //    response.Amount = Convert.ToInt32(transaction.TransactionAmount);
                //    response.BankNumber = transaction.BankNumber;
                //    response.InProcess = 0;
                //    response.PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile;
                //    response.CardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);
                //    response.ResultCode = TransactionResultEnum.Success;
                //}
                //else
                //{
                //    transaction.TransactionStatus = TransactionStatus.Expired;

                //    await Manager.UpdateAsync(transaction);
                //    await Manager.SaveAsync();

                //    response.ReturnUrl = transaction.ReturnUrl;
                //    response.Token = transaction.Token;
                //}
            }
        }

        public async override Task<bool> SendOtp(Transaction transaction, string cardNumber, string captcha)
        {
            var mobileTransferAccounts = await CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            var mobileTransferAccount = mobileTransferAccounts.FirstOrDefault(t => t.Id == transaction.ProxyPaymentAccountId);

            if (mobileTransferAccount == null)
            {
                throw new Exception($"Account is not found by id {transaction.MobileTransferAccountId}");
            }

            var settings = ServiceProvider.GetRequiredService<IOptions<ProxyPaymentApiSettings>>().Value;

            if (string.IsNullOrEmpty(transaction.ExternalReference))
            {
                var initializeResponse = await InitializeTransaction(transaction, mobileTransferAccount, settings);

                transaction.ExternalReference = initializeResponse.PaymentGuid;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();
            }

            string bankName = Helper.GetBankName((PaymentProviderTypes)mobileTransferAccount.PaymentProviderType, settings);

            var service = ServiceProvider.GetRequiredService<IPaymentProxyApiCommunicationService>();

            var response = await service.SendOtp(new SendOtpRequest()
            {
                BankName = bankName,
                BankSpecificInput = null,
                PaymentGUID = transaction.ExternalReference,
                CardNumber = cardNumber,
                Captcha = captcha
            }, transaction.Id);

            if(response != null)
            {
                if (!string.IsNullOrEmpty(response.ImageBase64Data))
                {
                    var cacheService = ServiceProvider.GetRequiredService<ICacheService>();

                    string key = $"img_{transaction.Token}";

                    var transactionConfiguration = ServiceProvider.GetRequiredService<IOptions<TransactionConfiguration>>().Value;

                    cacheService.Set(key, response.ImageBase64Data, transactionConfiguration.TransactionTimeout);
                }

                return response.OTPResult;
            }

            return false;
        }

        private async Task<InitializeTransactionResponse> InitializeTransaction(Transaction transaction, MobileTransferCardAccount account, ProxyPaymentApiSettings settings)
        {
            string bankName = Helper.GetBankName((PaymentProviderTypes)account.PaymentProviderType, settings);

            var service = ServiceProvider.GetRequiredService<IPaymentProxyApiCommunicationService>();

            var response = await service.InitializeTransaction(new InitializeTransactionRequest()
            {
                BankName = bankName,
                BankSpecificInput = null,
                MerchantId = account.MerchantId,
                PaymentAmount = Convert.ToInt64(transaction.TransactionAmount),
                PaymentReference = transaction.Id.ToString(),
                RedirectURL = transaction.ReturnUrl,
                MerchantKey = account.MerchantPassword,
                TerminalId = account.TerminalId
            }, transaction.Id);

            if (!string.IsNullOrEmpty(response.ImageBase64Data))
            {
                var cacheService = ServiceProvider.GetRequiredService<ICacheService>();

                string key = $"img_{transaction.Token}";

                var transactionConfiguration = ServiceProvider.GetRequiredService<IOptions<TransactionConfiguration>>().Value;

                cacheService.Set(key, response.ImageBase64Data, transactionConfiguration.TransactionTimeout);
            }

            return response;
        }

        private async Task<SendPaymentResponse> SendPayment(CompletePaymentRequest completePaymentRequest, Transaction transaction, MobileTransferCardAccount account, ProxyPaymentApiSettings settings)
        {
            string bankName = Helper.GetBankName((PaymentProviderTypes)account.PaymentProviderType, settings);

            var service = ServiceProvider.GetRequiredService<IPaymentProxyApiCommunicationService>();

            var request = new SendPaymentRequest()
            {
                BankName = bankName,
                CardCvv2 = completePaymentRequest.Cvv2,
                CardExpireMonth = completePaymentRequest.Month,
                CardExpireYear = completePaymentRequest.Year,
                CardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber),
                CardPIN = completePaymentRequest.Pin,
                PaymentGUID = transaction.ExternalReference,
                BankSpecificInput = null,
                Captcha = completePaymentRequest.CaptchaCode
            };

            return await service.SendPayment(request, transaction.Id);
        }

        private async Task<ConfirmPaymentResponse> ConfirmPayment(Transaction transaction, MobileTransferCardAccount account, ProxyPaymentApiSettings settings)
        {
            string bankName = Helper.GetBankName((PaymentProviderTypes)account.PaymentProviderType, settings);

            var service = ServiceProvider.GetRequiredService<IPaymentProxyApiCommunicationService>();

            var request = new ConfirmPaymentRequest()
            {
                BankName = bankName,
                PaymentGUID = transaction.ExternalReference,
                BankSpecificInput = null
            };

            return await service.ConfirmPayment(request, transaction.Id);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using Serilog;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobilePaymentService : BasePaymentTransactionService, IMobilePaymentService
    {
        protected ITransactionQueryHistoryManager TransactionQueryHistoryManager { get; set; }

        //ITransactionRequestContentRepository _TransactionRequestContentRepository = null;

        public MobilePaymentService(ILogger<MobilePaymentService> logger,
            IAesEncryptionService encryptionService,
            ICachedObjectManager cachedObjectManager,
            ITransactionManager manager,
            IMerchantManager merchantManager,
            IMerchantCustomerManager merchantCustomerManager,
            ITransactionQueueService queueService,
            ITransactionQueryHistoryManager transactionQueryHistoryManager,
            IApplicationSettingService applicationSettingService,
            IServiceProvider provider)
            : base(logger, encryptionService, cachedObjectManager, manager, merchantManager, merchantCustomerManager, queueService, provider, applicationSettingService)
        {
            TransactionQueryHistoryManager = transactionQueryHistoryManager;
        }

        public override async Task<CompletePaymentResponse> CompletePayment(CompletePaymentRequest request)
        {
            Transaction transaction = null;
            try
            {
                var result = await Manager.CheckTransactionReadyForComplete(request.Token);

                transaction = result.Item;

                if (string.IsNullOrEmpty(transaction.CustomerCardNumber))
                {
                    if (string.IsNullOrEmpty(request.CustomerCardNumber))
                    {
                        throw new TransactionException(TransactionResultEnum.InvalidCardNumber);
                    }

                    request.CustomerCardNumber = request.CustomerCardNumber.Trim();

                    if (!request.CustomerCardNumber.CheckCardNumberIsValid())
                    {
                        transaction.TransactionStatus = TransactionStatus.Expired;
                        transaction.ExternalMessage = "Invalid Card Number";

                        await Manager.UpdateAsync(transaction);
                        await Manager.SaveAsync();

                        throw new TransactionException(TransactionResultEnum.InvalidCardNumber);
                    }
                }
                else
                {
                    request.CustomerCardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);
                }

                var response = new CompletePaymentResponse(result.Result);

                response.Amount = Convert.ToInt32(transaction.TransactionAmount);
                response.BankNumber = string.Empty;
                response.ReturnUrl = transaction.ReturnUrl;
                response.Token = transaction.Token;
                response.PaymentType = transaction.PaymentType == (int)PaymentType.CardToCard ? transaction.PaymentType : (int)PaymentType.Mobile;

                transaction.IpAddress = request.IpAddress;

                response.CardNumber = request.CustomerCardNumber;

                if (result.Result == TransactionResultEnum.Success)
                {
                    if (transaction.IsMaliciousCustomer || transaction.IsPhoneNumberBlocked)
                    {
                        transaction.TransactionStatus = TransactionStatus.Expired;
                        transaction.UpdatedDate = DateTime.UtcNow;
                        transaction.BankNumber = null;
                        transaction.TransferredDate = null;
                        transaction.ExternalId = null;
                        transaction.CustomerCardNumber = !string.IsNullOrEmpty(transaction.CustomerCardNumber) ? transaction.CustomerCardNumber : EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                        transaction.ExternalMessage = string.Empty;

                        await Manager.UpdateAsync(transaction);
                        await Manager.SaveAsync();

                        response.Message = transaction.ExternalMessage;
                        response.ResultCode = TransactionResultEnum.TransactionIsExpired;
                        return response;
                    }

                    var merchants = await CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();

                    var merchant = merchants.FirstOrDefault(t => t.Id == transaction.MerchantId);

                    if (string.IsNullOrEmpty(transaction.CustomerCardNumber))
                    {
                        transaction.CustomerCardNumber = EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                    }

                    transaction.UpdatedDate = DateTime.UtcNow;
                    transaction.TransactionStatus = TransactionStatus.WaitingConfirmation;

                    await Manager.UpdateAsync(transaction);
                    await Manager.SaveAsync();

                    await TransferMoney(request, transaction, response, merchant);

                    return response;
                }

                return response;
            }
            catch (TransactionException ex)
            {
                if (transaction != null && transaction.WithdrawalId.HasValue)
                {
                    var manager = ServiceProvider.GetRequiredService<ITransactionWithdrawalRelationManager>();
                    await manager.DeleteRelation(transaction);

                    await AddTransactionWithdrawalHistory(transaction, ex.Message);
                }

                Logger.LogError(ex, ex.Message);
                return new CompletePaymentResponse(ex.Result)
                {
                    ReturnUrl = transaction?.ReturnUrl,
                    Token = transaction?.Token
                };
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.WithdrawalId.HasValue)
                {
                    var manager = ServiceProvider.GetRequiredService<ITransactionWithdrawalRelationManager>();
                    await manager.DeleteRelation(transaction);

                    await AddTransactionWithdrawalHistory(transaction, ex.Message);
                }

                Logger.LogError(ex, ex.Message);
                return new CompletePaymentResponse(TransactionResultEnum.UnknownError)
                {
                    ReturnUrl = transaction?.ReturnUrl,
                    Token = transaction?.Token
                };
            }
        }

        protected virtual async Task TransferMoney(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response, Merchant merchant)
        {
            int? mobileAccountId = transaction.MobileTransferAccountId;

            string cardNumber = transaction.CardNumber;
            string cardHolderName = transaction.CardHolderName;

            var settings = await ApplicationSettingService.Get<MobileApiConfiguration>();

            var mobileItems = settings.GetItems();

            Withdrawal withdrawal = null;

            MobileTransferDevice device = null;
            MerchantCustomer customer = null;

            if (transaction.MerchantCustomerId.HasValue)
            {
                var manager = ServiceProvider.GetRequiredService<ITransactionWithdrawalRelationManager>();
                customer = await MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                if (merchant.UseCardtoCardPaymentForWithdrawal)
                {
                    await SetApiType(transaction, mobileItems, customer, true);

                    if (transaction.ApiType != 0 && !string.IsNullOrEmpty(transaction.MobileDeviceNumber))
                    {
                        withdrawal = await manager.GetWithdrawal(transaction, merchant);

                        if (withdrawal != null)
                        {
                            transaction.CardNumber = withdrawal.CardNumber;
                            transaction.CardHolderName = EncryptionService.EncryptToBase64($"{EncryptionService.DecryptToString(withdrawal.FirstName)} {EncryptionService.DecryptToString(withdrawal.LastName)}");
                            transaction.WithdrawalId = withdrawal.Id;
                            transaction.MobileTransferAccountId = null;
                        }
                    }
                }

                if (withdrawal == null)
                {
                    await SetApiType(transaction, mobileItems, customer, false);
                }
            }
            //else if (mobileItems.Any(t => t.ApiType == ApiType.AsanPardakht))
            //{
            //    if (!transaction.MobileTransferAccountId.HasValue)
            //    {
            //        throw new TransactionException(TransactionResultEnum.DeviceNotFound);
            //    }

            //    var deviceManager = ServiceProvider.GetRequiredService<IMobileTransferDeviceManager>();
            //    transaction.ApiTypeAsEnum = ApiType.AsanPardakht;
            //    device = await deviceManager.GetItemAsync(t => t.MerchantCustomerId == transaction.MerchantCustomerId && t.Status == (int)MobileTransferDeviceStatus.PhoneNumberVerified);

            //    if (device != null)
            //    {
            //        transaction.MobileDeviceNumber = device.PhoneNumber;
            //    }
            //}

            transaction.PaymentType = (int)PaymentType.Mobile;
            transaction.AccountNumber = string.Empty;
            await Manager.UpdateAsync(transaction);
            await Manager.SaveAsync();

            if (transaction.ApiTypeAsEnum == ApiType.AsanPardakht && !transaction.WithdrawalId.HasValue && transaction.ProxyPaymentAccountId.HasValue)
            {
                await TransferWithProxyPayment(request, transaction, response, merchant);
            }
            else if ((transaction.ApiType == 0 || string.IsNullOrEmpty(transaction.MobileDeviceNumber) || !transaction.MobileTransferAccountId.HasValue) && !transaction.WithdrawalId.HasValue)
            {
                if (!transaction.ProxyPaymentAccountId.HasValue)
                {
                    transaction.ExternalMessage = "در حال حاضر این خدمت قابل ارايه نمی باشد";
                    transaction.TransactionStatus = TransactionStatus.Expired;
                    await Manager.UpdateAsync(transaction);
                    await Manager.SaveAsync();

                    throw new TransactionException(TransactionResultEnum.DeviceNotFound);
                }

                await TransferWithProxyPayment(request, transaction, response, merchant);
            }
            else
            {
                await Transfer(request, transaction, response);

                if (transaction.TransactionStatus == TransactionStatus.Expired)
                {
                    if (transaction.WithdrawalId.HasValue)
                    {
                        transaction.CardNumber = cardNumber;
                        transaction.CardHolderName = cardHolderName;
                        transaction.MobileTransferAccountId = mobileAccountId;
                        transaction.TransactionStatus = TransactionStatus.WaitingConfirmation;
                        transaction.WithdrawalId = null;

                        await Manager.UpdateAsync(transaction);
                        await Manager.SaveAsync();

                        if (!string.IsNullOrEmpty(transaction.ExternalMessage)
                            && !transaction.ExternalMessage.ToLowerInvariant().Contains("cvv"))
                        {
                            transaction.ApiType = 0;
                            transaction.MobileDeviceNumber = string.Empty;

                            await SetApiType(transaction, mobileItems, customer, false);
                        }
                        else
                        {
                            transaction.TransactionStatus = TransactionStatus.Expired;
                        }

                        await Manager.UpdateAsync(transaction);
                        await Manager.SaveAsync();

                        if (!string.IsNullOrEmpty(transaction.ExternalMessage)
                            && !transaction.ExternalMessage.ToLowerInvariant().Contains("cvv")
                            //&& response.IsProcessed
                            && transaction.ApiType != 0
                            && transaction.MobileTransferAccountId.HasValue
                            && !string.IsNullOrEmpty(transaction.MobileDeviceNumber))
                        {
                            await Transfer(request, transaction, response);

                            if (transaction.TransactionStatus == TransactionStatus.Completed && withdrawal != null)
                            {
                                var withdrawalManager = ServiceProvider.GetRequiredService<IWithdrawalManager>();
                                withdrawal = await withdrawalManager.GetItemAsync(t => t.Id == withdrawal.Id);

                                withdrawal.CardToCardTryCount++;

                                await withdrawalManager.UpdateAsync(withdrawal);
                                await withdrawalManager.SaveAsync();
                            }

                            //return response;
                        }
                        else if (transaction.ProxyPaymentAccountId.HasValue
                            && (string.IsNullOrEmpty(transaction.ExternalMessage) || !transaction.ExternalMessage.ToLowerInvariant().Contains("cvv")))
                        {
                            await TransferWithProxyPayment(request, transaction, response, merchant);
                        }
                        else
                        {
                            transaction.TransactionStatus = TransactionStatus.Expired;

                            await Manager.UpdateAsync(transaction);
                            await Manager.SaveAsync();
                        }
                    }
                    else if (transaction.ProxyPaymentAccountId.HasValue
                        && !response.IsProcessed)
                    {
                        await TransferWithProxyPayment(request, transaction, response, merchant);
                    }
                }
            }

            //return response;
        }

        private async Task SetApiType(Transaction transaction, List<MobileApiItem> mobileItems, MerchantCustomer customer, bool withdrawal)
        {
            if (!withdrawal && !transaction.MobileTransferAccountId.HasValue)
            {
                transaction.ApiType = 0;
                transaction.MobileDeviceNumber = string.Empty;

                return;
            }

            List<MobileApiItem> items = mobileItems;

            var unsupportedBins = await CachedObjectManager.GetCachedItems<UnsupportedBin, IUnsupportedBinRepository>();

            if (withdrawal)
            {
                items = items.Where(t => t.UseForWithdrawals).OrderBy(t => t.WithdrawalOrder).ToList();
            }
            else
            {
                items = items.Where(t => t.InUse).OrderBy(t => t.Order).ToList();
            }

            var cardNumber = EncryptionService.DecryptToString(transaction.CustomerCardNumber);

            var bin = cardNumber.Substring(0, 6);

            var mobileTransferService = ServiceProvider.GetRequiredService<IMobileTransferService>();

            for (int i = 0; i < items.Count; i++)
            {
                var mobileItem = items[i];

                if (mobileItem.Limit > 0 && transaction.TransactionAmount > mobileItem.Limit)
                {
                    continue;
                }

                if (mobileItem.ApiType == ApiType.HamrahCard && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.HamrahCard))
                {
                    if (customer.IsHamrahCardPhoneVerified && !string.IsNullOrEmpty(customer.HamrahCardVerifiedPhoneNumber))
                    {
                        try
                        {
                            var phoneNumber = customer.HamrahCardVerifiedPhoneNumber;

                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = customer.HamrahCardVerifiedPhoneNumber,
                                ApiType = (int)ApiType.HamrahCard
                            });

                            if (!deviceResponse.IsSuccess)
                            {
                                if (deviceResponse.Error.Code == Helper.HamrahCardTokenRemovedCode)
                                {
                                    customer.IsHamrahCardPhoneVerified = false;
                                    customer.HamrahCardVerifiedPhoneNumber = string.Empty;
                                    customer.HamrahCardTryCount = 0;
                                    customer.SmsVerificationType = 0;

                                    await MerchantCustomerManager.UpdateAsync(customer);
                                    await MerchantCustomerManager.SaveAsync();

                                    var customers = await MerchantCustomerManager.GetItemsAsync(t => t.IsHamrahCardPhoneVerified && t.HamrahCardVerifiedPhoneNumber == phoneNumber);

                                    for (int customerIndex = 0; customerIndex < customers.Count; customerIndex++)
                                    {
                                        customers[customerIndex].IsHamrahCardPhoneVerified = false;
                                        customers[customerIndex].HamrahCardVerifiedPhoneNumber = string.Empty;
                                        customers[customerIndex].HamrahCardTryCount = 0;
                                        customers[customerIndex].SmsVerificationType = 0;

                                        await MerchantCustomerManager.UpdateAsync(customers[customerIndex]);
                                    }

                                    await MerchantCustomerManager.SaveAsync();
                                }
                            }
                            else
                            {
                                transaction.ApiTypeAsEnum = ApiType.HamrahCard;
                                transaction.MobileDeviceNumber = customer.HamrahCardVerifiedPhoneNumber;

                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, ex.Message);
                        }
                    }
                }
                else if (mobileItem.ApiType == ApiType.Sekeh && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.Sekeh))
                {
                    try
                    {
                        var deviceManager = ServiceProvider.GetRequiredService<ISekehDeviceManager>();
                        var sekehDevice = await deviceManager.GetItemAsync(t => t.IsRegistered && t.MerchantCustomerId == customer.Id);

                        if (sekehDevice != null)
                        {
                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = sekehDevice.PhoneNumber,
                                ApiType = (int)ApiType.Sekeh
                            });

                            if (!deviceResponse.IsSuccess)
                            {
                                if (deviceResponse.Error.Code == Helper.SekehDeviceTokenRemovedCode)
                                {
                                    await deviceManager.DeActivateSekehDevices(sekehDevice.PhoneNumber);
                                }
                            }
                            else
                            {
                                transaction.ApiTypeAsEnum = ApiType.Sekeh;
                                transaction.MobileDeviceNumber = sekehDevice.PhoneNumber;

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message);
                    }
                }
                else if (mobileItem.ApiType == ApiType.Ses && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.Ses))
                {
                    try
                    {
                        var deviceManager = ServiceProvider.GetRequiredService<ISesDeviceManager>();
                        var sesDevice = await deviceManager.GetItemAsync(t => t.IsRegistered && t.MerchantCustomerId == customer.Id);

                        if (sesDevice != null)
                        {
                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = sesDevice.PhoneNumber,
                                ApiType = (int)ApiType.Ses
                            });

                            if (!deviceResponse.IsSuccess)
                            {
                                if (deviceResponse.Error.Code == Helper.SesDeviceTokenRemovedCode)
                                {
                                    await DeActivateSesDevices(sesDevice.PhoneNumber);
                                }
                            }
                            else
                            {
                                transaction.ApiTypeAsEnum = ApiType.Ses;
                                transaction.MobileDeviceNumber = sesDevice.PhoneNumber;

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message);
                    }
                }
                else if (mobileItem.ApiType == ApiType.SadadPsp && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.SadadPsp))
                {
                    try
                    {
                        var deviceManager = ServiceProvider.GetRequiredService<ISadadPspDeviceManager>();
                        var device = await deviceManager.GetItemAsync(t => t.IsRegistered && t.MerchantCustomerId == customer.Id);

                        if (device != null)
                        {
                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = device.PhoneNumber,
                                ApiType = (int)ApiType.SadadPsp
                            });

                            if (deviceResponse.IsSuccess)
                            {
                                transaction.ApiTypeAsEnum = ApiType.SadadPsp;
                                transaction.MobileDeviceNumber = device.PhoneNumber;

                                break;
                            }
                            else
                            {
                                if (deviceResponse.Error.Code == Helper.SadadDeviceTokenRemovedCode)
                                {
                                    await deviceManager.DeActivateDevices(device.PhoneNumber);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message);
                    }
                }
                else if (mobileItem.ApiType == ApiType.Mydigi && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.Mydigi))
                {
                    try
                    {
                        var deviceManager = ServiceProvider.GetRequiredService<IMydigiDeviceManager>();
                        var device = await deviceManager.GetItemAsync(t => t.IsRegistered && t.MerchantCustomerId == customer.Id);

                        if (device != null)
                        {
                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = device.PhoneNumber,
                                ApiType = (int)ApiType.Mydigi
                            });

                            if (!deviceResponse.IsSuccess)
                            {
                                if (deviceResponse.Error.Code == Helper.MydigiTokenRemovedCode)
                                {
                                    await deviceManager.DeActivateDevices(device.PhoneNumber);
                                }
                            }
                            else
                            {
                                transaction.ApiTypeAsEnum = ApiType.Mydigi;
                                transaction.MobileDeviceNumber = device.PhoneNumber;

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message);
                    }
                }
                else if (mobileItem.ApiType == ApiType.Payment780 && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.Payment780))
                {
                    try
                    {
                        var deviceManager = ServiceProvider.GetRequiredService<IPayment780DeviceManager>();
                        var device = await deviceManager.GetItemAsync(t => t.IsRegistered && t.MerchantCustomerId == customer.Id);

                        if (device != null)
                        {
                            var deviceResponse = await mobileTransferService.CheckDeviceStatusAsync(new MobileTransferSendSmsModel()
                            {
                                MobileNo = device.PhoneNumber,
                                ApiType = (int)ApiType.Payment780
                            });

                            if (!deviceResponse.IsSuccess)
                            {
                                if (deviceResponse.Error.Code == Helper.Payment780TokenRemoveCode)
                                {
                                    await deviceManager.DeActivateDevices(device.PhoneNumber);
                                }
                            }
                            else
                            {
                                transaction.ApiTypeAsEnum = ApiType.Payment780;
                                transaction.MobileDeviceNumber = device.PhoneNumber;

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, ex.Message);
                    }
                }
                else if (mobileItem.ApiType == ApiType.AsanPardakht && !unsupportedBins.Any(t => t.Bin == bin && t.ApiType == (int)ApiType.AsanPardakht))
                {
                    var deviceManager = ServiceProvider.GetRequiredService<IMobileTransferDeviceManager>();
                    var device = await deviceManager.GetItemAsync(t => t.MerchantCustomerId == customer.Id && t.Status == (int)MobileTransferDeviceStatus.PhoneNumberVerified);

                    if (device != null)
                    {
                        transaction.ApiTypeAsEnum = ApiType.AsanPardakht;
                        transaction.MobileDeviceNumber = device.PhoneNumber;
                        break;
                    }
                }
            }
        }

        public override async Task<TransactionResult<Transaction>> CreateTransaction(CreateTransactionRequest request, string ipAddress)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.CustomerCardNumber))
                {
                    if (!request.CustomerCardNumber.CheckCardNumberIsValid())
                    {
                        throw new TransactionException(TransactionResultEnum.InvalidCardNumber);
                    }
                }

                var merchant = await CheckTransactionCreating(request);

                if (!string.IsNullOrEmpty(request.Reference))
                {
                    var tran = await Manager.GetTransactionByReference(merchant.Id, request.Reference);

                    if (tran != null)
                    {
                        return new TransactionResult<Transaction>(tran);
                    }
                }

                MerchantCustomerMobileAccount customerAccount = null;
                MerchantCustomer customer = null;
                UserSegmentGroup group = null;
                MerchantCustomerMobileAccount customerPaymentGatewayAccount = null;

                if (!string.IsNullOrEmpty(request.UserId) && !string.IsNullOrEmpty(request.WebsiteName))
                {
                    customer = new MerchantCustomer();
                    customer.ActivityScore = request.UserActivityScore;
                    customer.GroupName = request.UserGroupName;
                    customer.LastActivity = request.UserLastActivity;
                    customer.MerchantId = merchant.Id;
                    customer.OwnerGuid = merchant.OwnerGuid;
                    customer.RegisterDate = request.UserRegisterDate;
                    customer.TenantGuid = merchant.TenantGuid;
                    customer.TotalDeposit = request.UserTotalDeposit;
                    customer.TotalWithdraw = request.UserTotalWithdraw;
                    customer.UserId = request.UserId;
                    customer.WebsiteName = request.WebsiteName;
                    customer.DepositNumber = request.UserDepositNumber;
                    customer.WithdrawNumber = request.UserWithdrawNumber;
                    customer.UserSportbookNumber = request.UserSportbookNumber;
                    customer.UserTotalSportbook = request.UserTotalSportbook;
                    customer.UserCasinoNumber = request.UserCasinoNumber;
                    customer.UserTotalCasino = request.UserTotalCasino;

                    customer = await MerchantCustomerManager.AddOrUpdateCustomer(customer);

                    customerAccount = await MerchantCustomerManager.GetCardToCardTransferAccountsForMobileTransfer(customer, merchant.Id, true);

                    customerPaymentGatewayAccount = await MerchantCustomerManager.GetCardToCardTransferAccountsForMobileTransfer(customer, merchant.Id, false);
                }
                else
                {
                    var groupManager = ServiceProvider.GetRequiredService<IUserSegmentGroupManager>();

                    group = await groupManager.GetGroup(merchant.OwnerGuid, (Dictionary<int, object>)null);

                    var manager = ServiceProvider.GetRequiredService<IMobileTransferCardAccountGroupItemManager>();

                    var account = await manager.GetRandomRelation(merchant.Id, group, true);

                    if (account != null)
                    {
                        var accounts = await CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

                        var cardAccount = accounts.FirstOrDefault(t => t.Id == account.ItemId);

                        if (cardAccount != null)
                        {
                            customerAccount = new MerchantCustomerMobileAccount()
                            {
                                Account = cardAccount,
                                Customer = null,
                                GroupItem = account,
                                UserSegmentGroup = group
                            };
                        }
                    }
                }

                if (customerAccount == null || customerAccount.Account == null)
                {
                    customerAccount = customerPaymentGatewayAccount;
                }

                string accountNo = string.Empty;
                string cardNumber = string.Empty;
                string cardHolderName = string.Empty;
                string accountGuid = string.Empty;
                string loginGuid = string.Empty;
                int? cardToCardAccountId = null;

                if (customerAccount != null && customerAccount.UserSegmentGroup != null && customerAccount.UserSegmentGroup.IsMalicious)
                {
                    var configuration = await ApplicationSettingService.Get<MaliciousCustomerSettings>();

                    cardNumber = EncryptionService.EncryptToBase64(configuration.FakeCardNumber);
                    cardHolderName = EncryptionService.EncryptToBase64(configuration.FakeCardHolderName);
                }
                else if (customerAccount != null && customerAccount.Account != null)
                {
                    if (customerAccount.Account.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
                    {
                        cardNumber = customerAccount.Account.CardNumber;
                        cardHolderName = customerAccount.Account.CardHolderName;
                    }
                    else
                    {
                        cardNumber = EncryptionService.EncryptToBase64(customerAccount.Account.MerchantId);
                        accountNo = customerAccount.Account.Title;
                    }
                }
                else
                {
                    throw new TransactionException(TransactionResultEnum.AccountNotFound);
                }

                var token = Guid.NewGuid().ToString();

                Transaction transaction = new Transaction();
                transaction.CreationDate = DateTime.UtcNow;
                transaction.CustomerCardNumber = null;
                transaction.IpAddress = ipAddress ?? string.Empty;
                transaction.MerchantId = merchant.Id;
                transaction.ReturnUrl = request.ReturnUrl;
                transaction.Token = token;
                transaction.TransactionAmount = request.Amount;
                transaction.TransactionStatus = TransactionStatus.Started;
                transaction.TransferredDate = null;
                transaction.UpdatedDate = null;
                transaction.CardNumber = cardNumber;
                transaction.AccountNumber = EncryptionService.EncryptToBase64(accountNo);
                transaction.CardHolderName = cardHolderName;
                transaction.CardToCardAccountId = cardToCardAccountId;
                transaction.TenantGuid = merchant.TenantGuid;
                transaction.OwnerGuid = merchant.OwnerGuid;
                transaction.AccountGuid = accountGuid;
                transaction.LoginGuid = loginGuid;
                transaction.RequestContent = EncryptionService.Encrypt(request);
                transaction.MerchantCustomerId = customer?.Id;
                transaction.Reference = request.Reference;
                transaction.HideCardNumber = false;
                transaction.PaymentType = customerAccount.Account.PaymentProviderType;
                transaction.IsMaliciousCustomer = false;
                transaction.UserSegmentGroupId = null;
                transaction.MobileTransferAccountId = customerAccount.Account.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal ? customerAccount.Account.Id : (int?)null;
                transaction.MobileDeviceNumber = string.Empty;
                transaction.UserSegmentGroupId = customerAccount?.UserSegmentGroup?.Id;
                transaction.ProxyPaymentAccountId = customerPaymentGatewayAccount?.Account?.Id;

                if (!string.IsNullOrEmpty(request.CustomerCardNumber))
                {
                    transaction.CustomerCardNumber = EncryptionService.EncryptToBase64(request.CustomerCardNumber);
                }

                if (customerAccount != null && customerAccount.UserSegmentGroup != null)
                {
                    transaction.IsMaliciousCustomer = customerAccount.UserSegmentGroup.IsMalicious;
                }

                if (customer != null)
                {
                    await CheckPhoneNumber(transaction, customer.ConfirmedPhoneNumber);
                }

                await Manager.AddAsync(transaction);
                await Manager.SaveAsync();

                var contentRepository = ServiceProvider.GetRequiredService<ITransactionRequestContentRepository>();

                await contentRepository.InsertAsync(new TransactionRequestContent()
                {
                    TransactionId = transaction.Id,
                    RequestContent = transaction.RequestContent
                });

                await contentRepository.SaveChangesAsync();

                return new TransactionResult<Transaction>(transaction);
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

        public override async Task<TransactionResult<TransactionPaymentInformation>> GetPaymentInformation(PaymentInformationRequest request)
        {
            try
            {
                var response = await CheckPaymentInformationConditions(request);

                var info = new TransactionPaymentInformation()
                {
                    Amount = response.Amount,
                    TransactionId = response.Transaction.Id,
                    ReturnUrl = response.Transaction.ReturnUrl,
                    CreationDate = response.Transaction.CreationDate,
                    PaymentType = PaymentType.Mobile,
                    Status = response.Transaction.Status
                };

                if (!string.IsNullOrEmpty(response.Transaction.CustomerCardNumber))
                {
                    info.CustomerCardNumber = EncryptionService.DecryptToString(response.Transaction.CustomerCardNumber);
                }

                var transaction = response.Transaction;

                if (transaction.TransactionStatus == TransactionStatus.TokenValidatedFromWebSite && transaction.ProxyPaymentAccountId.HasValue && string.IsNullOrEmpty(transaction.ExternalReference))
                {
                    Log.Information($"Initalizing transaction through payment proxy {transaction.Id}, transaction status {Enum.GetName(typeof(TransactionResultEnum),transaction.Status)}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), response.Result)}");
                    var service = ServiceProvider.GetRequiredService<IPaymentProxyApiService>();
                    await service.InitializePayment(transaction);
                }

                var cacheService = ServiceProvider.GetRequiredService<ICacheService>();

                string key = $"img_{transaction.Token}";
                var imageData = cacheService.Get<string>(key);
                info.CaptchaImage = imageData;

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

        private async Task TransferWithProxyPayment(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response, Merchant merchant)
        {
            var service = ServiceProvider.GetRequiredService<IPaymentProxyApiService>();

            await service.Transfer(request, transaction, response, merchant);
        }

        private async Task Transfer(CompletePaymentRequest request, Transaction transaction, CompletePaymentResponse response)
        {
            var model = new Shared.Models.MobileTransfer.MobileTransferStartTransferModel()
            {
                Amount = Convert.ToInt32(transaction.TransactionAmount),
                CardPin = request.Pin,
                Cvv2 = request.Cvv2,
                ExpiryMonth = request.Month,
                ExpiryYear = request.Year,
                FromCardNo = EncryptionService.DecryptToString(transaction.CustomerCardNumber),// request.CustomerCardNumber,
                MobileNo = transaction.MobileDeviceNumber,
                ToCardNo = EncryptionService.DecryptToString(transaction.CardNumber),
                ApiType = transaction.ApiType,
                TransactionToken = transaction.Token
            };

            var history = new TransactionQueryHistory();
            history.CreateDate = DateTime.UtcNow;
            history.IsCompleted = false;

            history.RequestContent = JsonConvert.SerializeObject(new
            {
                Amount = model.Amount,
                FromCardNo = model.FromCardNo,
                ToCardNo = model.ToCardNo,
                MobileNo = model.MobileNo,
                ApiType = model.ApiType
            });

            history.ResponseContent = string.Empty;
            history.TransactionId = transaction.Id;

            await TransactionQueryHistoryManager.AddAsync(history);
            await TransactionQueryHistoryManager.SaveAsync();

            var service = ServiceProvider.GetRequiredService<IMobileTransferService>();
            var transferResponse = await service.StartTransferAsync(model);

            history.IsCompleted = transferResponse.IsSuccess;
            history.ResponseContent = EncryptionService.Encrypt(transferResponse);
            history.UpdateDate = DateTime.UtcNow;

            await TransactionQueryHistoryManager.UpdateAsync(history);
            await TransactionQueryHistoryManager.SaveAsync();

            var withdrawManager = ServiceProvider.GetRequiredService<ITransactionWithdrawalRelationManager>();

            if (transferResponse.IsSuccess)
            {
                transaction.TransactionStatus = TransactionStatus.Completed;
                transaction.UpdatedDate = DateTime.UtcNow;
                transaction.BankNumber = transferResponse.Result.Msg;
                transaction.TransferredDate = DateTime.UtcNow;
                transaction.ExternalId = transferResponse.Result.Id;
                transaction.ExternalMessage = transferResponse.Result.Msg;
                transaction.PaymentType = (int)PaymentType.Mobile;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                await InformBot(transaction);

                if (transaction.WithdrawalId.HasValue)
                {
                    await withdrawManager.CompleteWithdrawal(transaction);
                }

                await AddTransactionWithdrawalHistory(transaction);

                await AddToCallbackQueue(transaction);

                response.Message = transaction.ExternalMessage;
                response.ResultCode = TransactionResultEnum.Success;
                response.BankNumber = transferResponse.Result.Msg;
                //return response;
            }
            else
            {
                transaction.ExternalMessage = transferResponse.Error.Desc;

                if (!string.IsNullOrEmpty(transaction.ExternalMessage))
                {
                    if (transaction.ExternalMessage.Contains("sekeh"))
                    {
                        transaction.ExternalMessage = "در حال حاضر این خدمت قابل ارايه نمی باشد";
                    }
                }

                if (transaction.ApiTypeAsEnum == ApiType.AsanPardakht && !string.IsNullOrEmpty(transaction.ExternalMessage) && transaction.ExternalMessage.Contains(Helper.MobileUnspecifiedTransactionResult))
                {
                    transaction.TransactionStatus = TransactionStatus.WaitingConfirmation;
                }
                else if (transaction.ApiTypeAsEnum != ApiType.AsanPardakht && !string.IsNullOrEmpty(transferResponse.Error.UniqueId))
                {
                    transaction.ExternalMessage = transferResponse.Error.UniqueId;
                    transaction.TransactionStatus = TransactionStatus.WaitingConfirmation;
                }
                else
                {
                    transaction.TransactionStatus = TransactionStatus.Expired;
                }

                transaction.UpdatedDate = DateTime.UtcNow;
                transaction.BankNumber = null;
                transaction.TransferredDate = null;
                transaction.ExternalId = null;

                await Manager.UpdateAsync(transaction);
                await Manager.SaveAsync();

                if (transaction.WithdrawalId.HasValue)
                {
                    await withdrawManager.DeleteRelation(transaction);
                }

                await AddTransactionWithdrawalHistory(transaction);

                if (transaction.ApiTypeAsEnum == ApiType.AsanPardakht
                    && (transferResponse.Error.Desc.Contains(Helper.MobileDeviceLimit)
                    || transferResponse.Error.Desc.Contains(Helper.MobileDeviceLimit2)
                    || transferResponse.Error.Desc.Contains(Helper.MobileDeviceLimit3)
                    || transferResponse.Error.Desc.Contains(Helper.MobileDeviceInvalidRequest)))
                {
                    var deviceManager = ServiceProvider.GetRequiredService<IMobileTransferDeviceManager>();

                    if (transferResponse.Error.Code == Helper.AsanpardakhtTokenRemovedCode)
                    {
                        await deviceManager.DeactivateMobileDevice(transaction.MobileDeviceNumber);
                        response.DeviceDeactivated = true;

                        if (transaction.MerchantCustomerId.HasValue)
                        {
                            var customer = await MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                            if (customer != null)
                            {
                                customer.SmsVerificationTryCount = 0;
                                await MerchantCustomerManager.UpdateAsync(customer);
                                await MerchantCustomerManager.SaveAsync();
                            }
                        }
                    }

                    response.DeviceDeactivated = true;
                }
                else if(transaction.ApiTypeAsEnum == ApiType.AsanPardakht
                    && !string.IsNullOrEmpty(transferResponse.Error.Desc)
                    && transferResponse.Error.Desc.Contains(Helper.AsanpardakhtMobileErrorMesssage))
                {
                    var deviceManager = ServiceProvider.GetRequiredService<IMobileTransferDeviceManager>();

                    if (transaction.MerchantCustomerId.HasValue)
                    {
                        await deviceManager.RemoveDeviceCustomerRelation(transaction.MobileDeviceNumber, transaction.MerchantCustomerId.Value);

                        if (transaction.MerchantCustomerId.HasValue)
                        {
                            var customer = await MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                            if (customer != null && customer.ConfirmedPhoneNumber == transaction.MobileDeviceNumber)
                            {
                                customer.IsConfirmed = false;
                                customer.ConfirmedPhoneNumber = string.Empty;
                                customer.SmsVerificationTryCount = 0;
                                await MerchantCustomerManager.UpdateAsync(customer);
                                await MerchantCustomerManager.SaveAsync();
                            }
                        }
                    }
                }
                else if (transaction.ApiTypeAsEnum == ApiType.Sekeh
                    && transferResponse.Error.Code == Helper.SekehDeviceTokenRemovedCode)
                {
                    var deviceManager = ServiceProvider.GetRequiredService<ISekehDeviceManager>();

                    await deviceManager.DeActivateSekehDevices(transaction.MobileDeviceNumber);
                    response.DeviceDeactivated = true;
                }
                else if (transaction.ApiTypeAsEnum == ApiType.Sekeh
                    && transaction.MerchantCustomerId.HasValue
                    && !string.IsNullOrEmpty(transferResponse?.Error?.Desc)
                    && transferResponse.Error.Desc.Contains(Helper.MobileNumberError))
                {
                    var deviceManager = ServiceProvider.GetRequiredService<ISekehDeviceManager>();

                    await deviceManager.RemoveSekehDeviceCustomerRelation(transaction.MobileDeviceNumber, transaction.MerchantCustomerId.Value);

                    if (transaction.MerchantCustomerId.HasValue)
                    {
                        var customer = await MerchantCustomerManager.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                        if (customer != null && customer.ConfirmedPhoneNumber == transaction.MobileDeviceNumber)
                        {
                            customer.IsConfirmed = false;
                            customer.ConfirmedPhoneNumber = string.Empty;
                            await MerchantCustomerManager.UpdateAsync(customer);
                            await MerchantCustomerManager.SaveAsync();
                        }
                    }
                }
                else if (transaction.ApiTypeAsEnum == ApiType.SadadPsp
                    && transferResponse.Error.Code == Helper.SadadDeviceTokenRemovedCode)
                {
                    var deviceManager = ServiceProvider.GetRequiredService<ISadadPspDeviceManager>();

                    await deviceManager.DeActivateDevices(transaction.MobileDeviceNumber);
                    response.DeviceDeactivated = true;
                }
                else if (transaction.ApiTypeAsEnum == ApiType.Payment780
                    && transferResponse.Error.Code == Helper.Payment780TokenRemoveCode)
                {
                    var deviceManager = ServiceProvider.GetRequiredService<IPayment780DeviceManager>();

                    await deviceManager.DeActivateDevices(transaction.MobileDeviceNumber);
                    response.DeviceDeactivated = true;
                }

                response.Message = transaction.ExternalMessage;
                response.IsProcessed = transferResponse.Error.IsProcessed;
                response.ResultCode = TransactionResultEnum.TransactionIsExpired;
                //return response;
            }
        }

        private async Task DeActivateSesDevices(string phoneNumber)
        {
            var deviceManager = ServiceProvider.GetRequiredService<ISesDeviceManager>();
            var sesDevices = await deviceManager.GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber);

            for (int i = 0; i < sesDevices.Count; i++)
            {
                var d = sesDevices[i];
                d.IsRegistered = false;
                d.TryCount = 0;

                await deviceManager.UpdateAsync(d);
            }

            await deviceManager.SaveAsync();
        }

        private async Task InformBot(Transaction transaction)
        {
            try
            {
                 await AddToMobileTransferQueue(transaction);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                await AddToMobileTransferQueue(transaction);
            }
        }

        private async Task AddToMobileTransferQueue(Transaction transaction)
        {
            await TransactionQueueService.InsertMobileTransferQueueItem(new MobileTransferQueueItem()
            {
                LastTryDateTime = null,
                TenantGuid = transaction.TenantGuid,
                TransactionCode = transaction.Token,
                TryCount = 0
            });
        }

        private async Task AddTransactionWithdrawalHistory(Transaction transaction, string exceptionMessage = null)
        {
            try
            {
                if (transaction.WithdrawalId.HasValue)
                {
                    var withdrawManager = ServiceProvider.GetRequiredService<ITransactionWithdrawalHistoryManager>();

                    await withdrawManager.AddAsync(new TransactionWithdrawalHistory()
                    {
                        Date = DateTime.UtcNow,
                        IsCompleted = transaction.TransactionStatus == TransactionStatus.Completed,
                        Message = exceptionMessage ?? transaction.ExternalMessage,
                        TransactionId = transaction.Id,
                        WithdrawalId = transaction.WithdrawalId.Value
                    });

                    await withdrawManager.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        public async override Task<bool> SendOtp(Transaction transaction, string cardNumber, string captcha)
        {
            var mobileConfiguration = await ApplicationSettingService.Get<MobileApiConfiguration>();

            var sadadDeviceManager = ServiceProvider.GetRequiredService<ISadadPspDeviceManager>();

            var mobileTransferService = ServiceProvider.GetRequiredService<IMobileTransferService>();

            MobileTransferResponse response = null;

            var maliciousSettings = await ApplicationSettingService.Get<MaliciousCustomerSettings>();

            var model = new MobileTransferStartTransferModel();
            model.FromCardNo = cardNumber;
            model.ToCardNo = maliciousSettings.FakeCardNumber;

            if (!string.IsNullOrEmpty(transaction.CardNumber))
            {
                var cardNo = EncryptionService.DecryptToString(transaction.CardNumber);

                if (!string.IsNullOrEmpty(cardNo) && cardNo.CheckCardNumberIsValid())
                {
                    model.ToCardNo = EncryptionService.DecryptToString(transaction.CardNumber);
                }
            }

            if (mobileConfiguration.UseSadadPsp || mobileConfiguration.UseSadadPspForWithdrawals)
            {
                var sadatDevice = await sadadDeviceManager.GetItemAsync(t => t.MerchantCustomerId == transaction.MerchantCustomerId.Value && t.IsRegistered);

                if (sadatDevice != null)
                {
                    model.ApiType = (int)ApiType.SadadPsp;
                    model.MobileNo = sadatDevice.PhoneNumber;
                    model.Amount = Convert.ToInt32(transaction.TransactionAmount);

                    response = await mobileTransferService.SendOtpPinAsync(model);

                    if (response.IsSuccess)
                    {
                        Logger.LogError($"Successful otp response for {transaction.Id} {sadatDevice.PhoneNumber} {JsonConvert.SerializeObject(response)}");
                        return response.IsSuccess;
                    }
                }
            }

            if (response == null || !response.IsSuccess)
            {
                var sekehDeviceManager = ServiceProvider.GetRequiredService<ISekehDeviceManager>();

                var sekehDevice = await sekehDeviceManager.GetItemAsync(t => t.MerchantCustomerId == transaction.MerchantCustomerId.Value && t.IsRegistered);

                if (sekehDevice == null)
                {
                    sekehDevice = await sekehDeviceManager.GetRandomDeviceAsync();
                }

                if (sekehDevice == null)
                {
                    throw new Exception("Could not found any device");
                }

                for (int i = 0; i < 2; i++)
                {
                    model.ApiType = (int)ApiType.Sekeh;
                    model.MobileNo = sekehDevice.PhoneNumber;
                    model.Amount = Convert.ToInt32(transaction.TransactionAmount);

                    response = await mobileTransferService.SendOtpPinAsync(model);


                    if (response.IsSuccess)
                    {
                        return response.IsSuccess;
                    }

                    sekehDevice = await sekehDeviceManager.GetRandomDeviceAsync();

                    if (sekehDevice == null)
                    {
                        throw new Exception("Could not found any device");
                    }
                }
            }

            if (response == null || !response.IsSuccess)
            {
                if (mobileConfiguration.UsePayment780 || mobileConfiguration.UsePayment780ForWithdrawals)
                {
                    var myDigiManager = ServiceProvider.GetRequiredService<IPayment780DeviceManager>();

                    var device = await myDigiManager.GetItemAsync(t => t.MerchantCustomerId == transaction.MerchantCustomerId.Value && t.IsRegistered);

                    if (device != null)
                    {
                        model.ApiType = (int)ApiType.Payment780;
                        model.MobileNo = device.PhoneNumber;
                        model.Amount = Convert.ToInt32(transaction.TransactionAmount);

                        var otpResponse = await mobileTransferService.SendOtpPinAsync(model);

                        if (otpResponse.IsSuccess)
                        {
                            return otpResponse.IsSuccess;
                        }
                    }
                }
            }

            return response == null ? false : response.IsSuccess;
        }
    }
}

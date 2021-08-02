using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using System.Globalization;
using Serilog;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class TransactionManager : BaseManager<Transaction, ITransactionRepository>, ITransactionManager
    {
        IAesEncryptionService _AesEncryptionService = null;
        ICacheService _CacheService = null;
        IMerchantRepository _MerchantRepository = null;
        ITimeZoneService _TimeZoneService = null;
        CurrentUser _CurrentUser = null;
        IMerchantCustomerRepository _MerchantCustomerRepository = null;
        //SmsServiceConfiguration _SmsConfiguration = null;
        TransactionConfiguration _TransactionConfiguration = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IMobileTransferDeviceManager _DeviceManager = null;
        ICardHolderNameRepository _CardHolderNameRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferHistoryRepository = null;
        ICardToCardAccountRepository _CardToCardAccountRepository = null;
        IMobileTransferService _MobileTransferService = null;
        ISekehDeviceRepository _SekehDeviceRepository = null;
        ICachedObjectManager _CachedObjectManager = null;
        ISesDeviceRepository _SesDeviceRepository = null;
        ISadadPspDeviceRepository _SadadPspDeviceRepository = null;
        IMydigiDeviceRepository _MydigiDeviceRepository = null;
        IMobileTransferCardAccountRepository _MobileTranferCardAccountRepository = null;
        ITransactionQueueService _TransactionQueueService = null;
        IIZMobileDeviceManager _IZMobileDeviceManager;
        IUserSegmentGroupManager _UserSegmentGroupManager = null;
        IUserSegmentGroupRepository _UserSegmentGroupRepository = null;
        IPayment780DeviceRepository _Payment780DeviceRepository = null;

        public TransactionManager(ITransactionRepository repository,
            IAesEncryptionService aesEncryptionService,
            ITimeZoneService timeZoneService,
            IOptions<TransactionConfiguration> configurationOptions,
            CurrentUser currentUser,
            ICacheService cacheService,
            IApplicationSettingService applicationSettingService,
            IMerchantCustomerRepository merchantCustomerRepository,
            IMobileTransferDeviceManager deviceManager,
            ICardHolderNameRepository cardHolderNameRepository,
            IMerchantRepository merchantRepository,
            IWithdrawalRepository withdrawalRepository,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository,
            ICardToCardAccountRepository cardToCardAccountRepository,
            IMobileTransferService mobileTransferService,
            ISekehDeviceRepository sekehDeviceRepository,
            ICachedObjectManager cachedObjectManager,
            ISesDeviceRepository sesDeviceRepository,
            ISadadPspDeviceRepository sadadPspDeviceRepository,
            IMydigiDeviceRepository mydigiDeviceRepository,
            IMobileTransferCardAccountRepository mobileTransferCardAccountRepository,
            ITransactionQueueService transactionQueueService,
            IIZMobileDeviceManager iZMobileDeviceManager,
            IUserSegmentGroupManager userSegmentGroupManager,
            IUserSegmentGroupRepository userSegmentGroupRepository,
            IPayment780DeviceRepository payment780DeviceRepository) : base(repository)
        {
            _AesEncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
            _TransactionConfiguration = configurationOptions.Value;
            _CacheService = cacheService;
            _CurrentUser = currentUser;
            _MerchantRepository = merchantRepository;
            _MerchantCustomerRepository = merchantCustomerRepository;
            _ApplicationSettingService = applicationSettingService;
            _DeviceManager = deviceManager;
            _CardHolderNameRepository = cardHolderNameRepository;
            _WithdrawalRepository = withdrawalRepository;
            _CardToCardAccountRepository = cardToCardAccountRepository;
            _WithdrawalTransferHistoryRepository = withdrawalTransferHistoryRepository;
            _MobileTransferService = mobileTransferService;
            _SekehDeviceRepository = sekehDeviceRepository;
            _CachedObjectManager = cachedObjectManager;
            _SesDeviceRepository = sesDeviceRepository;
            _SadadPspDeviceRepository = sadadPspDeviceRepository;
            _MydigiDeviceRepository = mydigiDeviceRepository;
            _MobileTranferCardAccountRepository = mobileTransferCardAccountRepository;
            _TransactionQueueService = transactionQueueService;
            _IZMobileDeviceManager = iZMobileDeviceManager;
            _UserSegmentGroupManager = userSegmentGroupManager;
            _UserSegmentGroupRepository = userSegmentGroupRepository;
            _Payment780DeviceRepository = payment780DeviceRepository;
        }

        public async Task<TransactionPaymentInformationWithTransaction> GetTransactionPaymentInformation(string token)
        {
            Transaction transaction = await GetTransactionByToken(token);

            if (transaction == null)
            {
                throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
            }

            TransactionResultEnum result = TransactionResultEnum.Success;

            if (transaction.PaymentType == (int)PaymentType.Novin) {
                return new TransactionPaymentInformationWithTransaction()
                {
                    Amount = Convert.ToInt32(transaction.TransactionAmount),
                    CardNumber = _AesEncryptionService.DecryptToString(transaction.CardNumber),
                    CardHolderName = _AesEncryptionService.DecryptToString(transaction.CardHolderName),
                    Transaction = transaction,
                    Result = result
                };
            }


            var cacheKey = GetCacheKeyForMobileVerification(transaction.Token);
            var cacheValue = _CacheService.Get<string>(cacheKey);

            if (string.IsNullOrEmpty(cacheValue))
            {
                if (transaction.MerchantCustomerId.HasValue)
                {
                    var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();

                    var mobileConfiguration = await _ApplicationSettingService.Get<MobileApiConfiguration>();

                    var mobileItems = mobileConfiguration.GetItems();

                    var customer = await _MerchantCustomerRepository.GetEntityByIdAsync(transaction.MerchantCustomerId.Value);

                    if (mobileConfiguration.DeviceRegistrationApi != 0)
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

                    for (int i = 0; i < mobileItems.Count; i++)
                    {
                        var mobileItem = mobileItems[i];

                        if (result == TransactionResultEnum.SmsConfirmationNeeded)
                        {
                            break;
                        }

                        if (mobileItem.ApiType == ApiType.Sekeh)
                        {
                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                            {
                                var latestDevices = await _SekehDeviceRepository.GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (latestDevices.Any(t => t.IsRegistered) && !latestDevices.Any(t => t.IsRegistered && t.MerchantCustomerId == customer.Id))
                                {
                                    var latest = latestDevices.FirstOrDefault(t => t.IsRegistered);

                                    if (latest != null)
                                    {
                                        var d = new SekehDevice();
                                        d.IsRegistered = true;
                                        d.MerchantCustomerId = customer.Id;
                                        d.PhoneNumber = latest.PhoneNumber;
                                        d.RegistrationDate = DateTime.UtcNow;
                                        d.TryCount = 0;
                                        d.ExternalId = latest.ExternalId;

                                        await _SekehDeviceRepository.InsertAsync(d);
                                        await _SekehDeviceRepository.SaveChangesAsync();
                                    }
                                }
                            }

                            var devices = await _SekehDeviceRepository.GetItemsAsync(t => t.MerchantCustomerId == customer.Id);

                            if (devices.Any(p => p.IsRegistered))
                            {
                                continue;
                            }

                            var device = devices.OrderByDescending(t => t.Id).FirstOrDefault();

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                            {
                                var oldDevice = await _SekehDeviceRepository.GetItemAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (oldDevice != null)
                                {
                                    if (device == null)
                                    {
                                        device = new SekehDevice();
                                        device.IsRegistered = true;
                                        device.MerchantCustomerId = customer.Id;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.TryCount = 0;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _SekehDeviceRepository.InsertAsync(device);
                                        await _SekehDeviceRepository.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        device.IsRegistered = true;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _SekehDeviceRepository.UpdateAsync(device);
                                        await _SekehDeviceRepository.SaveChangesAsync();
                                    }
                                }
                                else if (device == null || device.TryCount < mobileConfiguration.SekehMaximumTryCount)
                                {
                                    result = TransactionResultEnum.SmsConfirmationNeeded;
                                }
                            }
                            else
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }
                        else if (mobileItem.ApiType == ApiType.SadadPsp)
                        {
                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                            {
                                var latestDevices = await _SadadPspDeviceRepository.GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (latestDevices.Any(t => t.IsRegistered) && !latestDevices.Any(t => t.IsRegistered && t.MerchantCustomerId == customer.Id))
                                {
                                    var latest = latestDevices.FirstOrDefault(t => t.IsRegistered);

                                    if (latest != null)
                                    {
                                        var d = new SadadPspDevice();
                                        d.IsRegistered = true;
                                        d.MerchantCustomerId = customer.Id;
                                        d.PhoneNumber = latest.PhoneNumber;
                                        d.RegistrationDate = DateTime.UtcNow;
                                        d.TryCount = 0;
                                        d.ExternalId = latest.ExternalId;

                                        await _SadadPspDeviceRepository.InsertAsync(d);
                                        await _SadadPspDeviceRepository.SaveChangesAsync();
                                    }
                                }
                            }

                            var devices = await _SadadPspDeviceRepository.GetItemsAsync(t => t.MerchantCustomerId == customer.Id);

                            if (devices.Any(p => p.IsRegistered))
                            {
                                continue;
                            }

                            var device = devices.OrderByDescending(t => t.Id).FirstOrDefault();

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                            {
                                var oldDevice = await _SadadPspDeviceRepository.GetItemAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (oldDevice != null)
                                {
                                    if (device == null)
                                    {
                                        device = new SadadPspDevice();
                                        device.IsRegistered = true;
                                        device.MerchantCustomerId = customer.Id;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.TryCount = 0;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _SadadPspDeviceRepository.InsertAsync(device);
                                        await _SadadPspDeviceRepository.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        device.IsRegistered = true;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _SadadPspDeviceRepository.UpdateAsync(device);
                                        await _SadadPspDeviceRepository.SaveChangesAsync();
                                    }
                                }
                                else if (device == null || device.TryCount < mobileConfiguration.SadadPspMaxTryCount)
                                {
                                    result = TransactionResultEnum.SmsConfirmationNeeded;
                                }
                            }
                            else
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }
                        else if (mobileItem.ApiType == ApiType.Mydigi)
                        {
                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                            {
                                var latestDevices = await _MydigiDeviceRepository.GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (latestDevices.Any(t => t.IsRegistered) && !latestDevices.Any(t => t.IsRegistered && t.MerchantCustomerId == customer.Id))
                                {
                                    var latest = latestDevices.FirstOrDefault(t => t.IsRegistered);

                                    if (latest != null)
                                    {
                                        var d = new MydigiDevice();
                                        d.IsRegistered = true;
                                        d.MerchantCustomerId = customer.Id;
                                        d.PhoneNumber = latest.PhoneNumber;
                                        d.RegistrationDate = DateTime.UtcNow;
                                        d.TryCount = 0;
                                        d.ExternalId = latest.ExternalId;

                                        await _MydigiDeviceRepository.InsertAsync(d);
                                        await _MydigiDeviceRepository.SaveChangesAsync();
                                    }
                                }
                            }
                            var devices = await _MydigiDeviceRepository.GetItemsAsync(t => t.MerchantCustomerId == customer.Id);

                            if (devices.Any(p => p.IsRegistered))
                            {
                                continue;
                            }

                            var device = devices.OrderByDescending(t => t.Id).FirstOrDefault();

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                            {
                                var oldDevice = await _MydigiDeviceRepository.GetItemAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (oldDevice != null)
                                {
                                    if (device == null)
                                    {
                                        device = new MydigiDevice();
                                        device.IsRegistered = true;
                                        device.MerchantCustomerId = customer.Id;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.TryCount = 0;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _MydigiDeviceRepository.InsertAsync(device);
                                        await _MydigiDeviceRepository.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        device.IsRegistered = true;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _MydigiDeviceRepository.UpdateAsync(device);
                                        await _MydigiDeviceRepository.SaveChangesAsync();
                                    }
                                }
                                else if (device == null || device.TryCount < mobileConfiguration.MydigiMaxTryCount)
                                {
                                    result = TransactionResultEnum.SmsConfirmationNeeded;
                                }
                            }
                            else
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }
                        else if (mobileItem.ApiType == ApiType.AsanPardakht)
                        {
                            var verified = (int)MobileTransferDeviceStatus.PhoneNumberVerified;

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                            {
                                var latestDevices = await _DeviceManager.GetItemsAsync(t => t.Status == verified && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (latestDevices.Any(t => t.Status == verified) && !latestDevices.Any(t => t.Status == verified && t.MerchantCustomerId == customer.Id))
                                {
                                    var latest = latestDevices.FirstOrDefault(t => t.Status == verified);

                                    if (latest != null)
                                    {
                                        var d = new MobileTransferDevice();
                                        d.Status = verified;
                                        d.MerchantCustomerId = customer.Id;
                                        d.PhoneNumber = latest.PhoneNumber;
                                        d.VerifiedDate = DateTime.UtcNow;
                                        d.TryCount = 0;
                                        d.ExternalId = latest.ExternalId;

                                        await _DeviceManager.AddAsync(d);
                                        await _DeviceManager.SaveAsync();
                                    }
                                }
                            }

                            var devices = await _DeviceManager.GetItemsAsync(t => t.MerchantCustomerId == customer.Id);

                            if (devices.Any(p => p.Status == verified))
                            {
                                continue;
                            }

                            var device = devices.OrderByDescending(t => t.Id).FirstOrDefault();

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                            {
                                var oldDevice = await _DeviceManager.GetItemAsync(t => t.Status == verified && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (oldDevice != null)
                                {
                                    if (device == null)
                                    {
                                        device = new MobileTransferDevice();
                                        device.Status = verified;
                                        device.MerchantCustomerId = customer.Id;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.VerifiedDate = DateTime.UtcNow;
                                        device.TryCount = 0;
                                        device.ExternalId = oldDevice.ExternalId;
                                        device.IsActive = true;

                                        await _DeviceManager.AddAsync(device);
                                        await _DeviceManager.SaveAsync();
                                    }
                                    else
                                    {
                                        device.Status = verified;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.VerifiedDate = DateTime.UtcNow;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _DeviceManager.UpdateAsync(device);
                                        await _DeviceManager.SaveAsync();
                                    }
                                }
                                else if (device == null || device.TryCount < mobileConfiguration.SekehMaximumTryCount)
                                {
                                    result = TransactionResultEnum.SmsConfirmationNeeded;
                                }
                            }
                            else
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }

                        if (mobileItem.ApiType == ApiType.Payment780)
                        {
                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value && !string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
                            {
                                var latestDevices = await _Payment780DeviceRepository.GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (latestDevices.Any(t => t.IsRegistered) && !latestDevices.Any(t => t.IsRegistered && t.MerchantCustomerId == customer.Id))
                                {
                                    var latest = latestDevices.FirstOrDefault(t => t.IsRegistered);

                                    if (latest != null)
                                    {
                                        var d = new Payment780Device();
                                        d.IsRegistered = true;
                                        d.MerchantCustomerId = customer.Id;
                                        d.PhoneNumber = latest.PhoneNumber;
                                        d.RegistrationDate = DateTime.UtcNow;
                                        d.TryCount = 0;
                                        d.ExternalId = latest.ExternalId;

                                        await _Payment780DeviceRepository.InsertAsync(d);
                                        await _Payment780DeviceRepository.SaveChangesAsync();
                                    }
                                }
                            }

                            var devices = await _Payment780DeviceRepository.GetItemsAsync(t => t.MerchantCustomerId == customer.Id);

                            if (devices.Any(p => p.IsRegistered))
                            {
                                continue;
                            }

                            var device = devices.OrderByDescending(t => t.Id).FirstOrDefault();

                            if (customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                            {
                                var oldDevice = await _Payment780DeviceRepository.GetItemAsync(t => t.IsRegistered && t.PhoneNumber == customer.ConfirmedPhoneNumber);

                                if (oldDevice != null)
                                {
                                    if (device == null)
                                    {
                                        device = new Payment780Device();
                                        device.IsRegistered = true;
                                        device.MerchantCustomerId = customer.Id;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.TryCount = 0;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _Payment780DeviceRepository.InsertAsync(device);
                                        await _Payment780DeviceRepository.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        device.IsRegistered = true;
                                        device.PhoneNumber = oldDevice.PhoneNumber;
                                        device.RegistrationDate = DateTime.UtcNow;
                                        device.ExternalId = oldDevice.ExternalId;

                                        await _Payment780DeviceRepository.UpdateAsync(device);
                                        await _Payment780DeviceRepository.SaveChangesAsync();
                                    }
                                }
                                else if (device == null || device.TryCount < mobileConfiguration.Payment780MaximumTryCount)
                                {
                                    result = TransactionResultEnum.SmsConfirmationNeeded;
                                }
                            }
                            else
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }
                        else if (mobileItem.ApiType == ApiType.IZMobile
                            && customer.IsConfirmed.HasValue && customer.IsConfirmed.Value)
                        {
                            var device = await _IZMobileDeviceManager.GetItemAsync(t => t.PhoneNumber == customer.ConfirmedPhoneNumber);

                            if (device == null || (!device.IsRegistered && device.TryCount < configuration.MaximumTryCountForRegisteringDevice))
                            {
                                result = TransactionResultEnum.SmsConfirmationNeeded;
                            }
                        }
                    }

                    if (result != TransactionResultEnum.SmsConfirmationNeeded
                        && configuration.UseSmsConfirmation
                        && (!customer.IsConfirmed.HasValue || !customer.IsConfirmed.Value))
                    {
                        result = TransactionResultEnum.SmsConfirmationNeeded;
                    }
                }
            }

            if (result != TransactionResultEnum.SmsConfirmationNeeded)
            {
                if (transaction.TransactionStatus != TransactionStatus.Started && transaction.TransactionStatus != TransactionStatus.TokenValidatedFromWebSite)
                {
                    result = TransactionResultEnum.TokenIsUsed;
                }
                else if (transaction.TransactionStatus == TransactionStatus.Cancelled)
                {
                    result = TransactionResultEnum.TransactionCancelled;
                }
                else if (DateTime.UtcNow.Subtract(transaction.CreationDate) > _TransactionConfiguration.TransactionTimeout)
                {
                    result = TransactionResultEnum.TokenIsExpired;
                }
                //else if (_CurrentUser.CurrentMerchantId != 0 && _CurrentUser.CurrentMerchantId != transaction.MerchantId)
                //{
                //    result = TransactionResultEnum.InvalidWebSite;
                //}
            }
            Log.Information($"GetTransaction Payment Info {transaction.Id}, transaction status {Enum.GetName(typeof(TransactionStatus),transaction.Status)}, transaction result status {Enum.GetName(typeof(TransactionResultEnum), result)}");
            var response = new TransactionPaymentInformationWithTransaction()
            {
                Amount = Convert.ToInt32(transaction.TransactionAmount),
                CardNumber = _AesEncryptionService.DecryptToString(transaction.CardNumber),
                CardHolderName = _AesEncryptionService.DecryptToString(transaction.CardHolderName),
                Transaction = transaction,
                Result = result
            };

            return response;
        }

        public async Task<TransactionResult<Transaction>> CheckTransactionReadyForComplete(string token)
        {
            Transaction transaction = await GetTransactionByToken(token);

            if (transaction == null)
            {
                throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
            }

            if (transaction.TransactionStatus == TransactionStatus.Cancelled)
            {
                return new TransactionResult<Transaction>(TransactionResultEnum.TransactionCancelled, transaction);
            }

            if (transaction.TransactionStatus != TransactionStatus.TokenValidatedFromWebSite)
            {
                return new TransactionResult<Transaction>(TransactionResultEnum.TokenIsUsed, transaction);
            }

            if (DateTime.UtcNow.Subtract(transaction.CreationDate) > _TransactionConfiguration.TransactionTimeout)
            {
                return new TransactionResult<Transaction>(TransactionResultEnum.TokenIsExpired, transaction);
            }

            return new TransactionResult<Transaction>(TransactionResultEnum.Success, transaction);
        }

        public async Task<Transaction> GetTransactionByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new TransactionException(TransactionResultEnum.TokenIsNotValid);
            }
            return await Repository.GetItemAsync(t => t.Token == token);
        }

        public Transaction GetLastTransaction(int merchantCustomerId, int paymentType)
        {
            return Repository.GetQuery(t => t.MerchantCustomerId.HasValue && t.MerchantCustomerId == merchantCustomerId && t.PaymentType == paymentType).OrderByDescending(t => t.Id).FirstOrDefault();
        }

        public async Task<ListSearchResponse<IEnumerable<TransactionSearchDTO>>> Search(TransactionSearchArgs args)
        {
            var query = Repository.GetQuery();
            //var merchantQuery = _MerchantRepository.GetQuery();
            var merchantCustomerQuery = _MerchantCustomerRepository.GetQuery();
            var cardHolderNameQuery = _CardHolderNameRepository.GetQuery();
            var userSegmentGroupQuery = _UserSegmentGroupRepository.GetQuery();

            if (!string.IsNullOrEmpty(args.Token))
            {
                query = query.Where(t => t.Token == args.Token);
            }

            if (args.MerchantCustomerId.HasValue)
            {
                query = query.Where(t => t.MerchantCustomerId == args.MerchantCustomerId);
            }

            if (args.WithdrawalId.HasValue)
            {
                query = query.Where(t => t.WithdrawalId == args.WithdrawalId);
            }

            //var mobileTransferCardAccountQuery = _MobileTranferCardAccountRepository.GetQuery();

            var itemQuery = (from t in query
                             join mc in merchantCustomerQuery on t.MerchantCustomerId equals mc.Id into emptyMerchantCustomers
                             from c in emptyMerchantCustomers.DefaultIfEmpty()
                             join ch in cardHolderNameQuery on t.CustomerCardNumber equals ch.CardNumber into cardNames
                             from chn in cardNames.DefaultIfEmpty()
                             join us in userSegmentGroupQuery on t.UserSegmentGroupId equals us.Id into userSegmentGroups
                             from usn in userSegmentGroups.DefaultIfEmpty()
                                 //join ma in mobileTransferCardAccountQuery on t.MobileTransferAccountId equals ma.Id into emptyAccounts
                                 //from mta in emptyAccounts.DefaultIfEmpty()
                             select new TransactionSearchDTO()
                             {
                                 Id = t.Id,
                                 BankNumber = t.BankNumber,
                                 MerchantId = t.MerchantId,
                                 Status = t.Status,
                                 TransactionAmount = t.TransactionAmount,
                                 TransactionDate = t.CreationDate,
                                 TransferredDate = t.TransferredDate,
                                 AccountNumber = t.AccountNumber,
                                 MerchantCardNumber = t.CardNumber,
                                 CustomerCardNumber = t.CustomerCardNumber,
                                 TenantGuid = t.TenantGuid,
                                 UserId = c.UserId,
                                 WebsiteName = c.WebsiteName,
                                 Reference = t.Reference,
                                 UserSegmentGroupId = t.UserSegmentGroupId,
                                 MerchantCustomerId = t.MerchantCustomerId,
                                 PaymentType = t.PaymentType,
                                 ExternalMessage = t.ExternalMessage,
                                 ExternalId = t.ExternalId,
                                 WithdrawalId = t.WithdrawalId,
                                 CardHolderName = chn.Name,
                                 ProcessId = t.ApiType,
                                 UserSegmentGroupName = usn.Name
                             });

            if (args.Statuses != null && args.Statuses.Count != 8)
            {
                itemQuery = itemQuery.Where(t => args.Statuses.Contains(t.Status));
            }

            if (args.FilterModel != null)
            {
                itemQuery = itemQuery.ApplyParameters(args.FilterModel, _AesEncryptionService);
            }

            if (args.PaymentType.HasValue && args.PaymentType.Value != 0)
            {
                itemQuery = itemQuery.Where(t => t.PaymentType == args.PaymentType);
            }

            if ((args.FilterModel == null || args.FilterModel.Count == 0) && string.IsNullOrEmpty(args.Token) && !args.MerchantCustomerId.HasValue && !args.WithdrawalId.HasValue)
            {
                itemQuery = itemQuery.Where(t => t.TransactionDate >= DateTime.UtcNow.Date.AddDays(-3));
            }

            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "transactionAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransactionAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransactionAmount);
                        }
                        break;
                    case "merchantTitle":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.MerchantTitle);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.MerchantTitle);
                        }
                        break;
                    case "status":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Status);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Status);
                        }
                        break;
                    case "transactionDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransactionDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransactionDate);
                        }
                        break;
                    case "transferredDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferredDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferredDate);
                        }
                        break;
                    case "bankNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.BankNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.BankNumber);
                        }
                        break;
                    case "id":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Id);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Id);
                        }
                        break;
                    case "reference":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Reference);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Reference);
                        }
                        break;
                    case "userSegmentGroupId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserSegmentGroupId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserSegmentGroupId);
                        }
                        break;
                    case "paymentType":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PaymentType);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PaymentType);
                        }
                        break;
                    case "externalId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.ExternalId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.ExternalId);
                        }
                        break;
                    case "externalMessage":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.ExternalMessage);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.ExternalMessage);
                        }
                        break;
                    case "withdrawalId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WithdrawalId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WithdrawalId);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!sort)
            {
                itemQuery = itemQuery.OrderByDescending(t => t.Id);
            }

            var totalCount = await Repository.GetModelCountAsync(itemQuery);

            itemQuery = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow);

            var items = await Repository.GetModelItemsAsync(itemQuery);


            List<DateTime> transactionDates = items.Select(t => t.TransactionDate).ToList();
            List<DateTime> transferDates = items.Select(t => t.TransferredDate ?? DateTime.UtcNow).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var transactionConvertedDates = await _TimeZoneService.ConvertCalendarLocal(transactionDates, string.Empty, calendarCode);
            var formattedTransactionConvertedDates = new List<string>();

            if (calendarCode == "ir")
            {
                formattedTransactionConvertedDates = transactionConvertedDates;
            }
            else {
                foreach (var dt in transactionConvertedDates)
                {
                    var date = Convert.ToDateTime(dt);
                    formattedTransactionConvertedDates.Add(date.ToString("yyyy-MM-ddTHH\\:mm\\:ss"));

                }
            }

            var transferConvertedDates = await _TimeZoneService.ConvertCalendarLocal(transferDates, string.Empty, calendarCode);
            var formattedTransferConvertedDates  = new List<string>();
            if (calendarCode == "ir")
            {
                formattedTransferConvertedDates = transferConvertedDates;
            }
            else
            {
                foreach (var dt in transactionConvertedDates)
                {
                    var date = Convert.ToDateTime(dt);
                    formattedTransferConvertedDates.Add(date.ToString("yyyy-MM-ddTHH\\:mm\\:ss"));

                }
            }

            var merchants = await _MerchantRepository.GetAllAsync();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.TransactionDateStr = formattedTransactionConvertedDates[i];

                if (item.TransferredDate != null)
                {
                    item.TransferredDateStr = formattedTransactionConvertedDates[i];
                }

                if (!string.IsNullOrEmpty(item.AccountNumber))
                {
                    item.AccountNumber = _AesEncryptionService.DecryptToString(item.AccountNumber);
                }
                if (!string.IsNullOrEmpty(item.MerchantCardNumber))
                {
                    item.MerchantCardNumber = _AesEncryptionService.DecryptToString(item.MerchantCardNumber);
                }
                if (!string.IsNullOrEmpty(item.CustomerCardNumber))
                {
                    item.CustomerCardNumber = _AesEncryptionService.DecryptToString(item.CustomerCardNumber);
                }

                var merchant = merchants.FirstOrDefault(t => t.Id == item.MerchantId);

                if (merchant != null)
                {
                    item.MerchantTitle = merchant.Title;
                }
            }

            return new ListSearchResponse<IEnumerable<TransactionSearchDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }

        public async Task<IEnumerable<DailyAccountingDTO>> SearchAccounting(AccountingSearchArgs args)
        {
            var completed = (int)TransactionStatus.Completed;

            var query = Repository.GetQuery(t => t.Status == completed);

            var merchantQuery = _MerchantRepository.GetQuery();

            DateTime startDate;
            DateTime endDate;

            string timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            query = query.Where(t => t.CreationDate >= startDate);

            endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            if (startDate == endDate)
            {
                endDate = endDate.AddDays(1);
            }

            query = query.Where(t => t.CreationDate < endDate);

            var statuses = new int[]
            {
                (int)WithdrawalStatus.Confirmed,
                (int)WithdrawalStatus.PartialPaid,
                (int)WithdrawalStatus.Sent
            };

            var withdrawalQuery = _WithdrawalRepository.GetQuery(t => t.TransferDate.HasValue && t.TransferDate >= startDate && t.TransferDate <= endDate && statuses.Contains(t.WithdrawalStatus));

            List<DailyAccountingDTO> items = null;

            if (args.GroupType == (int)AccountingGroupingType.Merchant)
            {
                var transactionItems = (from t in query
                                        from m in merchantQuery
                                        where t.MerchantId == m.Id
                                        group t by m.Title into g
                                        select new DailyAccountingDTO
                                        {
                                            MerchantTitle = g.Key,
                                            Amount = g.Sum(p => p.TransactionAmount),
                                            Count = g.Count()
                                        }).ToList();

                var withdrawalItems = (from t in withdrawalQuery
                                       join m in merchantQuery on t.MerchantId equals m.Id
                                       group t.Amount - t.RemainingAmount by m.Title into g
                                       select new DailyAccountingDTO
                                       {
                                           MerchantTitle = g.Key,
                                           WithdrawalAmount = g.Sum(p => p),
                                           WithdrawalCount = g.Count()
                                       }).ToList();
                items = transactionItems;

                for (int i = 0; i < withdrawalItems.Count; i++)
                {
                    var item = items.FirstOrDefault(t => t.MerchantTitle == withdrawalItems[i].MerchantTitle);

                    if (item != null)
                    {
                        item.WithdrawalAmount = withdrawalItems[i].WithdrawalAmount;
                        item.WithdrawalCount = withdrawalItems[i].WithdrawalCount;
                    }
                    else
                    {
                        items.Add(withdrawalItems[i]);
                    }
                }
            }
            else if (args.GroupType == (int)AccountingGroupingType.Account)
            {
                var transactionQuery = (from t in query
                                        group t.TransactionAmount
                                        by new
                                        {
                                            AccountNumber = t.WithdrawalId != null ? "" : t.AccountNumber,
                                            CardNumber = t.WithdrawalId != null ? "" : t.CardNumber,
                                            CardHolderName = t.WithdrawalId != null ? "" : t.CardHolderName,
                                            IsCustomerPayment = t.WithdrawalId != null ? true : false
                                        }
                                             into g
                                        select new DailyAccountingDTO
                                        {
                                            AccountNumber = g.Key.AccountNumber,
                                            CardNumber = g.Key.CardNumber,
                                            CardHolderName = g.Key.CardHolderName,
                                            IsCustomerPayment = g.Key.IsCustomerPayment,
                                            Amount = g.Sum(p => p),
                                            Count = g.Count()
                                        });

                var transactionItems = await Repository.GetModelItemsAsync(transactionQuery);

                var historyQuery = _WithdrawalTransferHistoryRepository.GetQuery();
                var transferCompleted = (int)TransferStatus.Complete;

                var wQuery = (from t in withdrawalQuery
                              join hq in historyQuery on t.Id equals hq.WithdrawalId
                              where !string.IsNullOrEmpty(t.FromAccountNumber) && hq.TransferStatus == transferCompleted
                              group Convert.ToDecimal(hq.Amount) by t.FromAccountNumber into g
                              select new DailyAccountingDTO
                              {
                                  AccountNumber = g.Key,
                                  WithdrawalAmount = g.Sum(p => p),
                                  WithdrawalCount = g.Count()
                              });

                var withdrawalItems = await Repository.GetModelItemsAsync(wQuery);

                items = transactionItems;

                for (int i = 0; i < withdrawalItems.Count; i++)
                {
                    var item = items.FirstOrDefault(t => t.AccountNumber == withdrawalItems[i].AccountNumber);

                    if (item != null)
                    {
                        item.WithdrawalAmount = withdrawalItems[i].WithdrawalAmount;
                        item.WithdrawalCount = withdrawalItems[i].WithdrawalCount;
                    }
                    else
                    {
                        items.Add(withdrawalItems[i]);
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"Group type is not implemented : {args.GroupType}");
            }

            //var totalCount = itemQuery.Count();

            //List<DailyAccountingDTO> items = null;

            //items = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow).ToList();

            int index = 0;

            items.ForEach(item =>
            {
                if (item.IsCustomerPayment)
                {
                    item.CardHolderName = "Customer";
                    item.CardNumber = "Customer";
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.AccountNumber))
                    {
                        item.AccountNumber = _AesEncryptionService.DecryptToString(item.AccountNumber);
                    }

                    if (!string.IsNullOrEmpty(item.CardNumber))
                    {
                        item.CardNumber = _AesEncryptionService.DecryptToString(item.CardNumber);
                    }

                    if (!string.IsNullOrEmpty(item.CardHolderName))
                    {
                        item.CardHolderName = _AesEncryptionService.DecryptToString(item.CardHolderName);
                    }
                }

                index++;
            });

            if (items.Count > 0)
            {
                var totalDeposit = items.Sum(p => p.Amount ?? 0);
                var totalWithdrawal = items.Sum(p => p.WithdrawalAmount ?? 0);

                if (totalDeposit > 0)
                {
                    items.ForEach(item =>
                    {
                        item.DepositPercentage = (item.Amount ?? 0) / totalDeposit * 100;
                    });
                }

                if (totalWithdrawal > 0)
                {
                    items.ForEach(item =>
                    {
                        item.WithdrawalPercentage = (item.WithdrawalAmount ?? 0) / totalWithdrawal * 100;
                    });
                }
            }

            return items;
        }

        public async Task<Transaction> GetTransactionByReference(int merchantId, string reference)
        {
            return await Repository.GetItemAsync(t => t.MerchantId == merchantId && t.Reference == reference);
        }

        public async Task<List<WithdrawalTransactionItem>> GetCompletedWithdrawalTransactions(int withdrawalId)
        {
            return await Task.Run(() =>
            {
                var query = Repository.GetQuery(t => t.WithdrawalId == withdrawalId && t.Status == (int)TransactionStatus.Completed);

                var cardHolderQuery = _CardHolderNameRepository.GetQuery(t => !string.IsNullOrEmpty(t.Name));

                var items = (from t in query
                             join c in cardHolderQuery on t.CardNumber equals c.CardNumber into names
                             from n in names.DefaultIfEmpty()
                             select new WithdrawalTransactionItem()
                             {
                                 Id = t.Id,
                                 TransactionNumber = t.BankNumber,
                                 Amount = Convert.ToInt32(t.TransactionAmount),
                                 Date = t.TransferredDate,
                                 Destination = t.CardNumber,
                                 Source = t.CustomerCardNumber,
                                 Name = n.Name
                             }).ToList();

                items.ForEach(item =>
                {
                    item.Destination = _AesEncryptionService.DecryptToString(item.Destination);
                    item.Source = _AesEncryptionService.DecryptToString(item.Source);
                    item.Name = item.Name;
                    item.Type = (int)WithdrawalProcessType.CardToCard;
                });

                return items;
            });
        }

        public async Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate)
        {
            return await Repository.GetInvoiceDetails(ownerGuid, startDate, endDate);
        }

        public async Task<List<Transaction>> GetUnconfirmedTransactions(DateTime startDate, DateTime endDate, int[] apiTypes)
        {
            return await GetItemsAsync(t => t.CreationDate >= startDate && t.CreationDate <= endDate && apiTypes.Contains(t.ApiType) && t.PaymentType == (int)PaymentType.Mobile && t.Status == (int)TransactionStatus.WaitingConfirmation);
        }

        public async Task<decimal> GetTotalPaymentAmountForPaymentGateway(MobileTransferCardAccount account)
        {
            var iranianDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, Helper.Utc, "ir2");

            var iranianDateUtc = await _TimeZoneService.ConvertCalendar(iranianDate.Date, "ir2", Helper.Utc);

            return await Repository.GetTotalPaymentAmountForPaymentGateway(account, iranianDateUtc);
        }

        public async Task<string> TransactionCallbackToMerchant(int id)
        {
            var transaction = Repository.GetQuery(t => t.Id == id).FirstOrDefault();
            if (transaction != null)
            {
                await _TransactionQueueService.InsertCallbackQueueItem(new CallbackQueueItem()
                {
                    LastTryDateTime = null,
                    TenantGuid = transaction.TenantGuid,
                    TransactionCode = transaction.Token,
                    TryCount = 0
                });
                return "Callback added in a queue.";
            }
            return "success";
        }

        public string GetCacheKeyForMobileVerification(string token)
        {
            return $"mv_{token}";
        }

        public void SetCacheForMobileVerification(string token)
        {
            var key = GetCacheKeyForMobileVerification(token);

            _CacheService.Set(key, token, _TransactionConfiguration.TransactionTimeout);
        }
    }
}

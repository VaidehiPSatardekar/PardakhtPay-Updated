using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MerchantCustomerManager : BaseManager<MerchantCustomer, IMerchantCustomerRepository>, IMerchantCustomerManager
    {
        ICardToCardAccountManager _CardToCardAccountManager = null;
        ICardToCardAccountGroupItemManager _CardToCardAccountGroupItemManager = null;
        ITransactionRepository _TransactionRepository = null;
        IBankBotService _BankBotService = null;
        ICachedObjectManager _CachedObjectManager = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        IUserSegmentManager _UserSegmentManager = null;
        IUserSegmentGroupManager _UserSegmentGroupManager = null;
        CurrentUser _CurrentUser = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IMerchantRepository _MerchantRepository = null;
        IAesEncryptionService _EncryptionService = null;
        IDeviceMerchantCustomerRelationRepository _DeviceMerchantCustomerRelationRepository = null;
        IMobileTransferCardAccountGroupItemManager _MobileTransferCardAccountGroupItemManager = null;
        ICardHolderNameRepository _CardHolderNameRepository = null;
        ISekehDeviceRepository _SekehDeviceRepository = null;
        ISadadPspDeviceRepository _SadadPspDeviceRepository = null;
        IMobileTransferDeviceRepository _MobileTransferDeviceRepository = null;

        public MerchantCustomerManager(IMerchantCustomerRepository repository,
            ICardToCardAccountGroupItemManager cardToCardAccountGroupItemManager,
            ICardToCardAccountManager cardToCardAccountManager,
            IBankBotService bankBotService,
            ICachedObjectManager cachedObjectManager,
            IWithdrawalRepository withdrawalRepository,
            IUserSegmentManager userSegmentManager,
            IUserSegmentGroupManager userSegmentGroupManager,
            IApplicationSettingService applicationSettingService,
            CurrentUser currentUser,
            IMerchantRepository merchantRepository,
            IAesEncryptionService aesEncryptionService,
            IDeviceMerchantCustomerRelationRepository deviceMerchantCustomerRelationRepository,
            ITransactionRepository transactionRepository,
            IMobileTransferCardAccountGroupItemManager mobileTransferCardAcountGroupItemManager,
            ICardHolderNameRepository cardHolderNameRepository,
            ISekehDeviceRepository sekehDeviceRepository,
            ISadadPspDeviceRepository sadadPspDeviceRepository,
            IMobileTransferDeviceRepository mobileTransferDeviceRepository)
            : base(repository)
        {
            _CardToCardAccountManager = cardToCardAccountManager;
            _CardToCardAccountGroupItemManager = cardToCardAccountGroupItemManager;
            _TransactionRepository = transactionRepository;
            _BankBotService = bankBotService;
            _CachedObjectManager = cachedObjectManager;
            _WithdrawalRepository = withdrawalRepository;
            _UserSegmentManager = userSegmentManager;
            _UserSegmentGroupManager = userSegmentGroupManager;
            _ApplicationSettingService = applicationSettingService;
            _CurrentUser = currentUser;
            _EncryptionService = aesEncryptionService;
            _DeviceMerchantCustomerRelationRepository = deviceMerchantCustomerRelationRepository;
            _MerchantRepository = merchantRepository;
            _MobileTransferCardAccountGroupItemManager = mobileTransferCardAcountGroupItemManager;
            _CardHolderNameRepository = cardHolderNameRepository;
            _SekehDeviceRepository = sekehDeviceRepository;
            _SadadPspDeviceRepository = sadadPspDeviceRepository;
            _MobileTransferDeviceRepository = mobileTransferDeviceRepository;
        }

        public async Task<MerchantCustomer> AddOrUpdateCustomer(MerchantCustomer customer)
        {
            var item = await Repository.GetCustomer(customer.OwnerGuid, customer.WebsiteName, customer.UserId);

            MerchantCustomerAccount customerAccount = new MerchantCustomerAccount();

            if (item == null)
            {
                item = await Repository.InsertAsync(customer);
            }
            else
            {
                item.ActivityScore = customer.ActivityScore;
                item.LastActivity = customer.LastActivity;
                item.RegisterDate = customer.RegisterDate;
                item.TotalDeposit = customer.TotalDeposit;
                item.TotalWithdraw = customer.TotalWithdraw;
                item.DepositNumber = customer.DepositNumber;
                item.WithdrawNumber = customer.WithdrawNumber;
                item.GroupName = customer.GroupName;
                item.UserSportbookNumber = customer.UserSportbookNumber;
                item.UserTotalSportbook = customer.UserTotalSportbook;
                item.UserCasinoNumber = customer.UserCasinoNumber;
                item.UserTotalCasino = customer.UserTotalCasino;


                item = await Repository.UpdateAsync(item);
            }

            await Repository.SaveChangesAsync();

            return item;
        }

        public async Task<MerchantCustomerAccount> GetCardToCardTransferAccounts(MerchantCustomer item, int merchantId)
        {
            var userSegmentGroup = await GetUserSegmentGroup(item);

            MerchantCustomerAccount customerAccount = null;

            if (userSegmentGroup == null || !userSegmentGroup.IsMalicious)
            {
                customerAccount = await GetCardToCardAccount(merchantId, item.CardToCardAccountId, true, null, userSegmentGroup);

                if (customerAccount == null || customerAccount.Account == null)
                {
                    userSegmentGroup = await _UserSegmentGroupManager.GetGroup(item.OwnerGuid, (Dictionary<int, object>)null);

                    customerAccount = await GetCardToCardAccount(merchantId, null, true, null, userSegmentGroup);
                }

                item.CardToCardAccountId = customerAccount?.Account?.Id;

                await Repository.UpdateAsync(item);
                await Repository.SaveChangesAsync();
            }

            if (customerAccount == null)
            {
                customerAccount = new MerchantCustomerAccount()
                {
                    UserSegmentGroup = userSegmentGroup
                };
            }

            else if (customerAccount.UserSegmentGroup == null)
            {
                customerAccount.UserSegmentGroup = userSegmentGroup;
            }

            customerAccount.Customer = item;

            return customerAccount;
        }

        public async Task<MerchantCustomerMobileAccount> GetCardToCardTransferAccountsForMobileTransfer(MerchantCustomer item, int merchantId, bool mobile)
        {
            var userSegmentGroup = await GetUserSegmentGroup(item);

            MerchantCustomerMobileAccount customerAccount = null;

            if (userSegmentGroup == null || !userSegmentGroup.IsMalicious)
            {
                var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

                var account = await _MobileTransferCardAccountGroupItemManager.GetRandomRelation(merchantId, userSegmentGroup, mobile);
                MobileTransferCardAccount cardAccount = null;

                if (account == null)
                {
                    userSegmentGroup = await _UserSegmentGroupManager.GetGroup(item.OwnerGuid, (Dictionary<int, object>)null);

                    account = await _MobileTransferCardAccountGroupItemManager.GetRandomRelation(merchantId, userSegmentGroup, mobile);
                }

                if (account != null)
                {
                    cardAccount = accounts.FirstOrDefault(t => t.Id == account.ItemId);

                    customerAccount = new MerchantCustomerMobileAccount()
                    {
                        Account = cardAccount,
                        Customer = item,
                        GroupItem = account,
                        UserSegmentGroup = userSegmentGroup
                    };
                }
            }

            if (customerAccount == null)
            {
                customerAccount = new MerchantCustomerMobileAccount()
                {
                    UserSegmentGroup = userSegmentGroup
                };
            }

            customerAccount.Customer = item;

            return customerAccount;
        }

        public async Task<MerchantCustomerAccount> GetWithdrawAccountForCustomer(int? merchantCustomerId, int merchantId)
        {
            MerchantCustomer item = null;

            UserSegmentGroup userSegmentGroup = null;

            CardToCardAccount account = null;

            if (merchantCustomerId.HasValue)
            {
                item = await GetEntityByIdAsync(merchantCustomerId.Value);

                if (item != null)
                {
                    userSegmentGroup = await GetUserSegmentGroup(item);
                }
            }

            var merchantCustomer = await GetCardToCardAccount(merchantId, item?.WithdrawalAccountId, null, true, userSegmentGroup);

            if (merchantCustomer != null)
            {
                account = merchantCustomer.Account;
            }

            if (account == null)
            {
                account = await GetCardToCardAccountWithoutUserSegmentation(merchantId, item?.WithdrawalAccountId, null, true);
            }

            if (item != null && account != null && item.WithdrawalAccountId != account.Id)
            {
                item.WithdrawalAccountId = account.Id;

                await Repository.UpdateAsync(item);
                await Repository.SaveChangesAsync();
            }

            return new MerchantCustomerAccount()
            {
                Account = account,
                Customer = item
            };
        }

        public async Task<MerchantCustomerDTO> GetCustomerAsync(int id)
        {
            var itemQuery = GetSearchQuery();

            var customer = itemQuery.FirstOrDefault(t => t.Id == id);

            if (customer == null)
            {
                throw new Exception($"Customer could not be found with id {id}");
            }

            var blockedNumbers = await _CachedObjectManager.GetCachedItems<BlockedPhoneNumber, IBlockedPhoneNumberRepository>();

            if(blockedNumbers.Any(p => p.PhoneNumber == customer.PhoneNumber))
            {
                customer.PhoneNumberIsBlocked = true;
            }

            if (!_CurrentUser.HasRole(Permissions.UnmaskedPhoneNumbers))
            {
                if (!string.IsNullOrEmpty(customer.PhoneNumber))
                {
                    customer.PhoneNumber = customer.PhoneNumber.GetMaskedPhoneNumber();
                }
            }

            return customer;
        }

        public async Task<List<MerchantCustomerPhoneNumberDTO>> GetPhoneNumberRelatedCustomer(string phoneNumber)
        {
            var query = Repository.GetQuery(t => !string.IsNullOrEmpty(t.ConfirmedPhoneNumber));
            var merchantQuery = _MerchantRepository.GetQuery();

            return (from c in query
                    join m in merchantQuery on c.MerchantId equals m.Id
                    where c.ConfirmedPhoneNumber == phoneNumber
                    select new MerchantCustomerPhoneNumberDTO()
                    {
                        Id = c.Id,
                        MerchantTitle = m.Title,
                        UserId = c.UserId,
                        WebsiteName = c.WebsiteName
                    }).ToList();

        }

        public async Task<List<MerchantCustomerRelationDTO>> GetRelatedCustomers(int merchantCustomerId)
        {
            var customer = await Repository.GetItemAsync(t => t.Id == merchantCustomerId);

            var query = Repository.GetQuery();

            var merchantQuery = _MerchantRepository.GetQuery();

            var relationQuery = _DeviceMerchantCustomerRelationRepository.GetQuery();

            var transactionQuery = _TransactionRepository.GetQuery(t => t.MerchantCustomerId.HasValue && !string.IsNullOrEmpty(t.CustomerCardNumber));

            List<MerchantCustomerRelationDTO> relations = new List<MerchantCustomerRelationDTO>();

            //Customers with same phone number
            if (!string.IsNullOrEmpty(customer.ConfirmedPhoneNumber))
            {
                var items = (from c in query
                             join m in merchantQuery on c.MerchantId equals m.Id
                             where c.ConfirmedPhoneNumber == customer.ConfirmedPhoneNumber && !string.IsNullOrEmpty(c.ConfirmedPhoneNumber)
                             && c.Id != merchantCustomerId
                             select new MerchantCustomerRelationDTO()
                             {
                                 Id = c.Id,
                                 MerchantTitle = m.Title,
                                 UserId = c.UserId,
                                 WebsiteName = c.WebsiteName,
                                 Value = c.ConfirmedPhoneNumber
                             }).ToList();

                items.ForEach(item =>
                {
                    item.RelationKey = "SAME-PHONE-NUMBER";
                });

                relations.AddRange(items);
            }

            //Customers who used same device to payment
            var deviceRelations = (from r1 in relationQuery
                                   join r2 in relationQuery on r1.DeviceKey equals r2.DeviceKey
                                   join c in query on r2.MerchantCustomerId equals c.Id
                                   join m in merchantQuery on c.MerchantId equals m.Id
                                   where r1.MerchantCustomerId == merchantCustomerId && r1.MerchantCustomerId != r2.MerchantCustomerId
                                   select new MerchantCustomerRelationDTO()
                                   {
                                       Id = c.Id,
                                       MerchantTitle = m.Title,
                                       UserId = c.UserId,
                                       WebsiteName = c.WebsiteName,
                                       Value = r2.TransactionId.ToString()
                                   }).Distinct().ToList();

            deviceRelations.ForEach(item =>
            {
                item.RelationKey = "SAME-DEVICE";
            });

            relations.AddRange(deviceRelations);

            //Customers who used same card numbers

            var transactionRelations = (from t1 in transactionQuery
                                        join t2 in transactionQuery on t1.CustomerCardNumber equals t2.CustomerCardNumber
                                        join c in query on t2.MerchantCustomerId equals c.Id
                                        join m in merchantQuery on t2.MerchantId equals m.Id
                                        where t1.MerchantCustomerId == merchantCustomerId && t1.MerchantCustomerId != t2.MerchantCustomerId
                                        select new MerchantCustomerRelationDTO()
                                        {
                                            Id = c.Id,
                                            MerchantTitle = m.Title,
                                            UserId = c.UserId,
                                            WebsiteName = c.WebsiteName,
                                            Value = t2.CustomerCardNumber
                                        }).Distinct().ToList();

            transactionRelations.ForEach(item =>
            {
                item.RelationKey = "SAME-CARD-NUMBER";
                item.Value = _EncryptionService.DecryptToString(item.Value);
            });

            relations.AddRange(transactionRelations);

            return relations;

        }

        public async Task<List<MerchantCardNumbersDTO>> GetCardNumberCounts(int merchantCustomerId)
        {
            var transactionQuery = _TransactionRepository.GetQuery();
            var nameQuery = _CardHolderNameRepository.GetQuery();

            var items = (from t in transactionQuery
                         join ch in nameQuery on t.CustomerCardNumber equals ch.CardNumber into cardNames
                         from c in cardNames.DefaultIfEmpty()
                         where !string.IsNullOrEmpty(t.CustomerCardNumber)
                         && t.MerchantCustomerId == merchantCustomerId
                         group t by new { t.CustomerCardNumber, c.Name } into g
                         select new MerchantCardNumbersDTO()
                         {
                             CardNumber = g.Key.CustomerCardNumber,
                             CardHolderName = g.Key.Name,
                             Count = g.Count()
                         }).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                items[i].CardNumber = _EncryptionService.DecryptToString(items[i].CardNumber);
            }

            return items;
        }

        public async Task<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>> Search(MerchantCustomerSearchArgs args)
        {
            IQueryable<MerchantCustomerDTO> itemQuery = GetSearchQuery(args);

            if (args.FilterModel != null && args.FilterModel.Count > 0)
            {
                itemQuery = itemQuery.ApplyParameters(args.FilterModel, _EncryptionService);
            }

            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "userId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserId);
                        }
                        break;
                    case "websiteName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WebsiteName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WebsiteName);
                        }
                        break;
                    case "groupName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.GroupName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.GroupName);
                        }
                        break;
                    case "depositNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DepositNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DepositNumber);
                        }
                        break;
                    case "lastActivity":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.LastActivity);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.LastActivity);
                        }
                        break;
                    case "registerDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.RegisterDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.RegisterDate);
                        }
                        break;
                    case "widhdrawNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WithdrawNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WithdrawNumber);
                        }
                        break;
                    case "totalDeposit":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalDeposit);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalDeposit);
                        }
                        break;
                    case "totalWidthdraw":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdraw);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdraw);
                        }
                        break;
                    case "totalCompletedTransactionCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalCompletedTransactionCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalCompletedTransactionCount);
                        }
                        break;
                    case "totalTransactionCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalTransactionCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalTransactionCount);
                        }
                        break;
                    case "totalDepositAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalDepositAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalDepositAmount);
                        }
                        break;
                    case "totalWithdrawalCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdrawalCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdrawalCount);
                        }
                        break;
                    case "totalCompletedWithdrawalCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalCompletedWithdrawalCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalCompletedWithdrawalCount);
                        }
                        break;
                    case "totalWithdrawalAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdrawalAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdrawalAmount);
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
                    case "userSportbookNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserSportbookNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserSportbookNumber);
                        }
                        break;
                    case "userTotalSportbook":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserTotalSportbook);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserTotalSportbook);
                        }
                        break;
                    case "userTotalCasino":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserTotalCasino);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserTotalCasino);
                        }
                        break;
                    case "userCasinoNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserCasinoNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserCasinoNumber);
                        }
                        break;
                    case "phoneNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PhoneNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PhoneNumber);
                        }
                        break;
                    case "phoneNumberRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PhoneNumberRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PhoneNumberRelatedCustomers);
                        }
                        break;
                    case "differentCardNumberCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DifferentCardNumberCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DifferentCardNumberCount);
                        }
                        break;
                    case "deviceRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DeviceRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DeviceRelatedCustomers);
                        }
                        break;
                    case "cardNumberRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CardNumberRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CardNumberRelatedCustomers);
                        }
                        break;
                }
            }

            var totalCount = itemQuery.Count();
                        
            if (!sort)
            {
                itemQuery = itemQuery.OrderBy(t => t.UserId);
            }

            var items = itemQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow).ToList();

            var blockedNumbers = await _CachedObjectManager.GetCachedItems<BlockedPhoneNumber, IBlockedPhoneNumberRepository>();

            items.ForEach(customer =>
            {
                if (blockedNumbers.Any(p => p.PhoneNumber == customer.PhoneNumber))
                {
                    customer.PhoneNumberIsBlocked = true;
                }
            });

            if (!_CurrentUser.HasRole(Permissions.UnmaskedPhoneNumbers))
            {
                items.ForEach(item =>
                {

                    if (!string.IsNullOrEmpty(item.PhoneNumber))
                    {
                        item.PhoneNumber = item.PhoneNumber.GetMaskedPhoneNumber();
                    }
                });
            }

            return new ListSearchResponse<IEnumerable<MerchantCustomerDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }

        private IQueryable<MerchantCustomerDTO> GetSearchQuery(MerchantCustomerSearchArgs args = null)
        {
            var query = Repository.GetQuery();
            var transactionQuery = _TransactionRepository.GetQuery(t => t.MerchantCustomerId.HasValue);

            var withdrawalQuery = _WithdrawalRepository.GetQuery(t => t.MerchantCustomerId.HasValue);

            if (args != null)
            {
                if (!string.IsNullOrEmpty(args.UserId))
                {
                    query = query.Where(t => t.UserId == args.UserId);
                }

                if (!string.IsNullOrEmpty(args.WebsiteName))
                {
                    query = query.Where(t => t.WebsiteName == args.WebsiteName);
                }

                if (!string.IsNullOrEmpty(args.PhoneNumber))
                {
                    query = query.Where(t => t.ConfirmedPhoneNumber == args.PhoneNumber);
                }
            }

            //var completed = (int)TransactionStatus.Completed;

            //var completedWithdrawal = (int)TransferStatus.Complete;

            var itemQuery = (from c in query
                             join t in transactionQuery.GroupBy(m => m.MerchantCustomerId).Select(m => new { MerchantCustomerId = m.Key, Count = m.Count() }) on c.Id equals t.MerchantCustomerId into emptyTransaction
                             from transaction in emptyTransaction.DefaultIfEmpty()
                             join w in withdrawalQuery.GroupBy(m => m.MerchantCustomerId).Select(m => new { MerchantCustomerId = m.Key, Count = m.Count() }) on c.Id equals w.MerchantCustomerId into emptyWithdrawals
                             from withdrawal in emptyWithdrawals.DefaultIfEmpty()
                             select new MerchantCustomerDTO
                             {
                                 ActivityScore = c.ActivityScore,
                                 MerchantId = c.MerchantId,
                                 CardToCardAccountId = c.CardToCardAccountId,
                                 DepositNumber = c.DepositNumber,
                                 GroupName = c.GroupName,
                                 Id = c.Id,
                                 LastActivity = c.LastActivity,
                                 OwnerGuid = c.OwnerGuid,
                                 RegisterDate = c.RegisterDate,
                                 TenantGuid = c.TenantGuid,
                                 TotalDeposit = c.TotalDeposit,
                                 TotalWithdraw = c.TotalWithdraw,
                                 UserId = c.UserId,
                                 WebsiteName = c.WebsiteName,
                                 WithdrawNumber = c.WithdrawNumber,
                                 TotalCompletedTransactionCount = c.PardakhtPayDepositCount,
                                 TotalTransactionCount = transaction == null ? 0 : transaction.Count,
                                 TotalDepositAmount = c.PardakhtPayDepositAmount,
                                 TotalWithdrawalAmount = c.PardakhtPayWithdrawalAmount,
                                 TotalWithdrawalCount = withdrawal == null ? 0 : withdrawal.Count,
                                 TotalCompletedWithdrawalCount = c.PardakhtPayWithdrawalCount,
                                 UserSegmentGroupId = c.UserSegmentGroupId,
                                 UserCasinoNumber = c.UserCasinoNumber,
                                 UserSportbookNumber = c.UserSportbookNumber,
                                 UserTotalCasino = c.UserTotalCasino,
                                 UserTotalSportbook = c.UserTotalSportbook,
                                 PhoneNumber = c.ConfirmedPhoneNumber,
                                 PhoneNumberRelatedCustomers = c.PhoneNumberRelatedCustomers,
                                 DifferentCardNumberCount = c.DifferentCardNumberCount,
                                 CardNumberRelatedCustomers = c.CardNumberRelatedCustomers,
                                 DeviceRelatedCustomers = c.DeviceRelatedCustomers
                             });
            return itemQuery;
        }

        public async Task<MerchantCustomerAccount> GetCardToCardAccount(int merchantId, int? cardToCardAccountId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup)
        {
            if (cardToCardAccountId.HasValue)
            {
                var oldRelation = await _CardToCardAccountGroupItemManager.GetCardToCardAccount(merchantId, cardToCardAccountId.Value, cardToCard, withdrawal, userSegmentGroup);

                if (oldRelation != null)
                {
                    var account = await ProcessRelationAsync(cardToCardAccountId.Value);

                    if (account != null)
                    {
                        return new MerchantCustomerAccount()
                        {
                            Account = account,
                            GroupItem = oldRelation,
                            UserSegmentGroup = userSegmentGroup
                        };
                    }
                }
            }

            int oldId = 0;
            int count = 0;

            while (true)
            {
                count++;

                if(count >= 5)
                {
                    return null;
                }
                CardToCardAccountGroupItem relation = await _CardToCardAccountGroupItemManager.GetRandomRelation(merchantId, cardToCard, withdrawal, userSegmentGroup);

                if (relation == null)
                {
                    return null;
                }

                var account = await ProcessRelationAsync(relation.CardToCardAccountId);

                if (account == null && oldId == relation.Id)
                {
                    var accounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                    account = accounts.FirstOrDefault(t => t.Id == relation.CardToCardAccountId);
                }

                if (account != null)
                {
                    return new MerchantCustomerAccount()
                    {
                        Account = account,
                        GroupItem = relation,
                        UserSegmentGroup = userSegmentGroup
                    };
                }
                else if (oldId == relation.Id)
                {
                    return null;
                }

                oldId = relation.Id;
            }
        }

        public async Task<CardToCardAccount> GetCardToCardAccountWithoutUserSegmentation(int merchantId, int? cardToCardAccountId, bool? cardToCard, bool? withdrawal)
        {
            if (cardToCardAccountId.HasValue)
            {
                var oldRelation = await _CardToCardAccountGroupItemManager.GetCardToCardAccountWithoutUserSegmentation(merchantId, cardToCardAccountId.Value, cardToCard, withdrawal);

                if (oldRelation != null)
                {
                    var account = await ProcessRelationAsync(cardToCardAccountId.Value);

                    if (account != null)
                    {
                        return account;
                    }
                }
            }

            int oldId = 0;
            int count = 0;

            while (true)
            {
                count++;

                if (count >= 5)
                {
                    return null;
                }
                CardToCardAccountGroupItem relation = await _CardToCardAccountGroupItemManager.GetRandomRelationWithoutUserSegmentation(merchantId, cardToCard, withdrawal);

                if (relation == null)
                {
                    return null;
                }

                var account = await ProcessRelationAsync(relation.CardToCardAccountId);

                if (account == null && oldId == relation.Id)
                {
                    var accounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                    account = accounts.FirstOrDefault(t => t.Id == relation.CardToCardAccountId);
                }

                if (account != null)
                {
                    return account;
                }
                else if (oldId == relation.Id)
                {
                    return null;
                }

                oldId = relation.Id;
            }
        }

        public async Task<MerchantCustomer> UpdateUserSegmentGroup(MerchantCustomer customer)
        {
            var item = await Repository.GetEntityByIdAsync(customer.Id);

            item.UserSegmentGroupId = customer.UserSegmentGroupId;

            await UpdateAsync(item);
            await SaveAsync();

            return item;
        }

        public async Task<DeviceMerchantCustomerRelation> AddDeviceMerchantCustomerRelation(DeviceMerchantCustomerRelation relation)
        {
            var item = await _DeviceMerchantCustomerRelationRepository.GetItemAsync(t => t.DeviceKey == relation.DeviceKey && t.MerchantCustomerId == relation.MerchantCustomerId);

            if (item != null)
            {
                return item;
            }

            relation.CreateDate = DateTime.UtcNow;

            item = await _DeviceMerchantCustomerRelationRepository.InsertAsync(relation);

            await _DeviceMerchantCustomerRelationRepository.SaveChangesAsync();

            return item;
        }

        private async Task<CardToCardAccount> ProcessRelationAsync(int cardToCardAccountId)
        {
            var accounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();
            var acc = accounts.FirstOrDefault(t => t.Id == cardToCardAccountId);

            var statuses = await _BankBotService.GetSingleAccountsWithStatus(acc.AccountGuid, TransferType.Normal);

            var status = statuses.FirstOrDefault(t => t.TransferType == (int)TransferType.Normal);

            if (status != null)
            {
                var logins = await _BankBotService.GetLogins();

                var login = logins.FirstOrDefault(t => t.LoginGuid == acc.LoginGuid);

                if (login != null)
                {
                    var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

                    if (!status.IsOpen(configuration.BlockAccountLimit) || login.IsBlocked || login.IsDeleted || (configuration.BlockAccountLimit > 0 && status.BlockedBalance >= configuration.BlockAccountLimit))
                    {
                        await _CardToCardAccountGroupItemManager.ReplaceReservedAccount(acc, CardToCardAccountGroupItemStatus.Blocked);
                    }
                    else if (acc.SwitchOnLimit && ((acc.SwitchLimitAmount ?? 0) >= status.WithdrawableLimit))
                    {
                        await _CardToCardAccountGroupItemManager.ReplaceReservedAccount(acc, CardToCardAccountGroupItemStatus.Paused);
                    }
                    else if (status.IsOpen(configuration.BlockAccountLimit) && !login.IsBlocked && !login.IsDeleted && (configuration.BlockAccountLimit == 0 || status.BlockedBalance < configuration.BlockAccountLimit))
                    {
                        return acc;
                    }
                }
                else
                {
                    throw new Exception("Login not found");
                }
            }
            else
            {
                throw new Exception("Status not found");
            }

            return null;
        }

        private async Task<UserSegmentGroup> GetUserSegmentGroup(string ownerGuid, UserSegmentValues values)
        {
            var group = await _UserSegmentGroupManager.GetGroup(ownerGuid, values);
            return group;
        }

        private async Task<UserSegmentGroup> GetUserSegmentGroup(MerchantCustomer customer)
        {
            if (customer.UserSegmentGroupId.HasValue)
            {
                var group = await _UserSegmentGroupManager.GetGroupByIdFromCache(customer.UserSegmentGroupId.Value);

                if (group != null)
                {
                    return group;
                }
            }

            var query = Repository.GetQuery(t => t.Id == customer.Id);

            var values = (from c in query
                          where c.Id == customer.Id
                          select new UserSegmentValues
                          {
                              ActivityScore = customer.ActivityScore ?? 0,
                              TotalDepositCountMerchant = customer.DepositNumber ?? 0,
                              GroupName = customer.GroupName,
                              LastActivity = customer.LastActivity,
                              RegisterDate = customer.RegisterDate,
                              TotalDepositAmountMerchant = customer.TotalDeposit ?? 0,
                              TotalWithdrawalAmountMerchant = customer.TotalWithdraw ?? 0,
                              TotalWithdrawalCountMerchant = customer.WithdrawNumber ?? 0,
                              TotalDepositCountPardakhtPay = customer.PardakhtPayDepositCount,
                              TotalDepositAmountPardakhtPay = customer.PardakhtPayDepositAmount,
                              TotalWithdrawalCountPardakhtPay = customer.PardakhtPayWithdrawalCount,
                              TotalWithdrawalAmountPardakhtPay = customer.PardakhtPayWithdrawalAmount,
                              WebsiteName = customer.WebsiteName,
                              TotalSportbookAmount = customer.UserTotalSportbook ?? 0,
                              TotalCasinoAmount = customer.UserTotalCasino ?? 0,
                              TotalCasinoCount = customer.UserCasinoNumber ?? 0,
                              TotalSportbookCount = customer.UserSportbookNumber ?? 0
                          }).FirstOrDefault();

            if (values == null)
            {
                return null;
            }

            return await GetUserSegmentGroup(customer.OwnerGuid, values);
        }

        private async Task<UserSegmentGroup> GetUserSegmentGroupForFirstTransaction(MerchantCustomer customer)
        {
            var values = new UserSegmentValues();

            values.ActivityScore = customer.ActivityScore ?? 0;
            values.GroupName = customer.GroupName;
            values.LastActivity = customer.LastActivity;
            values.RegisterDate = customer.RegisterDate;
            values.TotalDepositAmountMerchant = customer.TotalDeposit ?? 0;
            values.TotalDepositCountMerchant = customer.DepositNumber ?? 0;
            values.TotalWithdrawalAmountMerchant = customer.TotalWithdraw ?? 0;
            values.TotalWithdrawalCountMerchant = customer.WithdrawNumber ?? 0;
            values.WebsiteName = customer.WebsiteName;
            values.TotalSportbookAmount = customer.UserTotalSportbook ?? 0;
            values.TotalCasinoAmount = customer.UserTotalCasino ?? 0;
            values.TotalCasinoCount = customer.UserCasinoNumber ?? 0;
            values.TotalSportbookCount = customer.UserSportbookNumber ?? 0;

            return await GetUserSegmentGroup(customer.OwnerGuid, values);
        }

        public async Task<MerchantCustomer> GetCustomerAsync(string ownerGuid, string websiteName, string userId)
        {
            return await Repository.GetCustomer(ownerGuid, websiteName, userId);
        }

        public async Task<PhoneNumbersResponse> ExportPhoneNumbers(MerchantCustomerSearchArgs args)
        {
            IQueryable<MerchantCustomerDTO> itemQuery = GetSearchQuery(args);
            var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

            if (args.FilterModel != null && args.FilterModel.Count > 0)
            {
                itemQuery = itemQuery.ApplyParameters(args.FilterModel, _EncryptionService);
            }

            bool sort = false;
            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "userId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserId);
                        }
                        break;
                    case "websiteName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WebsiteName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WebsiteName);
                        }
                        break;
                    case "groupName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.GroupName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.GroupName);
                        }
                        break;
                    case "depositNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DepositNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DepositNumber);
                        }
                        break;
                    case "lastActivity":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.LastActivity);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.LastActivity);
                        }
                        break;
                    case "registerDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.RegisterDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.RegisterDate);
                        }
                        break;
                    case "widhdrawNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WithdrawNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WithdrawNumber);
                        }
                        break;
                    case "totalDeposit":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalDeposit);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalDeposit);
                        }
                        break;
                    case "totalWidthdraw":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdraw);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdraw);
                        }
                        break;
                    case "totalCompletedTransactionCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalCompletedTransactionCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalCompletedTransactionCount);
                        }
                        break;
                    case "totalTransactionCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalTransactionCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalTransactionCount);
                        }
                        break;
                    case "totalDepositAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalDepositAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalDepositAmount);
                        }
                        break;
                    case "totalWithdrawalCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdrawalCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdrawalCount);
                        }
                        break;
                    case "totalCompletedWithdrawalCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalCompletedWithdrawalCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalCompletedWithdrawalCount);
                        }
                        break;
                    case "totalWithdrawalAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TotalWithdrawalAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TotalWithdrawalAmount);
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
                    case "userSportbookNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserSportbookNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserSportbookNumber);
                        }
                        break;
                    case "userTotalSportbook":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserTotalSportbook);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserTotalSportbook);
                        }
                        break;
                    case "userTotalCasino":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserTotalCasino);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserTotalCasino);
                        }
                        break;
                    case "userCasinoNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserCasinoNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserCasinoNumber);
                        }
                        break;
                    case "phoneNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PhoneNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PhoneNumber);
                        }
                        break;
                    case "phoneNumberRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.PhoneNumberRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.PhoneNumberRelatedCustomers);
                        }
                        break;
                    case "differentCardNumberCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DifferentCardNumberCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DifferentCardNumberCount);
                        }
                        break;
                    case "deviceRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.DeviceRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.DeviceRelatedCustomers);
                        }
                        break;
                    case "cardNumberRelatedCustomers":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CardNumberRelatedCustomers);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CardNumberRelatedCustomers);
                        }
                        break;
                }
            }

            var totalCount = itemQuery.Count();

            var sb = new StringBuilder();
            var phoneNumbers = itemQuery.Where(p => p.PhoneNumber != null).Select(p => p.PhoneNumber).ToList();
            foreach (var data in phoneNumbers)
            {
                sb.AppendLine("\t" + data);
            }

            string path = $"C:\\TempReport\\{Guid.NewGuid()}.csv";
            if (File.Exists(path)) File.Delete(path);

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(sb);
                sw.Close();
                sw.Dispose();
            }

            var repPhoneNumbers = new PhoneNumbersResponse();
            
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/csv");
                fileStreamResult.FileDownloadName = $@"PhoneNumbers-{DateTime.UtcNow.Date.ToShortDateString()}.csv";
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    repPhoneNumbers.Data = ms.ToArray();
                }
                repPhoneNumbers.ContentType = fileStreamResult.ContentType;                
            }
            File.Delete(path);

            return repPhoneNumbers;

          
        }
        public async Task<List<RegisteredPhoneNumbers>> GetRegisteredPhones(int id)
        {
            List<RegisteredPhoneNumbers> registeredPhones = new List<RegisteredPhoneNumbers>();
            var sekehDeviceQuery = _SekehDeviceRepository.GetQuery();
            var sadadDeviceQuery = _SadadPspDeviceRepository.GetQuery();
            var mobileTransferDeviceQuery = _MobileTransferDeviceRepository.GetQuery();

            var sekehRegisteredDevices = sekehDeviceQuery.Where(sk => sk.IsRegistered && sk.MerchantCustomerId == id).Select(sk=> sk.PhoneNumber).ToList();
            var sadadRegisteredDevices = sadadDeviceQuery.Where(sd => sd.IsRegistered && sd.MerchantCustomerId == id).Select(sk => sk.PhoneNumber).ToList();
            var asanPardakhtRegisteredDevices = mobileTransferDeviceQuery.Where(mt => mt.Status==3 && mt.MerchantCustomerId == id).Select(sk => sk.PhoneNumber).ToList();

            var distinctPhones= sekehRegisteredDevices.Concat(sadadRegisteredDevices).Concat(asanPardakhtRegisteredDevices).Distinct().ToList();

            foreach (var phone in distinctPhones) {
                registeredPhones.Add(new RegisteredPhoneNumbers { PhoneNumber = phone});
            }
            return registeredPhones;
        }

        public async Task RemoveRegisteredPhones(int id, RegisteredPhoneNumbersList phoneNumbers)
        {
            var query = Repository.GetQuery();
            var sekehDeviceQuery = _SekehDeviceRepository.GetQuery();
            var sadadDeviceQuery = _SadadPspDeviceRepository.GetQuery();
            var mobileTransferDeviceQuery = _MobileTransferDeviceRepository.GetQuery();

            foreach (var phone in phoneNumbers.PhoneNumber)
            {
                var sekehRegisteredDevices = sekehDeviceQuery.Where(sk => sk.IsRegistered && sk.PhoneNumber == phone && sk.MerchantCustomerId==id).FirstOrDefault();
                if (sekehRegisteredDevices !=null)
                {
                    sekehRegisteredDevices.MerchantCustomerId = -1;
                    await _SekehDeviceRepository.UpdateAsync(sekehRegisteredDevices);
                    await SaveAsync();
                }
                var sadadRegisteredDevices = sadadDeviceQuery.Where(sd => sd.IsRegistered && sd.PhoneNumber == phone && sd.MerchantCustomerId == id).FirstOrDefault();
                if (sadadRegisteredDevices !=null)
                {
                    sadadRegisteredDevices.MerchantCustomerId = -1;
                    await _SadadPspDeviceRepository.UpdateAsync(sadadRegisteredDevices);
                    await SaveAsync();
                }
                var asanPardakhtRegisteredDevices = mobileTransferDeviceQuery.Where(mt => mt.Status == 3 && mt.PhoneNumber == phone && mt.MerchantCustomerId == id).FirstOrDefault();
                if (asanPardakhtRegisteredDevices != null)
                {
                    asanPardakhtRegisteredDevices.MerchantCustomerId = -1;
                    await _MobileTransferDeviceRepository.UpdateAsync(asanPardakhtRegisteredDevices);
                    await SaveAsync();
                }
                var merchantCustomer = query.Where(mc => mc.ConfirmedPhoneNumber == phone && mc.Id == id).FirstOrDefault();
                if (merchantCustomer != null)
                {
                    merchantCustomer.ConfirmedPhoneNumber = "";
                    await Repository.UpdateAsync(merchantCustomer);
                    await SaveAsync();
                }

            }
        }
    }
}

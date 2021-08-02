using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using System.Threading;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class CardToCardAccountGroupItemManager : BaseManager<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>, ICardToCardAccountGroupItemManager
    {
        IMerchantManager _MerchantManager = null;
        ICardToCardAccountGroupOrderManager _OrderManager;
        ICachedObjectManager _CachedObjectManager = null;
        ICardToCardUserSegmentRelationRepository _CardToCardUserSegmentRelationRepository = null;
        static SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
        IBankBotService _BankBotService = null;
        ILogger _Logger = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IBankStatementItemRepository _BankStatementItemRepository = null;
        ITimeZoneService _TimeZoneService = null;

        public CardToCardAccountGroupItemManager(ICardToCardAccountGroupItemRepository repository,
            ICardToCardAccountGroupOrderManager cardToCardAccountGroupOrderManager,
            IMerchantManager merchantManager,
            ICardToCardUserSegmentRelationRepository cardToCardUserSegmentRelationRepository,
            IBankBotService bankBotService,
            ICachedObjectManager cachedObjectManager,
            ILogger<CardToCardAccountGroupItemManager> logger,
            IApplicationSettingService applicationSettingService,
            IBankStatementItemRepository bankStatementItemRepository,
            ITimeZoneService timeZoneService) : base(repository)
        {
            _MerchantManager = merchantManager;
            _OrderManager = cardToCardAccountGroupOrderManager;
            _CachedObjectManager = cachedObjectManager;
            _BankBotService = bankBotService;
            _CardToCardUserSegmentRelationRepository = cardToCardUserSegmentRelationRepository;
            _Logger = logger;
            _ApplicationSettingService = applicationSettingService;
            _BankStatementItemRepository = bankStatementItemRepository;
        }

        public async Task<List<CardToCardAccountGroupItem>> GetActiveRelations()
        {
            var active = (int)CardToCardAccountGroupItemStatus.Active;

            var groupItems = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            var segmentRelations = await _CachedObjectManager.GetCachedItems<CardToCardUserSegmentRelation, ICardToCardUserSegmentRelationRepository>();

            return groupItems.Where(t =>t.Status == active).ToList();
        }

        public async Task<List<CardToCardAccountGroupItem>> GetActiveRelations(int merchantId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup)
        {
            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();
            var merchant = merchants.FirstOrDefault(t => t.Id == merchantId);

            if (merchant == null)
            {
                throw new Exception($"Merchant could not be found with id : {merchantId}");
            }

            var active = (int)CardToCardAccountGroupItemStatus.Active;

            var groupItems = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            var segmentRelations = await _CachedObjectManager.GetCachedItems<CardToCardUserSegmentRelation, ICardToCardUserSegmentRelationRepository>();

            return groupItems.Where(t => t.CardToCardAccountGroupId == merchant.CardToCardAccountGroupId
                && t.Status == active && (cardToCard == null || t.AllowCardToCard == cardToCard)
                && (withdrawal == null || t.AllowWithdrawal == withdrawal)
                && (((userSegmentGroup == null || userSegmentGroup.IsDefault) && !segmentRelations.Any(p => p.CardToCardAccountGroupItemId == t.Id)) || segmentRelations.Any(p => p.UserSegmentGroupId == userSegmentGroup?.Id && p.CardToCardAccountGroupItemId == t.Id))
            ).ToList();
        }

        public async Task<List<CardToCardAccountGroupItem>> GetActiveRelationsWithoutUserSegmentation(int merchantId, bool? cardToCard, bool? withdrawal)
        {
            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();
            var merchant = merchants.FirstOrDefault(t => t.Id == merchantId);

            if (merchant == null)
            {
                throw new Exception($"Merchant could not be found with id : {merchantId}");
            }

            var active = (int)CardToCardAccountGroupItemStatus.Active;

            var groupItems = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            var segmentRelations = await _CachedObjectManager.GetCachedItems<CardToCardUserSegmentRelation, ICardToCardUserSegmentRelationRepository>();

            return groupItems.Where(t => t.CardToCardAccountGroupId == merchant.CardToCardAccountGroupId
                && t.Status == active && (cardToCard == null || t.AllowCardToCard == cardToCard)
                && (withdrawal == null || t.AllowWithdrawal == withdrawal)
            ).ToList();
        }

        public async Task<CardToCardAccountGroupItem> GetCardToCardAccount(int merchantId, int cardToCardAccountId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup)
        {
            var activeItems = await GetActiveRelations(merchantId, cardToCard, withdrawal, userSegmentGroup);

            var relation = activeItems.FirstOrDefault(t => t.CardToCardAccountId == cardToCardAccountId);

            return relation;
        }

        public async Task<CardToCardAccountGroupItem> GetCardToCardAccountWithoutUserSegmentation(int merchantId, int cardToCardAccountId, bool? cardToCard, bool? withdrawal)
        {
            var activeItems = await GetActiveRelationsWithoutUserSegmentation(merchantId, cardToCard, withdrawal);

            var relation = activeItems.FirstOrDefault(t => t.CardToCardAccountId == cardToCardAccountId);

            return relation;
        }

        public async Task<List<CardToCardAccountGroupItem>> GetItemsAsync(int groupId)
        {
            return await Repository.GetItemsAsync(t => t.CardToCardAccountGroupId == groupId);
        }

        public async Task<CardToCardAccountGroupItem> GetRandomRelation(int merchantId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup)
        {
            var relations = await GetActiveRelations(merchantId, cardToCard, withdrawal, userSegmentGroup);

            if (relations.Count == 0)
            {
                return null;
            }

            if (relations.Count == 1)
            {
                return relations[0];
            }

            var groupId = relations[0].CardToCardAccountGroupId;

            var next = _OrderManager.GetOrder(groupId);

            relations = relations.OrderBy(t => t.Id).ToList();

            var relation = relations[next % relations.Count];

            return relation;
        }

        public async Task<CardToCardAccountGroupItem> GetRandomRelationWithoutUserSegmentation(int merchantId, bool? cardToCard, bool? withdrawal)
        {
            var relations = await GetActiveRelationsWithoutUserSegmentation(merchantId, cardToCard, withdrawal);

            if (relations.Count == 0)
            {
                return null;
            }

            if (relations.Count == 1)
            {
                return relations[0];
            }

            var groupId = relations[0].CardToCardAccountGroupId;

            var next = _OrderManager.GetOrder(groupId);

            relations = relations.OrderBy(t => t.Id).ToList();

            var relation = relations[next % relations.Count];

            return relation;
        }

        public async Task ReplaceReservedAccount(CardToCardAccount account, CardToCardAccountGroupItemStatus status)
        {
            try
            {
                await _Semaphore.WaitAsync();

                var bankBotAccounts = await _BankBotService.GetAccountsWithStatus();

                var cardToCardAccounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();

                var relations = await Repository.GetItemsAsync(t => t.CardToCardAccountId == account.Id);

                for (int i = 0; i < relations.Count; i++)
                {
                    var relation = relations[i];

                    if (relation.RelationStatus == CardToCardAccountGroupItemStatus.Reserved && status == CardToCardAccountGroupItemStatus.Blocked)
                    {
                        relation.RelationStatus = status;
                        relation.BlockedDate = DateTime.UtcNow;

                        await UpdateAsync(relation);
                        await SaveAsync();
                    }
                    else if (relation.RelationStatus == CardToCardAccountGroupItemStatus.Active)
                    {
                        if (status == CardToCardAccountGroupItemStatus.Blocked)
                        {
                            relation.RelationStatus = status;
                            relation.BlockedDate = DateTime.UtcNow;

                            await UpdateAsync(relation);
                            await SaveAsync();
                        }

                        CardToCardAccountGroupItem reserved = null;

                        var groups = await GetUserSegments(relation.Id);

                        var reserveds = await Repository.GetItemsAsync(t => t.CardToCardAccountGroupId == relation.CardToCardAccountGroupId
                            && t.Status == (int)CardToCardAccountGroupItemStatus.Reserved
                            && t.AllowCardToCard == relation.AllowCardToCard
                            && t.AllowWithdrawal == relation.AllowWithdrawal);

                        reserved = await CheckUserSegmentForReserveAccounts(groups, reserveds, cardToCardAccounts, bankBotAccounts);

                        if (reserved == null)
                        {
                            reserved = GetAccountHasHighLimit(reserveds, cardToCardAccounts, bankBotAccounts);
                        }

                        if (reserved == null)
                        {
                            if (relation.AllowCardToCard)
                            {
                                var cardToCards = await Repository.GetItemsAsync(t => t.CardToCardAccountGroupId == relation.CardToCardAccountGroupId && t.Status == (int)CardToCardAccountGroupItemStatus.Reserved && t.AllowCardToCard);

                                reserved = await CheckUserSegmentForReserveAccounts(groups, cardToCards, cardToCardAccounts, bankBotAccounts);

                                if (reserved == null)
                                {
                                    reserved = GetAccountHasHighLimit(cardToCards, cardToCardAccounts, bankBotAccounts);
                                }

                                if (reserved != null)
                                {
                                    reserved.RelationStatus = CardToCardAccountGroupItemStatus.Active;

                                    await AddRelationsToNewAccount(relation, reserved);

                                    await UpdateAsync(reserved);
                                    await SaveAsync();
                                }
                            }

                            if (relation.AllowWithdrawal)
                            {
                                var cardToCards = await Repository.GetItemsAsync(t => t.CardToCardAccountGroupId == relation.CardToCardAccountGroupId && t.Status == (int)CardToCardAccountGroupItemStatus.Reserved && t.AllowWithdrawal);

                                reserved = await CheckUserSegmentForReserveAccounts(groups, cardToCards, cardToCardAccounts, bankBotAccounts);

                                if (reserved == null)
                                {
                                    //reserved = cardToCards.FirstOrDefault();
                                    reserved = GetAccountHasHighLimit(cardToCards, cardToCardAccounts, bankBotAccounts);
                                }

                                if (reserved != null)
                                {
                                    reserved.RelationStatus = CardToCardAccountGroupItemStatus.Active;

                                    await AddRelationsToNewAccount(relation, reserved);

                                    await UpdateAsync(reserved);
                                    await SaveAsync();
                                }
                            }
                        }
                        else
                        {
                            if (reserved != null)
                            {
                                reserved.RelationStatus = CardToCardAccountGroupItemStatus.Active;

                                await AddRelationsToNewAccount(relation, reserved);

                                await UpdateAsync(reserved);
                                await SaveAsync();
                            }
                        }

                        if (status == CardToCardAccountGroupItemStatus.Paused /*&& (reserved != null || !account.SwitchIfHasReserveAccount)*/)
                        {
                            relation.PausedDate = DateTime.UtcNow;
                            relation.TempGroupItemId = reserved?.Id;
                            relation.RelationStatus = CardToCardAccountGroupItemStatus.Paused;

                            await UpdateAsync(relation);
                            await SaveAsync();
                        }

                        if (reserved != null)
                        {
                            var oldRelations = Repository.GetQuery(t => t.TempGroupItemId == relation.Id).ToList();

                            if (oldRelations.Count > 0)
                            {
                                relation.TempGroupItemId = null;

                                await UpdateAsync(relation);
                                await SaveAsync();
                            }

                            for (int oldRelationId = 0; oldRelationId < oldRelations.Count; oldRelationId++)
                            {
                                var oldRelation = oldRelations[oldRelationId];

                                oldRelation.TempGroupItemId = reserved.Id;

                                await UpdateAsync(oldRelation);
                                await SaveAsync();
                            }
                        }
                    }
                }

                await SaveAsync();
                await _CardToCardUserSegmentRelationRepository.SaveChangesAsync();
            }
            finally
            {
                _Semaphore.Release();
            }
        }

        private async Task<CardToCardAccountGroupItem> CheckUserSegmentForReserveAccounts(List<int> groups,
            List<CardToCardAccountGroupItem> reserveds,
            List<CardToCardAccount> cardToCardAccounts,
            List<BankBotAccountWithStatusDTO> bankBotAccounts)
        {
            List<CardToCardAccountGroupItem> items = new List<CardToCardAccountGroupItem>();

            for (int reservedIndex = 0; reservedIndex < reserveds.Count; reservedIndex++)
            {
                var r = reserveds[reservedIndex];

                var reservedGroups = await GetUserSegments(r.Id);

                if (groups.All(t => reservedGroups.Contains(t)) && reservedGroups.All(t => groups.Contains(t)))
                {
                    items.Add(r);
                }
            }

            return GetAccountHasHighLimit(items, cardToCardAccounts, bankBotAccounts);
        }

        private CardToCardAccountGroupItem GetAccountHasHighLimit(List<CardToCardAccountGroupItem> groupItems,
            List<CardToCardAccount> cardAccounts,
            List<BankBotAccountWithStatusDTO> bankBotAccounts)
        {
            return (from g in groupItems
                    join a in cardAccounts on g.CardToCardAccountId equals a.Id
                    join b in bankBotAccounts on a.AccountGuid equals b.AccountGuid
                    where b.TransferType == (int)TransferType.Normal
                    orderby  b.WithdrawRemainedAmountForDay descending
                    select g).FirstOrDefault();
        }

        public async Task<List<CardToCardAccountGroupItem>> GetAllRelations(int merchantId, bool? cardToCard, bool? withdrawal)
        {
            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();
            var merchant = merchants.FirstOrDefault(t => t.Id == merchantId);

            if (merchant == null)
            {
                throw new Exception($"Merchant could not be found with id : {merchantId}");
            }

            var groupItems = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            return groupItems.Where(t => t.CardToCardAccountGroupId == merchant.CardToCardAccountGroupId && (cardToCard == null || t.AllowCardToCard == cardToCard) && (withdrawal == null || t.AllowWithdrawal == withdrawal)).ToList();
        }

        public async Task<List<int>> GetUserSegments(int cardToCardAccountGroupItemId)
        {
            var items = await _CardToCardUserSegmentRelationRepository.GetItemsAsync(t => t.CardToCardAccountGroupItemId == cardToCardAccountGroupItemId);

            return items.Select(p => p.UserSegmentGroupId).ToList();
        }

        public override async Task DeleteAsync(CardToCardAccountGroupItem item)
        {
            var relations = await _CardToCardUserSegmentRelationRepository.GetItemsAsync(t => t.CardToCardAccountGroupItemId == item.Id);

            for (int i = 0; i < relations.Count; i++)
            {
                await _CardToCardUserSegmentRelationRepository.DeleteAsync(relations[i]);
            }

            await base.DeleteAsync(item);
            await _CardToCardUserSegmentRelationRepository.SaveChangesAsync();
        }

        private async Task AddRelationsToNewAccount(CardToCardAccountGroupItem relation, CardToCardAccountGroupItem newItem)
        {
            var ids = await GetUserSegments(relation.Id);

            await AddRelationsToNewAccount(newItem, ids);
        }

        private async Task AddRelationsToNewAccount(CardToCardAccountGroupItem newItem, List<int> ids)
        {
            var existingRelations = await _CardToCardUserSegmentRelationRepository.GetItemsAsync(t => t.CardToCardAccountGroupItemId == newItem.Id);

            for (int i = 0; i < existingRelations.Count; i++)
            {
                await _CardToCardUserSegmentRelationRepository.DeleteAsync(existingRelations[i]);
            }

            for (int i = 0; i < ids.Count; i++)
            {
                CardToCardUserSegmentRelation segmentRelation = new CardToCardUserSegmentRelation();
                segmentRelation.CardToCardAccountGroupItemId = newItem.Id;
                segmentRelation.UserSegmentGroupId = ids[i];

                await _CardToCardUserSegmentRelationRepository.InsertAsync(segmentRelation);
            }
            await _CardToCardUserSegmentRelationRepository.SaveChangesAsync();
        }

        public async Task<CardToCardAccountGroupItem> AddAsync(CardToCardAccountGroupItem item, List<int> userSegments)
        {
            var result = await base.AddAsync(item);

            await _CardToCardUserSegmentRelationRepository.SaveChangesAsync();

            for (int i = 0; i < userSegments.Count; i++)
            {
                var relation = new CardToCardUserSegmentRelation();
                relation.CardToCardAccountGroupItemId = result.Id;
                relation.UserSegmentGroupId = userSegments[i];

                await _CardToCardUserSegmentRelationRepository.InsertAsync(relation);
            }

            return result;
        }

        public async Task<CardToCardAccountGroupItem> UpdateAsync(CardToCardAccountGroupItem item, List<int> userSegments)
        {
            var oldItem = await Repository.GetEntityByIdAsync(item.Id);
            item.BlockedDate = oldItem.BlockedDate;

            if (item.RelationStatus == CardToCardAccountGroupItemStatus.Paused)
            {
                item.PausedDate = oldItem.PausedDate;
                item.TempGroupItemId = oldItem.TempGroupItemId;
            }
            else
            {
                item.PausedDate = null;
                item.TempGroupItemId = null;
            }

            var oldRelations = await _CardToCardUserSegmentRelationRepository.GetItemsAsync(t => t.CardToCardAccountGroupItemId == item.Id);

            var deletedRelations = oldRelations.Where(t => !userSegments.Contains(t.UserSegmentGroupId)).ToList();

            for (int i = 0; i < deletedRelations.Count; i++)
            {
                await _CardToCardUserSegmentRelationRepository.DeleteAsync(deletedRelations[i]);
            }

            var newIds = userSegments.Where(t => !oldRelations.Any(p => p.UserSegmentGroupId == t)).ToList();

            for (int i = 0; i < newIds.Count; i++)
            {
                var relation = new CardToCardUserSegmentRelation();
                relation.CardToCardAccountGroupItemId = item.Id;
                relation.UserSegmentGroupId = newIds[i];

                await _CardToCardUserSegmentRelationRepository.InsertAsync(relation);
            }
            await _CardToCardUserSegmentRelationRepository.SaveChangesAsync();

            return await UpdateAsync(item);
        }

        public async Task CheckPausedAccounts()
        {
            _Logger.LogInformation("Paused accounts are being checked");

            var items = await GetItemsAsync(t => t.Status == (int)CardToCardAccountGroupItemStatus.Paused);

            var cardToCardAccounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();
            var bankAccounts = await _BankBotService.GetAccountsWithStatus();
            var logins = await _BankBotService.GetLogins();
            var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();


            if (items.Count > 0)
            {
                _Logger.LogInformation($"{items.Count} account(s) was found");

                for (int i = 0; i < items.Count; i++)
                {
                    _Logger.LogInformation($"{items.Count} paused account(s) was found");

                    var item = items[i];

                    var cardToCardAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == item.CardToCardAccountId);

                    if (cardToCardAccount != null)
                    {
                        var bankAccount = bankAccounts.FirstOrDefault(t => t.AccountGuid == cardToCardAccount.AccountGuid && t.TransferType == (int)TransferType.Normal);

                        if (bankAccount != null)
                        {
                            var isBlocked = await CheckIfAccountIsBlocked(item, bankAccount, logins, configuration);

                            if (isBlocked)
                            {
                                _Logger.LogInformation($"Paused account {item.Id} has been blocked");

                                continue;
                            }

                            if (!cardToCardAccount.SwitchOnLimit || (cardToCardAccount.SwitchLimitAmount ?? 0) < bankAccount.WithdrawableLimit)
                            {
                                if (cardToCardAccount.SwitchCreditDailyLimit.HasValue && cardToCardAccount.SwitchCreditDailyLimit > 0)
                                {
                                    var date = DateTime.UtcNow;

                                    var iranianDate = await _TimeZoneService.ConvertCalendar(date, Helper.Utc, "ir2");

                                    iranianDate = await _TimeZoneService.ConvertCalendar(date.Date, "ir2", Helper.Utc);

                                    var totalAmount = await _BankStatementItemRepository.GetTotalCreditAmount(cardToCardAccount.AccountGuid, iranianDate, iranianDate.AddDays(1));

                                    if(totalAmount >= cardToCardAccount.SwitchCreditDailyLimit.Value)
                                    {
                                        continue;
                                    }
                                }

                                var tempId = item.TempGroupItemId;

                                if (tempId.HasValue)
                                {
                                    _Logger.LogInformation($"Paused account {item.Id} is being reactivated");

                                    item.RelationStatus = CardToCardAccountGroupItemStatus.Active;
                                    item.PausedDate = null;
                                    item.TempGroupItemId = null;

                                    await UpdateAsync(item);
                                    await SaveAsync();

                                    var temp = await GetEntityByIdAsync(tempId.Value);

                                    if (temp != null && temp.RelationStatus == CardToCardAccountGroupItemStatus.Active)
                                    {
                                        var tempCardAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == temp.CardToCardAccountId);

                                        var tempBankBotAccount = bankAccounts.FirstOrDefault(t => t.AccountGuid == tempCardAccount.AccountGuid && t.TransferType == (int)TransferType.Normal);

                                        var isTempBlocked = await CheckIfAccountIsBlocked(temp, tempBankBotAccount, logins, configuration);

                                        if (!isTempBlocked)
                                        {
                                            _Logger.LogInformation($"Temp account {temp.Id} is reserved");

                                            temp.RelationStatus = CardToCardAccountGroupItemStatus.Reserved;
                                            await UpdateAsync(temp);
                                            await SaveAsync();
                                        }
                                    }
                                }
                                else
                                {
                                    var count = Repository.GetQuery(t => t.CardToCardAccountGroupId == item.CardToCardAccountGroupId && t.Status == (int)CardToCardAccountGroupItemStatus.Active).Count();

                                    if (count > 0)
                                    {
                                        item.RelationStatus = CardToCardAccountGroupItemStatus.Reserved;
                                    }
                                    else
                                    {
                                        _Logger.LogInformation($"Paused account {item.Id} is reactivated because of 0 active accounts");
                                        item.RelationStatus = CardToCardAccountGroupItemStatus.Active;
                                    }
                                    item.PausedDate = null;
                                    item.TempGroupItemId = null;

                                    await UpdateAsync(item);
                                    await SaveAsync();
                                }
                            }
                        }
                    }
                }
            }

            var groups = Repository.GetQuery().Select(t => t.CardToCardAccountGroupId).ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var activeCount = Repository.GetQuery(t => t.CardToCardAccountGroupId == groups[i] && t.Status == (int)CardToCardAccountGroupItemStatus.Active).Count();

                if (activeCount == 0)
                {
                    _Logger.LogInformation($"No active account for {groups[i]}");

                    var reserved = Repository.GetQuery(t => t.CardToCardAccountGroupId == groups[i] && t.Status == (int)CardToCardAccountGroupItemStatus.Reserved).FirstOrDefault();

                    if (reserved != null)
                    {
                        _Logger.LogInformation($"Reseved account found for {groups[i]}");

                        reserved.RelationStatus = CardToCardAccountGroupItemStatus.Active;

                        await UpdateAsync(reserved);
                        await SaveAsync();

                        await AddRelationsToNewAccount(reserved, new List<int>());
                    }
                    else
                    {
                        var pausedAccounts = Repository.GetQuery(t => t.CardToCardAccountGroupId == groups[i] && t.Status == (int)CardToCardAccountGroupItemStatus.Paused).ToList();

                        for (int pausedIndex = 0; pausedIndex < pausedAccounts.Count; pausedIndex++)
                        {
                            var pausedItem = pausedAccounts[pausedIndex];

                            var pausedAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == pausedItem.CardToCardAccountId);

                            if (pausedAccount != null && pausedAccount.SwitchIfHasReserveAccount)
                            {
                                _Logger.LogInformation($"Paused account {pausedItem.Id} is being reactivated becuase of (SwitchIfHasReserveAccount)");

                                pausedItem.RelationStatus = CardToCardAccountGroupItemStatus.Active;

                                await UpdateAsync(pausedItem);
                                await SaveAsync();

                                await AddRelationsToNewAccount(pausedItem, new List<int>());

                                break;
                            }
                        }
                    }
                }
            }

        }

        private async Task<bool> CheckIfAccountIsBlocked(CardToCardAccountGroupItem item, BankBotAccountWithStatusDTO bankAccount, List<BotLoginInformation> logins, BankAccountConfiguration configuration)
        {
            var login = logins.FirstOrDefault(t => t.LoginGuid == bankAccount.LoginGuid);

            if (!bankAccount.IsOpen(configuration.BlockAccountLimit) || login.IsBlocked || login.IsDeleted)
            {
                item.RelationStatus = CardToCardAccountGroupItemStatus.Blocked;
                item.BlockedDate = DateTime.UtcNow;
                item.PausedDate = null;
                item.TempGroupItemId = null;

                await UpdateAsync(item);
                await SaveAsync();
                return true;
            }

            return false;
        }
    }
}

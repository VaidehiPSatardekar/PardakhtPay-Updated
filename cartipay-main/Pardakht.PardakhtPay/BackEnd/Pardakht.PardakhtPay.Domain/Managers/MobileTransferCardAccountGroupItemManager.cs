using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MobileTransferCardAccountGroupItemManager : BaseManager<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>, IMobileTransferCardAccountGroupItemManager
    {
        IMobileCardAccountUserSegmentRelationRepository _UserSegmentRelationRepository = null;
        ICachedObjectManager _CachedObjectManager = null;
        ILogger _Logger = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IBankBotService _BankBotService = null;

        public MobileTransferCardAccountGroupItemManager(IMobileTransferCardAccountGroupItemRepository repository,
            IMobileCardAccountUserSegmentRelationRepository userSegmentRelationRepository,
            ICachedObjectManager cachedObjectManager,
            ILogger<MobileTransferCardAccountGroupItemManager> logger,
            IApplicationSettingService applicationSettingsService,
            IBankBotService bankBotService) : base(repository)
        {
            _CachedObjectManager = cachedObjectManager;
            _UserSegmentRelationRepository = userSegmentRelationRepository;
            _Logger = logger;
            _ApplicationSettingService = applicationSettingsService;
            _BankBotService = bankBotService;
        }

        public async Task<List<MobileTransferCardAccountGroupItem>> GetActiveRelations(int merchantId, UserSegmentGroup userSegmentGroup)
        {
            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();
            var merchant = merchants.FirstOrDefault(t => t.Id == merchantId);

            if (merchant == null)
            {
                throw new Exception($"Merchant could not be found with id : {merchantId}");
            }

            var active = (int)MobileTransferCardAccountGroupItemStatus.Active;

            var groupItems = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>();

            var segmentRelations = await _CachedObjectManager.GetCachedItems<MobileCardAccountUserSegmentRelation, IMobileCardAccountUserSegmentRelationRepository>();

            return groupItems.Where(t => t.GroupId == merchant.MobileTransferAccountGroupId
                && t.Status == active
                && (((userSegmentGroup == null || userSegmentGroup.IsDefault) && !segmentRelations.Any(p => p.MobileTransferCardAccountGroupItemId == t.Id)) || segmentRelations.Any(p => p.UserSegmentGroupId == userSegmentGroup?.Id && p.MobileTransferCardAccountGroupItemId == t.Id))
            ).ToList();
        }

        public async Task<List<MobileTransferCardAccountGroupItem>> GetActiveRelationsWithoutUserSegmentation(int merchantId)
        {
            var merchants = await _CachedObjectManager.GetCachedItems<Merchant, IMerchantRepository>();
            var merchant = merchants.FirstOrDefault(t => t.Id == merchantId);

            if (merchant == null)
            {
                throw new Exception($"Merchant could not be found with id : {merchantId}");
            }

            var active = (int)MobileTransferCardAccountGroupItemStatus.Active;

            var groupItems = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>();

            return groupItems.Where(t => t.GroupId == merchant.MobileTransferAccountGroupId
                && t.Status == active
            ).ToList();
        }

        public async Task<List<MobileTransferCardAccountGroupItem>> GetItemsAsync(int groupId)
        {
            return await Repository.GetItemsAsync(t => t.GroupId == groupId);
        }

        public async Task<MobileTransferCardAccountGroupItem> GetRandomRelation(int merchantId, UserSegmentGroup userSegmentGroup, bool mobile)
        {
            var relations = await GetActiveRelations(merchantId, userSegmentGroup);

            var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            relations = (from r in relations
                         join a in accounts on r.ItemId equals a.Id
                         where (a.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal) == mobile
                         select r)
                         .ToList();

            if (relations.Count == 0)
            {
                return null;
            }

            MobileTransferCardAccountGroupItem relation = null;

            if (relations.Count == 1)
            {
                relation = relations[0];
            }
            else
            {
                var next = new Random().Next(0, relations.Count);

                relations = relations.OrderBy(t => t.Id).ToList();

                relation = relations[next % relations.Count];
            }

            var account = accounts.FirstOrDefault(t => t.Id == relation.ItemId);

            var isActive = true;

            if (account.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
            {
                isActive = await CheckPardakhtPayAccountIsBlocked(account, relation);
            }

            if (!isActive)
            {
                return await GetRandomRelation(merchantId, userSegmentGroup, mobile);
            }
            else
            {
                return relation;
            }
        }

        public async Task<MobileTransferCardAccountGroupItem> GetRandomRelationWithoutUserSegmentation(int merchantId, bool mobile)
        {
            var relations = await GetActiveRelationsWithoutUserSegmentation(merchantId);

            var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            relations = (from r in relations
                         join a in accounts on r.ItemId equals a.Id
                         where (a.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal) == mobile
                         select r)
                         .ToList();

            if (relations.Count == 0)
            {
                return null;
            }

            MobileTransferCardAccountGroupItem relation = null;

            if (relations.Count == 1)
            {
                relation = relations[0];
            }
            else
            {
                var groupId = relations[0].GroupId;

                var next = new Random().Next(0, relations.Count);

                relations = relations.OrderBy(t => t.Id).ToList();

                relation = relations[next % relations.Count];
            }

            var account = accounts.FirstOrDefault(t => t.Id == relation.ItemId);

            var isActive = true;

            if (account.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
            {
                isActive = await CheckPardakhtPayAccountIsBlocked(account, relation);
            }

            if (!isActive)
            {
                return await GetRandomRelationWithoutUserSegmentation(merchantId, mobile);
            }
            else
            {
                return relation;
            }
        }

        public async Task<MobileTransferCardAccountGroupItem> AddAsync(MobileTransferCardAccountGroupItem item, List<int> userSegments)
        {
            var result = await base.AddAsync(item);

            await _UserSegmentRelationRepository.SaveChangesAsync();

            for (int i = 0; i < userSegments.Count; i++)
            {
                var relation = new MobileCardAccountUserSegmentRelation();
                relation.MobileTransferCardAccountGroupItemId = result.Id;
                relation.UserSegmentGroupId = userSegments[i];

                await _UserSegmentRelationRepository.InsertAsync(relation);
            }

            return result;
        }

        public async Task<MobileTransferCardAccountGroupItem> UpdateAsync(MobileTransferCardAccountGroupItem item, List<int> userSegments)
        {
            var oldItem = await Repository.GetEntityByIdAsync(item.Id);

            var oldRelations = await _UserSegmentRelationRepository.GetItemsAsync(t => t.MobileTransferCardAccountGroupItemId == item.Id);

            var deletedRelations = oldRelations.Where(t => !userSegments.Contains(t.UserSegmentGroupId)).ToList();

            for (int i = 0; i < deletedRelations.Count; i++)
            {
                await _UserSegmentRelationRepository.DeleteAsync(deletedRelations[i]);
            }

            var newIds = userSegments.Where(t => !oldRelations.Any(p => p.UserSegmentGroupId == t)).ToList();

            for (int i = 0; i < newIds.Count; i++)
            {
                var relation = new MobileCardAccountUserSegmentRelation();
                relation.MobileTransferCardAccountGroupItemId = item.Id;
                relation.UserSegmentGroupId = newIds[i];

                await _UserSegmentRelationRepository.InsertAsync(relation);
            }
            await _UserSegmentRelationRepository.SaveChangesAsync();

            return await UpdateAsync(item);
        }

        public async Task<List<int>> GetUserSegments(int mobileTransferCardAccountGroupItemId)
        {
            var items = await _UserSegmentRelationRepository.GetItemsAsync(t => t.MobileTransferCardAccountGroupItemId == mobileTransferCardAccountGroupItemId);

            return items.Select(p => p.UserSegmentGroupId).ToList();
        }

        public async Task<List<MobileTransferCardAccountGroupItem>> GetAccountGroupItems()
        {
            var groupItems = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>();

            var items = groupItems.Where(t => t.ItemStatus == MobileTransferCardAccountGroupItemStatus.Active).ToList();

            return items;
        }

        public async Task Replace(MobileTransferCardAccountGroupItem previous)
        {
            var groupItems = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>();

            var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            var previousAccount = accounts.FirstOrDefault(t => t.Id == previous.ItemId);

            if (previousAccount == null)
            {
                throw new Exception($"Could not find any PardakhtPal account for replacement with id {previous.ItemId}");
            }

            var accountIds = accounts.Where(t => t.PaymentProviderType == previousAccount.PaymentProviderType).Select(p => p.Id).ToList();

            var next = groupItems.FirstOrDefault(t =>
                t.ItemStatus == MobileTransferCardAccountGroupItemStatus.Reserved
                && t.GroupId == previous.GroupId
                && accountIds.Contains(t.ItemId));

            if (next != null)
            {
                var userSegments = await _UserSegmentRelationRepository.GetItemsAsync(t => t.MobileTransferCardAccountGroupItemId == next.ItemId);

                var previousUserSegments = await _UserSegmentRelationRepository.GetItemsAsync(t => t.MobileTransferCardAccountGroupItemId == previous.ItemId);

                for (int i = 0; i < userSegments.Count; i++)
                {
                    await _UserSegmentRelationRepository.DeleteAsync(userSegments[i]);
                }

                for (int i = 0; i < previousUserSegments.Count; i++)
                {
                    previousUserSegments[i].MobileTransferCardAccountGroupItemId = next.ItemId;
                    await _UserSegmentRelationRepository.UpdateAsync(previousUserSegments[i]);
                }

                await _UserSegmentRelationRepository.SaveChangesAsync();

                next.ItemStatus = MobileTransferCardAccountGroupItemStatus.Active;

                await Repository.UpdateAsync(next);
                await Repository.SaveChangesAsync();
            }
            else
            {
                _Logger.LogWarning($"New PardakhtPal account couldn't be found to replace {previous.ItemId} {previousAccount.MerchantId}");
            }
        }

        public async Task<bool> CheckPardakhtPayAccountIsBlocked(MobileTransferCardAccount account, MobileTransferCardAccountGroupItem activeItem)
        {
            if (!string.IsNullOrEmpty(account.CardToCardAccountGuid))
            {
                var accountStatuses = await _BankBotService.GetSingleAccountsWithStatus(account.CardToCardAccountGuid, TransferType.Normal);

                var accountStatus = accountStatuses.FirstOrDefault();

                if (accountStatus != null)
                {
                    var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

                    var logins = await _BankBotService.GetLogins();

                    var login = logins.FirstOrDefault(t => t.Id == accountStatus.LoginId);

                    if (login != null && !login.IsDeleted)
                    {
                        if (!accountStatus.IsOpen(configuration.BlockAccountLimit) || login.IsBlocked)
                        {
                            activeItem.ItemStatus = MobileTransferCardAccountGroupItemStatus.Blocked;

                            await UpdateAsync(activeItem);
                            await SaveAsync();

                            await Replace(activeItem);

                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
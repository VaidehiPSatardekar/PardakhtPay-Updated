using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Validators;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class UserSegmentGroupManager : BaseManager<UserSegmentGroup, IUserSegmentGroupRepository>, IUserSegmentGroupManager
    {
        IUserSegmentManager _SegmentManager = null;
        ICachedObjectManager _CachedObjectManager = null;

        public UserSegmentGroupManager(IUserSegmentGroupRepository repository,
            ICachedObjectManager cachedObjectManager,
            IUserSegmentManager segmentManager):base(repository)
        {
            _SegmentManager = segmentManager;
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<bool> Validate(int userSegmentGroupId, Dictionary<int, object> values)
        {
            var segments = await _SegmentManager.GetItemsAsync(userSegmentGroupId);

            if(segments.Count == 0)
            {
                return true;
            }

            IValidator validator = null;
            IValidator previous = null;

            for (int i = 0; i < segments.Count; i++)
            {
                var v = segments[i].GetValidator(values);

                if(previous != null)
                {
                    previous.Next = v;
                }
                else
                {
                    validator = v;
                }

                previous = v;
            }

            return validator.Validate();
        }

        public async Task<UserSegmentGroup> GetGroup(string ownerGuid, Dictionary<int, object> values)
        {
            var cachedGroups = await _CachedObjectManager.GetCachedItems<UserSegmentGroup, IUserSegmentGroupRepository>();

            var groups = cachedGroups.Where(t => t.OwnerGuid == ownerGuid && t.IsActive && !t.IsDeleted).ToList();

            if (values != null)
            {
                groups = groups.OrderBy(t => t.Order).ToList();

                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];

                    var validated = await Validate(group.Id, values);

                    if (validated)
                    {
                        return group;
                    }
                }
            }

            return groups.FirstOrDefault(t => t.IsDefault);
        }

        public async Task CreateDefaultUserSegmentGroups(string tenantGuid, string ownerGuid)
        {
            var oldGroup = Repository.GetQuery(t => t.IsActive && t.IsDefault && t.TenantGuid == tenantGuid && t.OwnerGuid == ownerGuid).FirstOrDefault();

            if (oldGroup == null)
            {
                var defaultGroup = new UserSegmentGroup();
                defaultGroup.IsActive = true;
                defaultGroup.IsDefault = true;
                defaultGroup.IsMalicious = false;
                defaultGroup.Name = "Default";
                defaultGroup.TenantGuid = tenantGuid;
                defaultGroup.OwnerGuid = ownerGuid;
                defaultGroup.CreateDate = DateTime.UtcNow;
                defaultGroup.Order = int.MaxValue;

                await Repository.InsertAsync(defaultGroup);

                var maliciousGroup = new UserSegmentGroup();
                maliciousGroup.IsActive = true;
                maliciousGroup.IsMalicious = true;
                maliciousGroup.IsDefault = false;
                maliciousGroup.Name = "Malicious Users";
                maliciousGroup.OwnerGuid = ownerGuid;
                maliciousGroup.TenantGuid = tenantGuid;
                maliciousGroup.CreateDate = DateTime.UtcNow;
                maliciousGroup.Order = -1;

                await Repository.InsertAsync(maliciousGroup);

                await Repository.SaveChangesAsync();

                var maliciousGroupSegment = new UserSegment();
                maliciousGroupSegment.UserSegmentCompareType = UserSegmentCompareType.Equals;
                maliciousGroupSegment.UserSegmentGroupId = maliciousGroup.Id;
                maliciousGroupSegment.UserSegmentType = UserSegmentType.GroupName;
                maliciousGroupSegment.Value = "Malicious";

                await _SegmentManager.AddAsync(maliciousGroupSegment);
                await _SegmentManager.SaveAsync();
            }
        }

        public async Task<UserSegmentGroup> GetGroup(string ownerGuid, UserSegmentValues segmentValues)
        {
            var values = new Dictionary<int, object>();

            values.Add((int)UserSegmentType.TotalDepositCountPardakhtPay, segmentValues.TotalDepositCountPardakhtPay);
            values.Add((int)UserSegmentType.TotalDepositAmountPardakhtPay, segmentValues.TotalDepositAmountPardakhtPay);
            //values.Add((int)UserSegmentType.WaitingPaymentTransactionCount, segmentValues.WaitingPaymentCount);
            //values.Add((int)UserSegmentType.ExpiredTransactionCount, segmentValues.ExpiredDepositCount);
            values.Add((int)UserSegmentType.TotalWithdrawalCountPardakhtPay, segmentValues.TotalWithdrawalCountPardakhtPay);
            values.Add((int)UserSegmentType.TotalWithdrawalCountMerchant, segmentValues.TotalWithdrawalCountMerchant);
            values.Add((int)UserSegmentType.TotalWithdrawalAmountPardakhtPay, segmentValues.TotalWithdrawalAmountPardakhtPay);
            values.Add((int)UserSegmentType.TotalWithdrawalAmountMerchant, segmentValues.TotalWithdrawalAmountMerchant);
            values.Add((int)UserSegmentType.TotalDepositCountMerchant, segmentValues.TotalDepositCountMerchant);
            values.Add((int)UserSegmentType.TotalDepositAmountMerchant, segmentValues.TotalDepositAmountMerchant);
            values.Add((int)UserSegmentType.RegistrationDate, segmentValues.RegisterDate);
            values.Add((int)UserSegmentType.GroupName, segmentValues.GroupName);
            values.Add((int)UserSegmentType.ActivityScore, segmentValues.ActivityScore);
            values.Add((int)UserSegmentType.LastActivity, segmentValues.LastActivity);
            values.Add((int)UserSegmentType.WebsiteName, segmentValues.WebsiteName);
            values.Add((int)UserSegmentType.TotalSportbookAmount, segmentValues.TotalSportbookAmount);
            values.Add((int)UserSegmentType.TotalSportbookCount, segmentValues.TotalSportbookCount);
            values.Add((int)UserSegmentType.TotalCasinoAmount, segmentValues.TotalCasinoAmount);
            values.Add((int)UserSegmentType.TotalCasinoCount, segmentValues.TotalCasinoCount);

            return await GetGroup(ownerGuid, values);
        }

        public async Task<UserSegmentGroup> GetGroupByIdFromCache(int id)
        {
            var groups = await _CachedObjectManager.GetCachedItems<UserSegmentGroup, IUserSegmentGroupRepository>();

            return groups.FirstOrDefault(t => t.Id == id);
        }
    }
}

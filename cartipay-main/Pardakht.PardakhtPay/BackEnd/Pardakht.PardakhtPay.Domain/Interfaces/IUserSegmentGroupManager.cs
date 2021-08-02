using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IUserSegmentGroupManager : IBaseManager<UserSegmentGroup>
    {
        Task<bool> Validate(int userSegmentGroupId, Dictionary<int, object> values);

        Task<UserSegmentGroup> GetGroup(string ownerGuid, Dictionary<int, object> values);

        Task<UserSegmentGroup> GetGroup(string ownerGuid, UserSegmentValues segmentValues);

        Task<UserSegmentGroup> GetGroupByIdFromCache(int id);

        Task CreateDefaultUserSegmentGroups(string tenantGuid, string ownerGuid);
    }
}

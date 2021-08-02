using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IUserSegmentManager : IBaseManager<UserSegment>
    {
        Task<List<UserSegment>> GetItemsAsync(int groupId);
    }
}

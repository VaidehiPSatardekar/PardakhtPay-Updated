using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IUserSegmentGroupService : IServiceBase<UserSegmentGroup, IUserSegmentGroupManager>
    {
        Task<WebResponse<List<UserSegmentGroupDTO>>> GetAllItemsAsync();

        Task<WebResponse<UserSegmentGroupDTO>> InsertAsync(UserSegmentGroupDTO item);

        Task<WebResponse<UserSegmentGroupDTO>> UpdateAsync(UserSegmentGroupDTO item);

        Task<WebResponse<UserSegmentGroupDTO>> GetItemById(int id);
    }
}

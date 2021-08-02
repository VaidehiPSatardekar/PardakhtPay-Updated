using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMobileTransferCardAccountGroupItemManager : IBaseManager<MobileTransferCardAccountGroupItem>
    {
        Task<MobileTransferCardAccountGroupItem> AddAsync(MobileTransferCardAccountGroupItem item, List<int> userSegments);

        Task<MobileTransferCardAccountGroupItem> UpdateAsync(MobileTransferCardAccountGroupItem item, List<int> userSegments);

        Task<List<int>> GetUserSegments(int mobileTransferCardAccountGroupItemId);

        Task<List<MobileTransferCardAccountGroupItem>> GetActiveRelations(int merchantId, UserSegmentGroup userSegmentGroup);

        Task<List<MobileTransferCardAccountGroupItem>> GetActiveRelationsWithoutUserSegmentation(int merchantId);

        Task<List<MobileTransferCardAccountGroupItem>> GetItemsAsync(int groupId);

        Task<MobileTransferCardAccountGroupItem> GetRandomRelation(int merchantId, UserSegmentGroup userSegmentGroup, bool mobile);

        Task<List<MobileTransferCardAccountGroupItem>> GetAccountGroupItems();

        Task Replace(MobileTransferCardAccountGroupItem previous);

        Task<bool> CheckPardakhtPayAccountIsBlocked(MobileTransferCardAccount account, MobileTransferCardAccountGroupItem activeItem);
    }
}

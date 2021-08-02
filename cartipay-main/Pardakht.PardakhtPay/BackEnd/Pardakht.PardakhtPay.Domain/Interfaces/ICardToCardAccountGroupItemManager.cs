using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ICardToCardAccountGroupItemManager : IBaseManager<CardToCardAccountGroupItem>
    {
        Task<List<CardToCardAccountGroupItem>> GetActiveRelations();

        Task<List<CardToCardAccountGroupItem>> GetActiveRelations(int merchantId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup);

        Task<List<CardToCardAccountGroupItem>> GetActiveRelationsWithoutUserSegmentation(int merchantId, bool? cardToCard, bool? withdrawal);

        Task<CardToCardAccountGroupItem> GetRandomRelation(int merchantId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup);

        Task<CardToCardAccountGroupItem> GetRandomRelationWithoutUserSegmentation(int merchantId, bool? cardToCard, bool? withdrawal);

        Task<CardToCardAccountGroupItem> GetCardToCardAccount(int merchantId, int cardToCardAccountId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup);

        Task<CardToCardAccountGroupItem> GetCardToCardAccountWithoutUserSegmentation(int merchantId, int cardToCardAccountId, bool? cardToCard, bool? withdrawal);

        Task<List<CardToCardAccountGroupItem>> GetItemsAsync(int groupId);

        Task<List<CardToCardAccountGroupItem>> GetAllRelations(int merchantId, bool? cardToCard, bool? withdrawal);

        Task ReplaceReservedAccount(CardToCardAccount account, CardToCardAccountGroupItemStatus status);

        Task<List<int>> GetUserSegments(int cardToCardAccountGroupItemId);

        Task<CardToCardAccountGroupItem> AddAsync(CardToCardAccountGroupItem item, List<int> userSegments);

        Task<CardToCardAccountGroupItem> UpdateAsync(CardToCardAccountGroupItem item, List<int> userSegments);

        Task CheckPausedAccounts();
    }
}

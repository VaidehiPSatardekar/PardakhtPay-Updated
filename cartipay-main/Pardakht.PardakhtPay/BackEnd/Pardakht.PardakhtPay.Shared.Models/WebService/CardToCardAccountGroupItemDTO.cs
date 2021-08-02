using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CardToCardAccountGroupItemDTO: BaseEntityDTO
    {
        public int CardToCardAccountGroupId { get; set; }

        public int CardToCardAccountId { get; set; }

        public int Status { get; set; }

        public int LoginType { get; set; }

        public bool AllowCardToCard { get; set; }

        public bool AllowWithdrawal { get; set; }

        public bool HideCardNumber { get; set; }

        public List<int> UserSegmentGroups { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class AccountGroupWithdrawalItem : BaseEntity
    {
        public int CardToCardAccountGroupId { get; set; }

        public int CardToCardAccountId { get; set; }

        public int Status { get; set; }

        [NotMapped]
        public CardToCardAccountGroupItemStatus RelationStatus
        {
            get
            {
                return (CardToCardAccountGroupItemStatus)Status;
            }
            set
            {
                Status = (int)value;
            }
        }
    }
}

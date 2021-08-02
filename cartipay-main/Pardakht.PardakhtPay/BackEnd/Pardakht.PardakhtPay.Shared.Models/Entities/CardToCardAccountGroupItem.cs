using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class CardToCardAccountGroupItem: BaseEntity
    {
        public int CardToCardAccountGroupId { get; set; }

        public int CardToCardAccountId { get; set; }

        public int Status { get; set; }

        public int LoginType { get; set; }

        public bool AllowCardToCard { get; set; }

        public bool AllowWithdrawal { get; set; }

        public bool HideCardNumber { get; set; }

        public DateTime? BlockedDate { get; set; }

        public DateTime? PausedDate { get; set; }

        public int? TempGroupItemId { get; set; }

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

    public enum CardToCardAccountGroupItemStatus
    {
        Active = 1,
        Reserved = 2,
        Blocked = 3,
        Dormant = 4,
        Paused = 5
    }
}

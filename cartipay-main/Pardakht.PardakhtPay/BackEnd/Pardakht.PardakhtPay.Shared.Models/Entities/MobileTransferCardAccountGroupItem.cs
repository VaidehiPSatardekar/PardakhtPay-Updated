using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MobileTransferCardAccountGroupItem : BaseEntity
    {
        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public int Status { get; set; }

        public MobileTransferCardAccount Account { get; set; }

        [NotMapped]
        public MobileTransferCardAccountGroupItemStatus ItemStatus
        {
            get
            {
                return (MobileTransferCardAccountGroupItemStatus)Status;
            }
            set
            {
                Status = (int)value;
            }
        }
    }

    public enum MobileTransferCardAccountGroupItemStatus
    {
        Active = 1,
        Dormant = 2,
        Blocked = 3,
        Reserved = 4
    }
}

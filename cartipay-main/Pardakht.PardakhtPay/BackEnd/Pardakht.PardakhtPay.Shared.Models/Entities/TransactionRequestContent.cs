using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class TransactionRequestContent : BaseEntity
    {
        public int TransactionId { get; set; }

        [StringLength(4000)]
        public string RequestContent { get; set; }
    }
}

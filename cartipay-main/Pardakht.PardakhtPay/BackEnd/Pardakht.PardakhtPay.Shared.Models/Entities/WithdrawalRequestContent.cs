using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class WithdrawalRequestContent : BaseEntity
    {
        public int WithdrawalId { get; set; }

        [StringLength(2000)]
        public string RequestContent { get; set; }
    }
}

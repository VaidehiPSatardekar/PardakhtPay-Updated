using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Merchant : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public string TenantGuid { get; set; }

        [Required(ErrorMessage = "Title is a mandatory field")]
        [MaxLength(2000, ErrorMessage = "Title must has lower than 2000 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Domain Address is a mandatory field")]
        [MaxLength(200, ErrorMessage = "Domain Address must has lower than 200 characters")]
        public string DomainAddress { get; set; }

        public bool IsActive { get; set; }

        //public List<Transaction> Transactions { get; set; }

        public string ApiKey { get; set; }

        public int? CardToCardAccountGroupId { get; set; }

        public int? MobileTransferAccountGroupId { get; set; }

        public string OwnerGuid { get; set; }

        public decimal? MinimumTransactionAmount { get; set; }

        public bool IsDeleted { get; set; }

        public bool UseCardtoCardPaymentForWithdrawal { get; set; }

        public bool AllowPartialPaymentForWithdrawals { get; set; }
    }
}

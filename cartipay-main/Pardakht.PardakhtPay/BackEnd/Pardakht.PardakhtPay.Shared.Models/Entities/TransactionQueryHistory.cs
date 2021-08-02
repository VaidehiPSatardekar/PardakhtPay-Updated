using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    /// <summary>
    /// Represenst a class for store transaction validation query records.
    /// Every time we request to validate for a transaction, we will store it to be able to check later
    /// </summary>
    public class TransactionQueryHistory : BaseEntity
    {
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int TransactionId { get; set; }

        [Required]
        public string RequestContent { get; set; }

        public string ResponseContent { get; set; }

        public bool IsCompleted { get; set; }
    }
}

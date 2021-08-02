using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class ManualTransferSourceCardDetails : BaseEntity
    {
        public int ManualTransferId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int CardToCardAccountId { get; set; }

        public string AccountGuid { get; set; }

       
    }

   
}

using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class WithdrawalQueryHistory : BaseEntity
    {
        public DateTime CreationDate { get; set; }

        public int WithdrawalId { get; set; }

        public string RequestContent { get; set; }

        public string ResponseContent { get; set; }
    }
}

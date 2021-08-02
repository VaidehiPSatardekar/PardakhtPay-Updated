using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class TransactionWithdrawalHistory : BaseEntity
    {
        public int TransactionId { get; set; }

        public int WithdrawalId { get; set; }

        public DateTime Date { get; set; }

        public bool IsCompleted { get; set; }

        public string Message { get; set; }
    }
}

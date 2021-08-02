﻿using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class TransactionWithdrawalRelation : BaseEntity
    {
        public int TransactionId { get; set; }

        public int WithdrawalId { get; set; }

        public DateTime Date { get; set; }
    }
}

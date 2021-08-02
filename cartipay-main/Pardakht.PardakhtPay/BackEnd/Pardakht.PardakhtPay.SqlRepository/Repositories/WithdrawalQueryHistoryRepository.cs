﻿using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class WithdrawalQueryHistoryRepository : GenericRepository<WithdrawalQueryHistory>, IWithdrawalQueryHistoryRepository
    {
        public WithdrawalQueryHistoryRepository(PardakhtPayDbContext context,
            IServiceProvider provider) :base(context, provider)
        {

        }
    }
}

﻿using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class AccountGroupWithdrawalItemRepository : GenericRepository<AccountGroupWithdrawalItem>, IAccountGroupWithdrawalItemRepository
    {
        public AccountGroupWithdrawalItemRepository(PardakhtPayDbContext context, IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}

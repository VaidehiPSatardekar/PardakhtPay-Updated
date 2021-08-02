﻿using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class InvoiceOwnerSettingRepository : GenericRepository<InvoiceOwnerSetting>, IInvoiceOwnerSettingRepository
    {
        public InvoiceOwnerSettingRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }
    }
}

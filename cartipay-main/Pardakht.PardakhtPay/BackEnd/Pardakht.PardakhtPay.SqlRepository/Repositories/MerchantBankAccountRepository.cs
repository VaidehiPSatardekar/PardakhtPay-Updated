//using System;
//using System.Collections.Generic;
//using System.Text;
//using Pardakht.PardakhtPay.Infrastructure.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace Pardakht.PardakhtPay.SqlRepository.Repositories
//{
//    public class MerchantBankAccountRepository : GenericRepository<MerchantBankAccount>, IMerchantBankAccountRepository
//    {
//        public MerchantBankAccountRepository(PardakhtPayDbContext context,
//            IServiceProvider provider) : base(context, provider)
//        {

//        }

//        public MerchantBankAccount GetAccountByApiKey(string apiKey)
//        {
//            return base.GetQuery(p => p.ApiKey == apiKey && p.IsActive).FirstOrDefault();
//        }

//        public async Task<List<int>> GetBankAccountIds(int merchantId)
//        {
//            throw new NotImplementedException();
//            //return await Task.Run(() =>
//            //{
//            //    return Context.MerchantBankAccounts.Where(t => t.MerchantId == merchantId).AsNoTracking().Select(t => t.Id).ToList();

//            //});
//        }
//    }
//}

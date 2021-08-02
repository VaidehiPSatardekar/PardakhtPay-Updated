//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Domain.Base;
//using Pardakht.PardakhtPay.Domain.Interfaces;
//using Pardakht.PardakhtPay.Infrastructure.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using System.Linq;

//namespace Pardakht.PardakhtPay.Domain.Managers
//{
//    public class MerchantBankAccountManager : BaseManager<MerchantBankAccount, IMerchantBankAccountRepository>, IMerchantBankAccountManager
//    {
//        public MerchantBankAccountManager(IMerchantBankAccountRepository repository):base(repository)
//        {

//        }

//        public MerchantBankAccount GetAccountByApiKey(string apiKey)
//        {
//            return Repository.GetAccountByApiKey(apiKey);
//        }

//        public async Task<List<int>> GetBankAccountIds(int merchantId)
//        {
//            return await Repository.GetBankAccountIds(merchantId);
//        }

//        public async Task<List<MerchantBankAccount>> GetByMerchantId(int merchantId)
//        {
//            return await Repository.GetItemsAsync(t => t.MerchantId == merchantId);
//        }
//    }
//}

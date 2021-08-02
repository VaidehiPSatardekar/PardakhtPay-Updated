using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MerchantManager : BaseManager<Merchant, IMerchantRepository>, IMerchantManager
    {
        IAesEncryptionService _EncryptionService = null;

        public MerchantManager(IMerchantRepository repository,
            IAesEncryptionService encryptionService) :base(repository)
        {
            _EncryptionService = encryptionService;
        }

        public async Task<Merchant> GetMerchantByApiKey(string key)
        {
            var item = await Repository.GetItemAsync(t => t.ApiKey == key);

            return item;
        }

        public async Task<Merchant> GetMerchantByClearApiKey(string key)
        {
            string encrypted = _EncryptionService.EncryptToBase64(key);
            
            var item = await Repository.GetItemAsync(t => t.ApiKey == encrypted);

            return item;
        }

        public async Task<IEnumerable<Merchant>> Search(string term)
        {
            term = term.ToLower();

            var result = await Repository.GetItemsAsync(q => q.Title.ToLower().Contains(term));
            var response = AutoMapper.Mapper.Map<IEnumerable<Merchant>>(result);

            return response;
        }
    }
}

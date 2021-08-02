//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Domain.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.Application.Interfaces
//{
//    public interface IMerchantBankAccountService : IServiceBase<MerchantBankAccount, IMerchantBankAccountManager>
//    {
//        Task<WebResponse<List<MerchantBankAccountDTO>>> GetAllItemsAsync();

//        Task<WebResponse<MerchantBankAccountDTO>> InsertAsync(MerchantBankAccountDTO item);

//        Task<WebResponse<MerchantBankAccountDTO>> UpdateAsync(MerchantBankAccountDTO item);

//        Task<WebResponse<MerchantBankAccountDTO>> GetItemById(int id);

//        Task<WebResponse<List<MerchantBankAccountDTO>>> GetItemsByMerchantId(int merchantId);
//    }
//}

using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IMerchantCustomerService : IServiceBase<MerchantCustomer, IMerchantCustomerManager>
    {
        Task<WebResponse<List<MerchantCustomerPhoneNumberDTO>>> GetPhoneNumberRelatedCustomer(string phoneNumber);

        Task<WebResponse<List<MerchantCustomerRelationDTO>>> GetRelatedCustomers(int merchantCustomerId);

        Task<WebResponse<List<MerchantCardNumbersDTO>>> GetCardNumberCounts(int merchantCustomerId);

        Task<WebResponse<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>>> Search(MerchantCustomerSearchArgs args);

        Task<WebResponse<MerchantCustomerDTO>> UpdateUserSegmentGroup(MerchantCustomerDTO customer);

        Task<WebResponse<MerchantCustomerDTO>> GetCustomerAsync(int id);

        Task<WebResponse> ReplaceUserId(ReplaceUserIdRequest request);

        Task<WebResponse<PhoneNumbersResponse>> ExportPhoneNumbers(MerchantCustomerSearchArgs args);
        Task<WebResponse<List<RegisteredPhoneNumbers>>> GetRegisteredPhones(int id);

        Task<WebResponse> RemoveRegisteredPhones(int id, RegisteredPhoneNumbersList phoneNumbers);
        

    }
}

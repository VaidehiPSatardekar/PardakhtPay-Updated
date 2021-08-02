using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMerchantCustomerManager: IBaseManager<MerchantCustomer>
    {
        Task<MerchantCustomer> AddOrUpdateCustomer(MerchantCustomer customer);

        Task<MerchantCustomerAccount> GetCardToCardTransferAccounts(MerchantCustomer item, int merchantId);

        Task<MerchantCustomerMobileAccount> GetCardToCardTransferAccountsForMobileTransfer(MerchantCustomer item, int merchantId, bool mobile);

        Task<MerchantCustomerAccount> GetCardToCardAccount(int merchantId, int? cardToCardAccountId, bool? cardToCard, bool? withdrawal, UserSegmentGroup userSegmentGroup);

        Task<List<MerchantCustomerPhoneNumberDTO>> GetPhoneNumberRelatedCustomer(string phoneNumber);

        Task<List<MerchantCustomerRelationDTO>> GetRelatedCustomers(int merchantCustomerId);

        Task<List<MerchantCardNumbersDTO>> GetCardNumberCounts(int merchantCustomerId);

        Task<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>> Search(MerchantCustomerSearchArgs args);

        Task<MerchantCustomerAccount> GetWithdrawAccountForCustomer(int? merchantCustomerId, int merchantId);

        Task<MerchantCustomer> UpdateUserSegmentGroup(MerchantCustomer customer);

        Task<MerchantCustomerDTO> GetCustomerAsync(int id);

        Task<MerchantCustomer> GetCustomerAsync(string ownerGuid, string websiteName, string userId);

        Task<DeviceMerchantCustomerRelation> AddDeviceMerchantCustomerRelation(DeviceMerchantCustomerRelation relation);

        Task<PhoneNumbersResponse> ExportPhoneNumbers(MerchantCustomerSearchArgs args);

        Task<List<RegisteredPhoneNumbers>> GetRegisteredPhones(int id);

        Task RemoveRegisteredPhones(int id,RegisteredPhoneNumbersList phoneNumbers);
    }
}

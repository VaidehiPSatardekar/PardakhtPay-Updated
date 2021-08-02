using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MerchantCustomerService : DatabaseServiceBase<MerchantCustomer, IMerchantCustomerManager>, IMerchantCustomerService
    {
        IMerchantManager _MerchantManager = null;
        CurrentUser _CurrentUser = null;

        public MerchantCustomerService(IMerchantCustomerManager manager,
            ILogger<MerchantCustomerService> logger,
            IMerchantManager merchantManager,
            CurrentUser currentUser):base(manager, logger)
        {
            _CurrentUser = currentUser;
            _MerchantManager = merchantManager;
        }

        public async Task<WebResponse<MerchantCustomerDTO>> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await Manager.GetCustomerAsync(id);

                return new WebResponse<MerchantCustomerDTO>(customer);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<MerchantCustomerDTO>(ex);
            }
        }

        public async Task<WebResponse<List<MerchantCustomerPhoneNumberDTO>>> GetPhoneNumberRelatedCustomer(string phoneNumber)
        {
            try
            {
                var items = await Manager.GetPhoneNumberRelatedCustomer(phoneNumber);

                return new WebResponse<List<MerchantCustomerPhoneNumberDTO>>(items);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<MerchantCustomerPhoneNumberDTO>>(ex);
            }
        }

        public async Task<WebResponse<List<MerchantCustomerRelationDTO>>> GetRelatedCustomers(int merchantCustomerId)
        {
            try
            {
                var items = await Manager.GetRelatedCustomers(merchantCustomerId);

                return new WebResponse<List<MerchantCustomerRelationDTO>>(items);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<MerchantCustomerRelationDTO>>(ex);
            }
        }

        public async Task<WebResponse<List<MerchantCardNumbersDTO>>> GetCardNumberCounts(int merchantCustomerId)
        {
            try
            {
                var items = await Manager.GetCardNumberCounts(merchantCustomerId);

                return new WebResponse<List<MerchantCardNumbersDTO>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<MerchantCardNumbersDTO>>(ex);
            }
        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>>> Search(MerchantCustomerSearchArgs args)
        {
            try
            {
                var response = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>>(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return new WebResponse<ListSearchResponse<IEnumerable<MerchantCustomerDTO>>>(ex);
            }
        }

        public async Task<WebResponse<MerchantCustomerDTO>> UpdateUserSegmentGroup(MerchantCustomerDTO customer)
        {
            try
            {
                var item = AutoMapper.Mapper.Map<MerchantCustomer>(customer);

                var response = await Manager.UpdateUserSegmentGroup(item);

                var dto = AutoMapper.Mapper.Map<MerchantCustomerDTO>(response);

                return new WebResponse<MerchantCustomerDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return new WebResponse<MerchantCustomerDTO>(ex);
            }
        }

        public async Task<WebResponse> ReplaceUserId(ReplaceUserIdRequest request)
        {
            try
            {
                var merchant = await _MerchantManager.GetMerchantByClearApiKey(request.ApiKey);

                var item = await Manager.GetCustomerAsync(merchant?.OwnerGuid, request.WebsiteName, request.OldUserId);

                if(item != null)
                {
                    item.UserId = request.NewUserId;

                    await Manager.UpdateAsync(item);
                    await Manager.SaveAsync();
                }

                return new WebResponse()
                {
                    Success = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse<PhoneNumbersResponse>> ExportPhoneNumbers(MerchantCustomerSearchArgs args)
        {
            try
            {
                var response = await Manager.ExportPhoneNumbers(args);

                return new WebResponse<PhoneNumbersResponse>(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return new WebResponse<PhoneNumbersResponse>(ex);
            }
        }

        public async Task<WebResponse<List<RegisteredPhoneNumbers>>> GetRegisteredPhones(int id)
        {
            try
            {
                var registeredPhoneNumbers = await Manager.GetRegisteredPhones(id);

                return new WebResponse<List<RegisteredPhoneNumbers>>(registeredPhoneNumbers);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<RegisteredPhoneNumbers>>(ex);
            }
        }

        public async Task<WebResponse> RemoveRegisteredPhones(int id, RegisteredPhoneNumbersList phoneNumbers)
        {
            try
            {
                await Manager.RemoveRegisteredPhones(id, phoneNumbers);

                return new WebResponse()
                {
                    Success = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }
    }
}

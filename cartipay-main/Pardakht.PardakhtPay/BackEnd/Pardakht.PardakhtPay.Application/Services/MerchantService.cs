using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MerchantService : DatabaseServiceBase<Merchant, IMerchantManager>, IMerchantService
    {
        IAesEncryptionService _AesEncryptionService = null;
        IUserSegmentGroupManager _UserSegmentGroupManager = null;
        JwtIssuerOptions _JwtOptions = null;

        public MerchantService(IMerchantManager manager,
            //IMerchantBankAccountManager merchantBankAccountManager,
            //IMerchantBankAccountService merchantBankAccountService,
            ILogger<MerchantService> logger,
            IUserSegmentGroupManager userSegmentGroupManager,
            IOptions<JwtIssuerOptions> jwtOptions,
            IAesEncryptionService encryptionService):base(manager, logger)
        {
            _AesEncryptionService = encryptionService;
            //_MerchantBankAccountManager = merchantBankAccountManager;
            //_MerchantBankAccountService = merchantBankAccountService;
            _JwtOptions = jwtOptions.Value;
            _UserSegmentGroupManager = userSegmentGroupManager;
        }

        public async Task<WebResponse<List<MerchantDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<MerchantDTO>>(result.Payload);

                dtos.ForEach(t =>
                {
                    t.ApiKey = _AesEncryptionService.DecryptToString(t.ApiKey);
                });

                return new WebResponse<List<MerchantDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<MerchantDTO>>(ex);
            }
        }

        public async Task<WebResponse<MerchantUpdateDTO>> GetMerchantById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<MerchantUpdateDTO>(result.Payload);

                dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);

                return new WebResponse<MerchantUpdateDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<MerchantUpdateDTO>(ex);
            }
        }

        public async Task<WebResponse<MerchantDTO>> InsertAsync(MerchantDTO item)
        {
            try
            {
                var merchant = AutoMapper.Mapper.Map<Merchant>(item);
                merchant.ApiKey = _AesEncryptionService.EncryptToBase64(merchant.ApiKey);

                var result = await Manager.AddAsync(merchant);

                await Manager.SaveAsync();

                await _UserSegmentGroupManager.CreateDefaultUserSegmentGroups(item.TenantGuid, item.OwnerGuid);

                var dto = AutoMapper.Mapper.Map<MerchantDTO>(result);

                dto.ApiKey = item.ApiKey;

                return new WebResponse<MerchantDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MerchantDTO>(ex);
            }
        }

        public async Task<WebResponse<MerchantCreateDTO>> InsertWithAccountsAsync(MerchantCreateDTO item)
        {
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                try
                {

                    var merchant = AutoMapper.Mapper.Map<Merchant>(item);

                    merchant.ApiKey = _AesEncryptionService.EncryptToBase64(item.ApiKey);

                    var result = await Manager.AddAsync(merchant);

                    await Manager.SaveAsync();

                    await _UserSegmentGroupManager.CreateDefaultUserSegmentGroups(item.TenantGuid, item.OwnerGuid);

                    var dto = AutoMapper.Mapper.Map<MerchantCreateDTO>(result);

                    //scope.Complete();

                    dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);
                    return new WebResponse<MerchantCreateDTO>(dto);

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return new WebResponse<MerchantCreateDTO>(ex);
                }
            //}
        }

        public async Task<WebResponse<MerchantUpdateDTO>> UpdateWithAccountsAsync(MerchantUpdateDTO item)
        {
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                try
                {
                    var merchant = AutoMapper.Mapper.Map<Merchant>(item);

                    merchant.ApiKey = _AesEncryptionService.EncryptToBase64(item.ApiKey);

                    var result = await Manager.UpdateAsync(merchant);

                    await Manager.SaveAsync();

                    await _UserSegmentGroupManager.CreateDefaultUserSegmentGroups(item.TenantGuid, item.OwnerGuid);

                    var dto = AutoMapper.Mapper.Map<MerchantUpdateDTO>(result);

                    //scope.Complete();

                    dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);
                    return new WebResponse<MerchantUpdateDTO>(dto);

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return new WebResponse<MerchantUpdateDTO>(ex);
                }
            //}
        }

        public async Task<WebResponse<MerchantDTO>> UpdateAsync(MerchantDTO item)
        {
            try
            {
                var oldMerchant = await Manager.GetEntityByIdAsync(item.Id);

                var merchant = AutoMapper.Mapper.Map<Merchant>(item);

                merchant.ApiKey = _AesEncryptionService.EncryptToBase64(item.ApiKey);

                var result = await Manager.UpdateAsync(merchant);

                await Manager.SaveAsync();

                await _UserSegmentGroupManager.CreateDefaultUserSegmentGroups(item.TenantGuid, item.OwnerGuid);

                var dto = AutoMapper.Mapper.Map<MerchantDTO>(result);

                dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);

                return new WebResponse<MerchantDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<MerchantDTO>(ex);
            }
        }

        public override async Task<WebResponse> DeleteAsync(int id)
        {
            try
            {
                await Manager.SoftDeleteAsync(id);

                await Manager.SaveAsync();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse<IEnumerable<Merchant>>> Search(string term)
        {
            var result = await Manager.Search(term);

            return new WebResponse<IEnumerable<Merchant>>
            {
                Payload = result,
                Success = result != null,
                Message = result == null ? "Something went wrong.." : string.Empty
            };
        }
    }
}

//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Domain.Interfaces;
//using Pardakht.PardakhtPay.Shared.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.Application.Services
//{
//    public class MerchantBankAccountService : DatabaseServiceBase<MerchantBankAccount, IMerchantBankAccountManager>, IMerchantBankAccountService
//    {
//        IAesEncryptionService _AesEncryptionService = null;

//        public MerchantBankAccountService(IMerchantBankAccountManager manager,
//            ILogger<MerchantBankAccountService> logger,
//            IAesEncryptionService encryptionService) :base(manager, logger)
//        {
//            _AesEncryptionService = encryptionService;
//        }

//        public async Task<WebResponse<List<MerchantBankAccountDTO>>> GetAllItemsAsync()
//        {
//            try
//            {
//                var result = await GetAllAsync();

//                if (!result.Success)
//                {
//                    throw new Exception(result.Message);
//                }

//                var dtos = AutoMapper.Mapper.Map<List<MerchantBankAccountDTO>>(result.Payload);

//                dtos.ForEach(dto =>
//                {
//                    dto.CardNumber = _AesEncryptionService.DecryptToString(dto.CardNumber);
//                    dto.AccountNumber = _AesEncryptionService.DecryptToString(dto.AccountNumber);
//                    dto.BusinessAccountNumber = _AesEncryptionService.DecryptToString(dto.BusinessAccountNumber);
//                    dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);
//                });

//                return new WebResponse<List<MerchantBankAccountDTO>>(true, string.Empty, dtos);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<List<MerchantBankAccountDTO>>(ex);
//            }
//        }

//        public async Task<WebResponse<MerchantBankAccountDTO>> GetItemById(int id)
//        {
//            try
//            {
//                var result = await GetEntityByIdAsync(id);

//                if (!result.Success)
//                {
//                    throw result.Exception;
//                }

//                var dto = AutoMapper.Mapper.Map<MerchantBankAccountDTO>(result.Payload);

//                dto.CardNumber = _AesEncryptionService.DecryptToString(dto.CardNumber);
//                dto.AccountNumber = _AesEncryptionService.DecryptToString(dto.AccountNumber);
//                dto.BusinessAccountNumber = _AesEncryptionService.DecryptToString(dto.BusinessAccountNumber);
//                dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);

//                return new WebResponse<MerchantBankAccountDTO>(dto);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<MerchantBankAccountDTO>(ex);
//            }
//        }

//        public async Task<WebResponse<List<MerchantBankAccountDTO>>> GetItemsByMerchantId(int merchantId)
//        {
//            try
//            {
//                var accounts = await Manager.GetByMerchantId(merchantId);

//                var dtos = new List<MerchantBankAccountDTO>();

//                accounts.ForEach(account =>
//                {
//                    var dto = AutoMapper.Mapper.Map<MerchantBankAccountDTO>(account);

//                    dto.AccountNumber = _AesEncryptionService.DecryptToString(dto.AccountNumber);
//                    dto.ApiKey = _AesEncryptionService.DecryptToString(dto.ApiKey);
//                    dto.BusinessAccountNumber = _AesEncryptionService.DecryptToString(dto.BusinessAccountNumber);
//                    dto.CardNumber = _AesEncryptionService.DecryptToString(dto.CardNumber);

//                    dtos.Add(dto);
//                });

//                return new WebResponse<List<MerchantBankAccountDTO>>(dtos);
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex, ex.Message);

//                return new WebResponse<List<MerchantBankAccountDTO>>(ex);
//            }
//        }

//        public async Task<WebResponse<MerchantBankAccountDTO>> InsertAsync(MerchantBankAccountDTO item)
//        {
//            try
//            {
//                var merchant = AutoMapper.Mapper.Map<MerchantBankAccount>(item);

//                merchant.CardNumber = _AesEncryptionService.EncryptToBase64(merchant.CardNumber);
//                merchant.AccountNumber = _AesEncryptionService.EncryptToBase64(merchant.AccountNumber);
//                merchant.BusinessAccountNumber = _AesEncryptionService.EncryptToBase64(merchant.BusinessAccountNumber);
//                merchant.ApiKey = _AesEncryptionService.EncryptToBase64(merchant.ApiKey);

//                var result = await Manager.AddAsync(merchant);

//                await Manager.SaveAsync();

//                var dto = AutoMapper.Mapper.Map<MerchantBankAccountDTO>(result);

//                dto.CardNumber = _AesEncryptionService.DecryptToString(merchant.CardNumber);
//                dto.AccountNumber = _AesEncryptionService.DecryptToString(merchant.AccountNumber);
//                dto.BusinessAccountNumber = _AesEncryptionService.DecryptToString(merchant.BusinessAccountNumber);
//                dto.ApiKey = _AesEncryptionService.DecryptToString(merchant.ApiKey);

//                return new WebResponse<MerchantBankAccountDTO>(true, string.Empty, dto);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<MerchantBankAccountDTO>(ex);
//            }
//        }

//        public async Task<WebResponse<MerchantBankAccountDTO>> UpdateAsync(MerchantBankAccountDTO item)
//        {
//            try
//            {
//                var oldMerchant = await Manager.GetEntityByIdAsync(item.Id);

//                var merchant = AutoMapper.Mapper.Map<MerchantBankAccount>(item);

//                merchant.CardNumber = _AesEncryptionService.EncryptToBase64(item.CardNumber);
//                merchant.AccountNumber = _AesEncryptionService.EncryptToBase64(item.AccountNumber);
//                merchant.BusinessAccountNumber = _AesEncryptionService.EncryptToBase64(item.BusinessAccountNumber);
//                merchant.ApiKey = _AesEncryptionService.EncryptToBase64(item.ApiKey);

//                var result = await Manager.UpdateAsync(merchant);

//                await Manager.SaveAsync();

//                var dto = AutoMapper.Mapper.Map<MerchantBankAccountDTO>(result);

//                dto.CardNumber = _AesEncryptionService.DecryptToString(merchant.CardNumber);
//                dto.AccountNumber = _AesEncryptionService.DecryptToString(merchant.AccountNumber);
//                dto.BusinessAccountNumber = _AesEncryptionService.DecryptToString(merchant.BusinessAccountNumber);
//                dto.ApiKey = _AesEncryptionService.DecryptToString(merchant.ApiKey);

//                return new WebResponse<MerchantBankAccountDTO>(dto);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<MerchantBankAccountDTO>(ex);
//            }
//        }
//    }
//}

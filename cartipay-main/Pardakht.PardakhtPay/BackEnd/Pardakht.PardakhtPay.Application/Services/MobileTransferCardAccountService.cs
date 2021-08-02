using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobileTransferCardAccountService: DatabaseServiceBase<MobileTransferCardAccount, IMobileTransferCardAccountManager>, IMobileTransferCardAccountService
    {
        IAesEncryptionService _EncryptionService = null;

        public MobileTransferCardAccountService(IMobileTransferCardAccountManager manager,
            ILogger<MobileTransferCardAccountService> logger,
            IAesEncryptionService aesEncryptionService):base(manager, logger)
        {
            _EncryptionService = aesEncryptionService;
        }

        public async Task<WebResponse<List<MobileTransferCardAccountDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<MobileTransferCardAccountDTO>>(result.Payload);

                for (int i = 0; i < dtos.Count; i++)
                {
                    dtos[i].CardNumber = _EncryptionService.DecryptToString(dtos[i].CardNumber);
                    dtos[i].CardHolderName = _EncryptionService.DecryptToString(dtos[i].CardHolderName);
                }

                return new WebResponse<List<MobileTransferCardAccountDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<MobileTransferCardAccountDTO>>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<MobileTransferCardAccountDTO>(result.Payload);

                dto.CardNumber = _EncryptionService.DecryptToString(dto.CardNumber);
                dto.CardHolderName = _EncryptionService.DecryptToString(dto.CardHolderName);

                return new WebResponse<MobileTransferCardAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountDTO>> InsertAsync(MobileTransferCardAccountDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferCardAccount>(item);
                
                entity.IsDeleted = false;
                entity.CardNumber = _EncryptionService.EncryptToBase64(item.CardNumber);
                entity.CardHolderName = _EncryptionService.EncryptToBase64(item.CardHolderName);

                if (entity.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
                {
                    entity.MerchantId = string.Empty;
                    entity.Title = string.Empty;
                    entity.MerchantPassword = string.Empty;
                    entity.TerminalId = string.Empty;
                }
                else
                {
                    entity.CardHolderName = string.Empty;
                    entity.CardNumber = string.Empty;
                }

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<MobileTransferCardAccountDTO>(result);

                dto.CardHolderName = item.CardHolderName;
                dto.CardNumber = item.CardNumber;

                return new WebResponse<MobileTransferCardAccountDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountDTO>> UpdateAsync(MobileTransferCardAccountDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferCardAccount>(item);
                entity.CardNumber = _EncryptionService.EncryptToBase64(item.CardNumber);
                entity.CardHolderName = _EncryptionService.EncryptToBase64(item.CardHolderName);

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<MobileTransferCardAccountDTO>(result);

                dto.CardHolderName = item.CardHolderName;
                dto.CardNumber = item.CardNumber;

                return new WebResponse<MobileTransferCardAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountDTO>(ex);
            }
        }
    }
}

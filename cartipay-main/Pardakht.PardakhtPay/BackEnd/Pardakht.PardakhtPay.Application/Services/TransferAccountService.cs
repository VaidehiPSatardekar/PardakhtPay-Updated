using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class TransferAccountService : DatabaseServiceBase<TransferAccount, ITransferAccountManager>, ITransferAccountService
    {
        IAesEncryptionService _EncryptionService = null;
        public TransferAccountService(ITransferAccountManager manager,
            ILogger<TransferAccountService> logger,
            IAesEncryptionService aesEncryptionService):base(manager, logger)
        {
            _EncryptionService = aesEncryptionService;
        }

        public async Task<WebResponse<List<TransferAccountDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<TransferAccountDTO>>(result.Payload);

                dtos.ForEach(dto =>
                {
                    dto.AccountNo = _EncryptionService.DecryptToString(dto.AccountNo);

                    if (!string.IsNullOrEmpty(dto.AccountHolderFirstName))
                    {
                        dto.AccountHolderFirstName = _EncryptionService.DecryptToString(dto.AccountHolderFirstName);
                    }

                    if (!string.IsNullOrEmpty(dto.AccountHolderLastName))
                    {
                        dto.AccountHolderLastName = _EncryptionService.DecryptToString(dto.AccountHolderLastName);
                    }

                    if (!string.IsNullOrEmpty(dto.Iban))
                    {
                        dto.Iban = _EncryptionService.DecryptToString(dto.Iban);
                    }
                });

                return new WebResponse<List<TransferAccountDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<TransferAccountDTO>>(ex);
            }
        }

        public async Task<WebResponse<TransferAccountDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<TransferAccountDTO>(result.Payload);

                dto.AccountNo = _EncryptionService.DecryptToString(dto.AccountNo);

                if (!string.IsNullOrEmpty(dto.AccountHolderFirstName))
                {
                    dto.AccountHolderFirstName = _EncryptionService.DecryptToString(dto.AccountHolderFirstName);
                }

                if (!string.IsNullOrEmpty(dto.AccountHolderLastName))
                {
                    dto.AccountHolderLastName = _EncryptionService.DecryptToString(dto.AccountHolderLastName);
                }

                if (!string.IsNullOrEmpty(dto.Iban))
                {
                    dto.Iban = _EncryptionService.DecryptToString(dto.Iban);
                }

                return new WebResponse<TransferAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<TransferAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<TransferAccountDTO>> InsertAsync(TransferAccountDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<TransferAccount>(item);

                entity.AccountNo = _EncryptionService.EncryptToBase64(item.AccountNo);

                var old = await Manager.GetItemAsync(t => t.AccountNo == entity.AccountNo && t.OwnerGuid == item.OwnerGuid);

                if(old != null)
                {
                    throw new Exception("This account no is used");
                }

                if (!string.IsNullOrEmpty(entity.AccountHolderFirstName))
                {
                    entity.AccountHolderFirstName = _EncryptionService.EncryptToBase64(item.AccountHolderFirstName);
                }

                if (!string.IsNullOrEmpty(entity.AccountHolderLastName))
                {
                    entity.AccountHolderLastName = _EncryptionService.EncryptToBase64(item.AccountHolderLastName);
                }

                if (!string.IsNullOrEmpty(entity.Iban))
                {
                    entity.Iban = _EncryptionService.EncryptToBase64(item.Iban);
                }

                old = await Manager.GetItemAsync(t => t.Iban == entity.Iban && t.OwnerGuid == item.OwnerGuid);

                if (old != null)
                {
                    throw new Exception("This iban is used");
                }

                old = await Manager.GetItemAsync(t => t.FriendlyName == entity.FriendlyName && t.OwnerGuid == item.OwnerGuid);

                if (old != null)
                {
                    throw new Exception("This friendly name is used");
                }

                entity.CreationDate = DateTime.UtcNow;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<TransferAccountDTO>(result);

                dto.AccountNo = _EncryptionService.DecryptToString(dto.AccountNo);

                if (!string.IsNullOrEmpty(dto.AccountHolderFirstName))
                {
                    dto.AccountHolderFirstName = _EncryptionService.DecryptToString(dto.AccountHolderFirstName);
                }

                if (!string.IsNullOrEmpty(dto.AccountHolderLastName))
                {
                    dto.AccountHolderLastName = _EncryptionService.DecryptToString(dto.AccountHolderLastName);
                }

                if (!string.IsNullOrEmpty(dto.Iban))
                {
                    dto.Iban = _EncryptionService.DecryptToString(dto.Iban);
                }

                return new WebResponse<TransferAccountDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<TransferAccountDTO>(ex);
            }
        }

        public async Task<WebResponse<TransferAccountDTO>> UpdateAsync(TransferAccountDTO item)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(item.Id);

                var entity = AutoMapper.Mapper.Map<TransferAccount>(item);

                entity.AccountNo = _EncryptionService.EncryptToBase64(item.AccountNo);

                var old = await Manager.GetItemAsync(t => t.Id != item.Id && t.AccountNo == entity.AccountNo && t.OwnerGuid == item.OwnerGuid);

                if (old != null)
                {
                    throw new Exception("This account no is used");
                }

                if (!string.IsNullOrEmpty(item.AccountHolderFirstName))
                {
                    entity.AccountHolderFirstName = _EncryptionService.EncryptToBase64(item.AccountHolderFirstName);
                }

                if (!string.IsNullOrEmpty(item.AccountHolderLastName))
                {
                    entity.AccountHolderLastName = _EncryptionService.EncryptToBase64(item.AccountHolderLastName);
                }

                if (!string.IsNullOrEmpty(item.Iban))
                {
                    entity.Iban = _EncryptionService.EncryptToBase64(item.Iban);
                }



                old = await Manager.GetItemAsync(t => t.Id != item.Id && t.Iban == entity.Iban && t.OwnerGuid == item.OwnerGuid);

                if (old != null)
                {
                    throw new Exception("This iban is used");
                }

                old = await Manager.GetItemAsync(t => t.Id != item.Id && t.FriendlyName == entity.FriendlyName && t.OwnerGuid == item.OwnerGuid);

                if (old != null)
                {
                    throw new Exception("This friendly name is used");
                }

                entity.CreationDate = oldEntity.CreationDate;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<TransferAccountDTO>(result);

                dto.AccountNo = _EncryptionService.DecryptToString(dto.AccountNo);

                if (!string.IsNullOrEmpty(dto.AccountHolderFirstName))
                {
                    dto.AccountHolderFirstName = _EncryptionService.DecryptToString(dto.AccountHolderFirstName);
                }

                if (!string.IsNullOrEmpty(dto.AccountHolderLastName))
                {
                    dto.AccountHolderLastName = _EncryptionService.DecryptToString(dto.AccountHolderLastName);
                }

                if (!string.IsNullOrEmpty(dto.Iban))
                {
                    dto.Iban = _EncryptionService.DecryptToString(dto.Iban);
                }

                return new WebResponse<TransferAccountDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<TransferAccountDTO>(ex);
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
                Logger.LogError(ex, ex.Message);
                return new WebResponse(ex);
            }
        }
    }
}


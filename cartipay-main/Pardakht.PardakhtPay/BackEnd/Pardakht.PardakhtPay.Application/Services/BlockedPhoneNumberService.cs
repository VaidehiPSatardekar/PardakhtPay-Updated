using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class BlockedPhoneNumberService : DatabaseServiceBase<BlockedPhoneNumber, IBlockedPhoneNumberManager>, IBlockedPhoneNumberService
    {
        CurrentUser _CurrentUser = null;

        public BlockedPhoneNumberService(IBlockedPhoneNumberManager manager,
            ILogger<BlockedPhoneNumberService> logger,
            CurrentUser currentUser):base(manager, logger)
        {
            _CurrentUser = currentUser;
        }

        public async Task<WebResponse<List<BlockedPhoneNumberDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<BlockedPhoneNumberDTO>>(result.Payload);

                dtos.ForEach(dto =>
                {
                    if (!_CurrentUser.HasRole(Permissions.UnmaskedPhoneNumbers))
                    {
                        if (!string.IsNullOrEmpty(dto.PhoneNumber))
                        {
                            dto.PhoneNumber = dto.PhoneNumber.GetMaskedPhoneNumber();
                        }
                    }
                });

                return new WebResponse<List<BlockedPhoneNumberDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<BlockedPhoneNumberDTO>>(ex);
            }
        }

        public async Task<WebResponse<BlockedPhoneNumberDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<BlockedPhoneNumberDTO>(result.Payload);

                return new WebResponse<BlockedPhoneNumberDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedPhoneNumberDTO>(ex);
            }
        }

        public async Task<WebResponse<BlockedPhoneNumberDTO>> InsertAsync(BlockedPhoneNumberDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BlockedPhoneNumber>(item);
                entity.BlockedDate = DateTime.UtcNow;
                entity.InsertUserId = _CurrentUser.IdentifierGuid;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<BlockedPhoneNumberDTO>(result);

                return new WebResponse<BlockedPhoneNumberDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedPhoneNumberDTO>(ex);
            }
        }

        public async Task<WebResponse<BlockedPhoneNumberDTO>> UpdateAsync(BlockedPhoneNumberDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BlockedPhoneNumber>(item);
                entity.BlockedDate = DateTime.UtcNow;
                entity.InsertUserId = _CurrentUser.IdentifierGuid;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<BlockedPhoneNumberDTO>(AutoMapper.Mapper.Map<BlockedPhoneNumberDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedPhoneNumberDTO>(ex);
            }
        }
    }
}

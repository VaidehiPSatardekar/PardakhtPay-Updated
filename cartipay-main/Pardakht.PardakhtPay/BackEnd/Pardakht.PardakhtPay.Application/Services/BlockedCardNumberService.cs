using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class BlockedCardNumberService : DatabaseServiceBase<BlockedCardNumber, IBlockedCardNumberManager>, IBlockedCardNumberService
    {
        CurrentUser _CurrentUser = null;

        public BlockedCardNumberService(IBlockedCardNumberManager manager,
            ILogger<BlockedCardNumberService> logger,
            CurrentUser currentUser) : base(manager, logger)
        {
            _CurrentUser = currentUser;
        }

        public async Task<WebResponse<List<BlockedCardNumberDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<BlockedCardNumberDTO>>(result.Payload);

                return new WebResponse<List<BlockedCardNumberDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<BlockedCardNumberDTO>>(ex);
            }
        }

        public async Task<WebResponse<BlockedCardNumberDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<BlockedCardNumberDTO>(result.Payload);

                return new WebResponse<BlockedCardNumberDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedCardNumberDTO>(ex);
            }
        }

        public async Task<WebResponse<BlockedCardNumberDTO>> InsertAsync(BlockedCardNumberDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BlockedCardNumber>(item);
                entity.BlockedDate = DateTime.UtcNow;
                entity.InsertUserId = _CurrentUser.IdentifierGuid;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<BlockedCardNumberDTO>(result);

                return new WebResponse<BlockedCardNumberDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedCardNumberDTO>(ex);
            }
        }

        public async Task<WebResponse<BlockedCardNumberDTO>> UpdateAsync(BlockedCardNumberDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BlockedCardNumber>(item);
                entity.BlockedDate = DateTime.UtcNow;
                entity.InsertUserId = _CurrentUser.IdentifierGuid;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<BlockedCardNumberDTO>(AutoMapper.Mapper.Map<BlockedCardNumberDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<BlockedCardNumberDTO>(ex);
            }
        }
    }
}


using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class BankStatementItemService : DatabaseServiceBase<BankStatementItem, IBankStatementItemManager>, IBankStatementItemService
    {
        public BankStatementItemService(IBankStatementItemManager manager, ILogger<BankStatementItemService> logger):base(manager, logger)
        {

        }

        public async Task<WebResponse<List<BankStatementItemDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<BankStatementItemDTO>>(result.Payload);

                return new WebResponse<List<BankStatementItemDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<BankStatementItemDTO>>(ex);
            }
        }

        public async Task<WebResponse<BankStatementItemDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<BankStatementItemDTO>(result.Payload);

                return new WebResponse<BankStatementItemDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<BankStatementItemDTO>(ex);
            }
        }

        public async Task<WebResponse<BankStatementItemDTO>> InsertAsync(BankStatementItemDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BankStatementItem>(item);
                entity.CreationDate = DateTime.UtcNow;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<BankStatementItemDTO>(result);

                return new WebResponse<BankStatementItemDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<BankStatementItemDTO>(ex);
            }
        }

        public async Task<WebResponse> InsertAsync(List<BankStatementItemDTO> items)
        {
            try
            {
                foreach (var item in items)
                {
                    var entity = AutoMapper.Mapper.Map<BankStatementItem>(item);
                    entity.CreationDate = DateTime.UtcNow;

                    var result = await Manager.AddAsync(entity);
                }

                await Manager.SaveAsync();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse(ex);
            }
        }

        public async Task<WebResponse<ListSearchResponse<List<BankStatementItemSearchDTO>>>> Search(BankStatementSearchArgs args)
        {
            try
            {
                var items = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<List<BankStatementItemSearchDTO>>>(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<ListSearchResponse<List<BankStatementItemSearchDTO>>>(ex);
            }
        }

        public async Task<WebResponse<BankStatementItemDTO>> UpdateAsync(BankStatementItemDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<BankStatementItem>(item);

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<BankStatementItemDTO>(AutoMapper.Mapper.Map<BankStatementItemDTO>(result));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.GetExceptionDetailsWithStackTrace());
                return new WebResponse<BankStatementItemDTO>(ex);
            }
        }
    }
}

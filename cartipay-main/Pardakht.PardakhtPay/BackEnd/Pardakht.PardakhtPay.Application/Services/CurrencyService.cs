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
    public class CurrencyService : DatabaseServiceBase<Currency, ICurrencyManager>, ICurrencyService
    {
        public CurrencyService(ICurrencyManager manager, ILogger<CurrencyService> logger):base(manager, logger)
        {

        }

        public async Task<WebResponse<List<CurrencyDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<CurrencyDTO>>(result.Payload);

                return new WebResponse<List<CurrencyDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<CurrencyDTO>>(ex);
            }
        }

        public async Task<WebResponse<CurrencyDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<CurrencyDTO>(result.Payload);

                return new WebResponse<CurrencyDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CurrencyDTO>(ex);
            }
        }

        public async Task<WebResponse<CurrencyDTO>> InsertAsync(CurrencyDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<Currency>(item);

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<CurrencyDTO>(result);

                return new WebResponse<CurrencyDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CurrencyDTO>(ex);
            }
        }

        public async Task<WebResponse<CurrencyDTO>> UpdateAsync(CurrencyDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<Currency>(item);

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<CurrencyDTO>(AutoMapper.Mapper.Map<CurrencyDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<CurrencyDTO>(ex);
            }
        }
    }
}



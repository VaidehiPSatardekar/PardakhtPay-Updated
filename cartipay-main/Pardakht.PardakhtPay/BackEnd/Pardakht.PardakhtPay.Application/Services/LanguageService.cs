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
    public class LanguageService : DatabaseServiceBase<Language, ILanguageManager>, ILanguageService
    {
        public LanguageService(ILanguageManager manager, ILogger<LanguageService> logger):base(manager, logger)
        {

        }

        public async Task<WebResponse<List<LanguageDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<LanguageDTO>>(result.Payload);

                return new WebResponse<List<LanguageDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<LanguageDTO>>(ex);
            }
        }

        public async Task<WebResponse<LanguageDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<LanguageDTO>(result.Payload);

                return new WebResponse<LanguageDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<LanguageDTO>(ex);
            }
        }

        public async Task<WebResponse<LanguageDTO>> InsertAsync(LanguageDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<Language>(item);

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<LanguageDTO>(result);

                return new WebResponse<LanguageDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<LanguageDTO>(ex);
            }
        }

        public async Task<WebResponse<LanguageDTO>> UpdateAsync(LanguageDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<Language>(item);

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<LanguageDTO>(AutoMapper.Mapper.Map<LanguageDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<LanguageDTO>(ex);
            }
        }
    }
}


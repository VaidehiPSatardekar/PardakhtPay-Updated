//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Domain.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.Application.Services
//{
//    public class TenantApiService : DatabaseServiceBase<TenantApi, ITenantApiManager>, ITenantApiService
//    {
//        public TenantApiService(ITenantApiManager manager, ILogger<TenantApiService> logger):base(manager, logger)
//        {

//        }

//        public async Task<WebResponse<List<TenantApiDTO>>> GetAllItemsAsync()
//        {
//            try
//            {
//                var result = await GetAllAsync();

//                if (!result.Success)
//                {
//                    throw new Exception(result.Message);
//                }

//                var dtos = AutoMapper.Mapper.Map<List<TenantApiDTO>>(result.Payload);

//                return new WebResponse<List<TenantApiDTO>>(true, string.Empty, dtos);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<List<TenantApiDTO>>(ex);
//            }
//        }

//        public async Task<WebResponse<TenantApiDTO>> GetItemById(int id)
//        {
//            try
//            {
//                var result = await GetEntityByIdAsync(id);

//                if (!result.Success)
//                {
//                    throw result.Exception;
//                }

//                var dto = AutoMapper.Mapper.Map<TenantApiDTO>(result.Payload);

//                return new WebResponse<TenantApiDTO>(dto);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<TenantApiDTO>(ex);
//            }
//        }

//        public async Task<WebResponse<TenantApiDTO>> InsertAsync(TenantApiDTO item)
//        {
//            try
//            {
//                var entity = AutoMapper.Mapper.Map<TenantApi>(item);

//                var result = await Manager.AddAsync(entity);

//                await Manager.SaveAsync();

//                var dto = AutoMapper.Mapper.Map<TenantApiDTO>(result);

//                return new WebResponse<TenantApiDTO>(true, string.Empty, dto);
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<TenantApiDTO>(ex);
//            }
//        }

//        public async Task<WebResponse<TenantApiDTO>> UpdateAsync(TenantApiDTO item)
//        {
//            try
//            {
//                var entity = AutoMapper.Mapper.Map<TenantApi>(item);

//                var result = await Manager.UpdateAsync(entity);

//                await Manager.SaveAsync();

//                return new WebResponse<TenantApiDTO>(AutoMapper.Mapper.Map<TenantApiDTO>(result));
//            }
//            catch (Exception ex)
//            {
//                return new WebResponse<TenantApiDTO>(ex);
//            }
//        }
//    }
//}

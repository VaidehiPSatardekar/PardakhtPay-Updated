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
    public class PaymentGatewayConfigurationService : DatabaseServiceBase<PaymentGatewayConfiguration, IPaymentGatewayConfigurationManager>, IPaymentGatewayConfigurationService
    {
        public PaymentGatewayConfigurationService(IPaymentGatewayConfigurationManager manager, ILogger<PaymentGatewayConfigurationService> logger) : base(manager, logger)
        {

        }

        public async Task<WebResponse<List<PaymentGatewayConfigurationDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<PaymentGatewayConfigurationDTO>>(result.Payload);

                return new WebResponse<List<PaymentGatewayConfigurationDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<PaymentGatewayConfigurationDTO>>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayConfigurationDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<PaymentGatewayConfigurationDTO>(result.Payload);

                return new WebResponse<PaymentGatewayConfigurationDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayConfigurationDTO>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayConfigurationDTO>> InsertAsync(PaymentGatewayConfigurationDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<PaymentGatewayConfiguration>(item);
                entity.PaymentGatewayGuid = Guid.NewGuid().ToString();

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<PaymentGatewayConfigurationDTO>(result);

                return new WebResponse<PaymentGatewayConfigurationDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayConfigurationDTO>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayConfigurationDTO>> UpdateAsync(PaymentGatewayConfigurationDTO item)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(item.Id);

                var entity = AutoMapper.Mapper.Map<PaymentGatewayConfiguration>(item);

                entity.PaymentGatewayGuid = oldEntity.PaymentGatewayGuid;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<PaymentGatewayConfigurationDTO>(AutoMapper.Mapper.Map<PaymentGatewayConfigurationDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayConfigurationDTO>(ex);
            }
        }
    }
}


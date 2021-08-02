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
    public class PaymentGatewayService : DatabaseServiceBase<PaymentGateway, IPaymentGatewayManager>, IPaymentGatewayService
    {
        public PaymentGatewayService(IPaymentGatewayManager manager, ILogger<PaymentGatewayService> logger) : base(manager, logger)
        {

        }

        public async Task<WebResponse<List<Shared.Models.WebService.PaymentGatewayDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<PaymentGatewayDTO>>(result.Payload);

                return new WebResponse<List<PaymentGatewayDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<PaymentGatewayDTO>>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayDTO>> GetById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<PaymentGatewayDTO>(result.Payload);

                return new WebResponse<PaymentGatewayDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayDTO>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayDTO>> InsertAsync(PaymentGatewayDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<PaymentGateway>(item);

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<PaymentGatewayDTO>(result);

                return new WebResponse<PaymentGatewayDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayDTO>(ex);
            }
        }

        public async Task<WebResponse<PaymentGatewayDTO>> UpdateAsync(PaymentGatewayDTO item)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(item.Id);

                var entity = AutoMapper.Mapper.Map<PaymentGateway>(item);

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<PaymentGatewayDTO>(AutoMapper.Mapper.Map<PaymentGatewayDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<PaymentGatewayDTO>(ex);
            }
        }
    }
}


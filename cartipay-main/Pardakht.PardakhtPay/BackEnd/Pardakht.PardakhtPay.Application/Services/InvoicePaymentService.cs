using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class InvoicePaymentService : DatabaseServiceBase<InvoicePayment, IInvoicePaymentManager>, IInvoicePaymentService
    {
        CurrentUser _CurrentUser = null;

        public InvoicePaymentService(IInvoicePaymentManager manager,
            ILogger<InvoicePaymentService> logger,
            CurrentUser currentUser
            ):base(manager, logger)
        {
            _CurrentUser = currentUser;
        }

        public async Task<WebResponse<InvoicePaymentDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<InvoicePaymentDTO>(result.Payload);
                dto.PaymentDate = dto.PaymentDate.ToUniversalTime();

                return new WebResponse<InvoicePaymentDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<InvoicePaymentDTO>(ex);
            }
        }

        public async Task<WebResponse<InvoicePaymentDTO>> InsertAsync(InvoicePaymentDTO item)
        {
            try
            {
                var payment = AutoMapper.Mapper.Map<InvoicePayment>(item);

                payment.CreateDate = DateTime.UtcNow;
                payment.CreateUserId = _CurrentUser.IdentifierGuid;
                payment.PaymentDate = item.PaymentDate.ToLocalTime();

                var result = await Manager.AddAsync(payment);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<InvoicePaymentDTO>(result);

                return new WebResponse<InvoicePaymentDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<InvoicePaymentDTO>(ex);
            }
        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>>> Search(InvoicePaymentSearchArgs args)
        {
            try
            {
                var response = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>>(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>>(ex);
            }
        }

        public async Task<WebResponse<InvoicePaymentDTO>> UpdateAsync(InvoicePaymentDTO item)
        {
            try
            {
                var oldSetting = await Manager.GetEntityByIdAsync(item.Id);

                var setting = AutoMapper.Mapper.Map<InvoicePayment>(item);
                setting.CreateDate = oldSetting.CreateDate;
                setting.CreateUserId = oldSetting.CreateUserId;
                setting.PaymentDate = item.PaymentDate.ToLocalTime();

                setting.UpdateDate = DateTime.UtcNow;
                setting.UpdateUserId = _CurrentUser.IdentifierGuid;

                var result = await Manager.UpdateAsync(setting);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<InvoicePaymentDTO>(result);

                return new WebResponse<InvoicePaymentDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<InvoicePaymentDTO>(ex);
            }
        }
    }
}

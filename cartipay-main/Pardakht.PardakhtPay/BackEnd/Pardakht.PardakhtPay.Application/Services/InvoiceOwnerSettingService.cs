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
    public class InvoiceOwnerSettingService : DatabaseServiceBase<InvoiceOwnerSetting, IInvoiceOwnerSettingManager>, IInvoiceOwnerSettingService
    {
        CurrentUser _CurrentUser = null;

        public InvoiceOwnerSettingService(IInvoiceOwnerSettingManager manager,
            ILogger<InvoiceOwnerSettingService> logger,
            CurrentUser currentUser):base(manager, logger)
        {
            _CurrentUser = currentUser;
        }

        public async Task<WebResponse<List<InvoiceOwnerSettingDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<InvoiceOwnerSettingDTO>>(result.Payload);

                return new WebResponse<List<InvoiceOwnerSettingDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<List<InvoiceOwnerSettingDTO>>(ex);
            }
        }

        public async Task<WebResponse<InvoiceOwnerSettingDTO>> GetInvoiceOwnerSettingById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<InvoiceOwnerSettingDTO>(result.Payload);

                dto.StartDate = dto.StartDate.ToUniversalTime();

                return new WebResponse<InvoiceOwnerSettingDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<InvoiceOwnerSettingDTO>(ex);
            }
        }

        public async Task<WebResponse<InvoiceOwnerSettingDTO>> InsertAsync(InvoiceOwnerSettingDTO item)
        {
            try
            {
                var setting = AutoMapper.Mapper.Map<InvoiceOwnerSetting>(item);

                setting.CreateDate = DateTime.UtcNow;
                setting.CreateUserId = _CurrentUser.IdentifierGuid;
                setting.StartDate = item.StartDate.ToLocalTime();

                var result = await Manager.AddAsync(setting);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<InvoiceOwnerSettingDTO>(result);

                return new WebResponse<InvoiceOwnerSettingDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<InvoiceOwnerSettingDTO>(ex);
            }
        }

        public async Task<WebResponse<InvoiceOwnerSettingDTO>> UpdateAsync(InvoiceOwnerSettingDTO item)
        {
            try
            {
                var oldSetting = await Manager.GetEntityByIdAsync(item.Id);

                var setting = AutoMapper.Mapper.Map<InvoiceOwnerSetting>(item);
                setting.CreateDate = oldSetting.CreateDate;
                setting.CreateUserId = oldSetting.CreateUserId;

                setting.UpdateDate = DateTime.UtcNow;
                setting.UpdateUserId = _CurrentUser.IdentifierGuid;
                setting.StartDate = item.StartDate.ToLocalTime();

                var result = await Manager.UpdateAsync(setting);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<InvoiceOwnerSettingDTO>(result);

                return new WebResponse<InvoiceOwnerSettingDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<InvoiceOwnerSettingDTO>(ex);
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
                return new WebResponse(ex);
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class OwnerSettingService : DatabaseServiceBase<OwnerSetting, IOwnerSettingManager>, IOwnerSettingService
    {
        CurrentUser _CurrentUser = null;

        public OwnerSettingService(IOwnerSettingManager manager,
            ILogger<OwnerSettingService> logger,
            CurrentUser currentUser)
            :base(manager, logger)
        {
            _CurrentUser = currentUser;
        }

        public async Task<WebResponse<OwnerSettingDTO>> Get()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var item = result.Payload.FirstOrDefault();

                if(item == null)
                {
                    item = new OwnerSetting();
                }

                var dto = AutoMapper.Mapper.Map<OwnerSettingDTO>(item);

                return new WebResponse<OwnerSettingDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<OwnerSettingDTO>(ex);
            }
        }

        public async Task<WebResponse<OwnerSettingDTO>> InsertAsync(OwnerSettingDTO item)
        {
            try
            {
                var setting = AutoMapper.Mapper.Map<OwnerSetting>(item);

                setting.CreateUserId = _CurrentUser.IdentifierGuid;
                setting.OwnerGuid = _CurrentUser.ParentAccountId ?? _CurrentUser.IdentifierGuid;
                setting.CreateDate = DateTime.UtcNow;

                var result = await Manager.AddAsync(setting);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<OwnerSettingDTO>(result);

                return new WebResponse<OwnerSettingDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<OwnerSettingDTO>(ex);
            }
        }

        public async Task<WebResponse<OwnerSettingDTO>> UpdateAsync(OwnerSettingDTO item)
        {
            try
            {
                var oldSetting = await Manager.GetEntityByIdAsync(item.Id);

                var setting = AutoMapper.Mapper.Map<OwnerSetting>(item);
                setting.CreateDate = oldSetting.CreateDate;
                setting.CreateUserId = oldSetting.CreateUserId;

                setting.UpdateDate = DateTime.UtcNow;
                setting.UpdateUserId = _CurrentUser.IdentifierGuid;
                setting.OwnerGuid = oldSetting.OwnerGuid;

                var result = await Manager.UpdateAsync(setting);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<OwnerSettingDTO>(result);

                return new WebResponse<OwnerSettingDTO>(dto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<OwnerSettingDTO>(ex);
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

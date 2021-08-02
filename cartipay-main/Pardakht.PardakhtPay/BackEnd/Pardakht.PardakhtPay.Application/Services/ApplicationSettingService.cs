using Microsoft.Extensions.Logging;
using System;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class ApplicationSettingService : DatabaseServiceBase<ApplicationSetting, IApplicationSettingManager>, IApplicationSettingService
    {
        ICachedObjectManager _CachedObjectManager = null;
        IReflectionService _ReflectionService = null;

        public ApplicationSettingService(IApplicationSettingManager manager,
            ILogger<ApplicationSettingService> logger,
            ICachedObjectManager cachedObjectManager,
            IReflectionService reflectionService) : base(manager, logger)
        {
            _CachedObjectManager = cachedObjectManager;
            _ReflectionService = reflectionService;
        }

        public async Task<T> Get<T>() where T : new()
        {

            var items = await _CachedObjectManager.GetCachedItems<ApplicationSetting, IApplicationSettingRepository>();

            var attributes = _ReflectionService.GetAttributes<T>();

            var attribute = attributes.OfType<SettingAttribute>().FirstOrDefault();

            if(attribute != null)
            {
                var item = items.FirstOrDefault(t => t.Key == attribute.Key);

                if(item != null)
                {
                    return JsonConvert.DeserializeObject<T>(item.Value);
                }
            }

            return default(T);
        }

        public async Task<WebResponse<ApplicationSettingsDTO>> GetAllSettings()
        {
            try
            {
                ApplicationSettingsDTO model = new ApplicationSettingsDTO();
                var settings = await Manager.Get<SmsServiceConfiguration>();

                var settingsItem = JsonConvert.DeserializeObject<SmsServiceConfiguration>(settings.Value);
                settingsItem.Id = settings.Id;
                settingsItem.Seconds = Convert.ToInt32(settingsItem.ExpireTime.TotalSeconds);
                model.SmsConfiguration = settingsItem;

                var maliciousSettings = await Manager.Get<MaliciousCustomerSettings>();
                var malicious = JsonConvert.DeserializeObject<MaliciousCustomerSettings>(maliciousSettings.Value);
                malicious.Id = maliciousSettings.Id;
                model.MaliciousCustomerSettings = malicious;

                var bankAccountConfiguration = await Manager.Get<BankAccountConfiguration>();
                var bankAccount = JsonConvert.DeserializeObject<BankAccountConfiguration>(bankAccountConfiguration.Value);
                bankAccount.Id = bankAccountConfiguration.Id;
                model.BankAccountConfiguration = bankAccount;

                var mobileApiConfiguration = await Manager.Get<MobileApiConfiguration>();
                var mobileApi = JsonConvert.DeserializeObject<MobileApiConfiguration>(mobileApiConfiguration.Value);
                mobileApi.Id = mobileApiConfiguration.Id;
                model.MobileApiConfiguration = mobileApi;

                return new WebResponse<ApplicationSettingsDTO>(model);
            }
            catch (Exception ex)
            {
                return new WebResponse<ApplicationSettingsDTO>(ex);
            }
        }

        public async Task<WebResponse<ApplicationSettingsDTO>> Update(ApplicationSettingsDTO model)
        {
            try
            {
                var settings = new ApplicationSetting();
                settings.Key = ApplicationSettingKeys.SmsService;
                model.SmsConfiguration.ExpireTime = TimeSpan.FromSeconds(model.SmsConfiguration.Seconds);
                settings.Id = model.SmsConfiguration.Id;
                settings.Value = JsonConvert.SerializeObject(model.SmsConfiguration);

                await Manager.UpdateAsync(settings);
                await Manager.SaveAsync();

                var maliciousSettings = new ApplicationSetting();
                maliciousSettings.Key = ApplicationSettingKeys.Malicious;
                maliciousSettings.Id = model.MaliciousCustomerSettings.Id;
                maliciousSettings.Value = JsonConvert.SerializeObject(model.MaliciousCustomerSettings);

                await Manager.UpdateAsync(maliciousSettings);
                await Manager.SaveAsync();

                var bankAccountSettings = new ApplicationSetting();
                bankAccountSettings.Key = ApplicationSettingKeys.BankAccount;
                bankAccountSettings.Id = model.BankAccountConfiguration.Id;
                bankAccountSettings.Value = JsonConvert.SerializeObject(model.BankAccountConfiguration);

                await Manager.UpdateAsync(bankAccountSettings);
                await Manager.SaveAsync();

                var mobileApiSettings = new ApplicationSetting();
                mobileApiSettings.Key = ApplicationSettingKeys.MobileApi;
                mobileApiSettings.Id = model.MobileApiConfiguration.Id;
                mobileApiSettings.Value = JsonConvert.SerializeObject(model.MobileApiConfiguration);

                await Manager.UpdateAsync(mobileApiSettings);
                await Manager.SaveAsync();

                return new WebResponse<ApplicationSettingsDTO>(model);
            }
            catch (Exception ex)
            {
                return new WebResponse<ApplicationSettingsDTO>(ex);
            }
        }
    }
}

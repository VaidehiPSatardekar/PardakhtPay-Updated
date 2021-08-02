using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.UserManagement.Application.TenantServiceApiAuth;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;
using Pardakht.UserManagement.Shared.Models.WebService.Enum;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public abstract class StaffUserHandlerBase
    {
        private static MemoryCache tenantCache { get; } = new MemoryCache(new MemoryCacheOptions());
        private const int cacheTimeInMinutes = 5; // TODO: configure
        protected UserManager<ApplicationUser> identityUserManager { get { return handlerArgs.identityUserManager; } }
        protected UserContext userContext { get { return handlerArgs.userContext; } }
        protected IStaffUserManager staffUserManager { get { return handlerArgs.staffUserManager; } }
        protected IHttpClientFactory httpClientFactory { get { return handlerArgs.httpClientFactory; } }

        protected SendGridSettings emailServiceSettings { get { return handlerArgs.emailServiceSettings; } }
        protected RoleSettings roleSettings { get { return handlerArgs.roleSettings; } }
        protected EmailNotification emailNotification { get { return handlerArgs.emailNotification; } }


        protected ILogger logger { get { return handlerArgs.logger; } }
        private readonly StaffUserHandlerArgs handlerArgs;
        private const string cacheName = "tenants";

        public StaffUserHandlerBase(StaffUserHandlerArgs handlerArgs)
        {
            this.handlerArgs = handlerArgs;
        }
        
      
        protected async Task<Shared.Models.WebService.Tenant> GetTenant(string tenantPlatformGuid)
        {
            return new Tenant
            {
                TenancyName = "1",
                PrimaryDomainName = "1",
                Id = 1,
                Languages = new TenantPlatformMapLanguages[0],
                PlatformMappings = new List<TenantPlatformMap>()
                {
                    new TenantPlatformMap
                    {
                        TenancyName = "1",
                        Tenant = new TenantDto()
                        {
                            TenancyName = "1",
                            BusinessName = "1",
                            Email = "1"
                        },
                        PrimaryDomainName = "1",
                        BrandName = "1",
                        PlatformGuid = "1",
                        PreferenceConfig = string.Empty,
                        TenantPlatformMapGuid = "1"
                    }
                }
            };
        }

        protected  bool GetTenantNotificationPreference(Tenant tenant)
        {
            return true;
        }
        protected async Task<TenantPlatformMap> GetTenantPlatformMapping(int tenantId, string platformGuid)
        {
            var tenants = await GetTenants();

            if (tenants != null)
            {
                var tenant = tenants
                    .Where(t => t.Id == tenantId && t.PlatformMappings != null && t.PlatformMappings.Any(p => p.PlatformGuid == platformGuid))
                    .FirstOrDefault();

                if (tenant != null)
                {
                    var _tenant = tenant.PlatformMappings.Where(m => m.PlatformGuid == platformGuid).FirstOrDefault();
                    _tenant.TenancyName = tenant.TenancyName;
                    return _tenant;
                }
            }

            return null;
        }

        protected void InvalidateTenantCache()
        {
            tenantCache.Remove(cacheName);
        }

        protected string GenerateRandomPassword()
        {
            var generator = new RandomPasswordGenerator();

            return generator.GeneratePassword();
        }

        protected async Task<bool> SendMail(Shared.Models.WebService.StaffUser staffUser, string subject, string body, string platformGuid)
        {
            var fromAddress = emailNotification.FromAddress;
            var fromName = emailNotification.FromEmail;
            return await SendMailAsync(body, subject, fromName, fromAddress, staffUser.Email, staffUser.Email);
        }

        public async Task<Shared.Models.Infrastructure.StaffUser> GetStaffUserByAccountId(Shared.Models.WebService.StaffUser request)
        {
            return await staffUserManager.GetStaffUserByAccountId(request.AccountId);
        }

        public async Task SendEmailTenantBlockedStaffUserorDeletedorPasswordChange(Shared.Models.WebService.StaffUser request, UserAction userAction)
        {
            try
            {
                Shared.Models.WebService.Tenant tenant = null;
                return;

                await SendMailUserAction(tenant, request, userAction, "");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "StaffUserHandlerBase.SendEmailTenantRoleAssignedNotification");
            }
        }
        public async Task<List<int>> GetNewRoleList(Shared.Models.WebService.StaffUser request,string platformGuid)
        {
            try
            {


                var existingRoles = await staffUserManager.GetById(request.Id, userContext.PlatformGuid);
                var _existingRoles = existingRoles.PlatformRoleMappings.Select(x => x.Roles.ToList()).ToList();
                var newRoles = request.PlatformRoleMappings.Select(x => x.Roles.ToList()).ToList();

                List<int> existsRoleList = new List<int>();
                List<int> newRoleList = new List<int>();
                List<int> _newRoleList = new List<int>();
                foreach (var existRole in _existingRoles)
                {
                    foreach (var itemExist in existRole)
                    {
                        existsRoleList.Add(itemExist);

                    }
                }

                foreach (var itemNewRole in newRoles)
                {
                    foreach (var item in itemNewRole)
                    {
                        newRoleList.Add(item);
                    }
                }

                foreach (var item in newRoleList)
                {
                    int findIndex = existsRoleList.ToList().IndexOf(item);
                    if (findIndex == -1)
                    {
                        _newRoleList.Add(item);
                    }
                }
                return _newRoleList;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "StaffUserHandlerBase.GetNewRoleList ERROR");
                return null;
            }
        }
        public async Task SendEmailTenantRoleAssignedNotification(Shared.Models.WebService.StaffUser request, List<int> roleList)
        {
            try
            {
                Shared.Models.WebService.Tenant tenant = null;
                if (!string.IsNullOrEmpty(request.TenantGuid))
                {
                    tenant = await GetTenant(request.TenantGuid);
                }

                var tenantNotification = GetTenantNotificationPreference(tenant);

                if (!tenantNotification)
                    return;

                //if (userDetail.UserType != UserType.StaffUser)
                //    return;

                string rolesName = string.Empty;

                var roles = staffUserManager.GetRoles();
                var _newRoles = request.PlatformRoleMappings.Select(x => x.Roles);

                foreach (var item in roleList.ToList())
                {
                    //foreach (var _item in item)
                    //{
                    var roleDetail = roles.Where(x => x.Id == item).FirstOrDefault();
                    if (rolesName == string.Empty)
                        rolesName = string.Format($"{roleDetail?.Name}");
                    else
                        rolesName += string.Format($", {roleDetail?.Name}");
                    //}

                }

                await SendMailUserAction(tenant, request, UserAction.StaffUserRoleAsg, rolesName);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "StaffUserHandlerBase.SendEmailTenantRoleAssignedNotification");
            }
        }
        public async Task SendEmailTenantCreatedStaffNotification(Tenant tenant, Shared.Models.WebService.StaffUser request)
        {
            try
            {
                var tenantNotification = GetTenantNotificationPreference(tenant);

                if (!tenantNotification)
                    return;

                var userDetail = await staffUserManager.GetStaffUserByAccountId(request.AccountId);

                if (userDetail.UserType != UserType.StaffUser)
                    return;

                string rolesName = string.Empty;
                var userHasRoles = userDetail.UserPlatforms.Where(x => x.StaffUserId == userDetail.Id)
                                                            .Select(s => s.UserPlatformRoles
                                                                    .Select(x => x.Role.Name)
                                                            ).ToList();
                foreach (var item in userHasRoles)
                {
                    foreach (var itemRoleName in item)
                    {
                        if (rolesName == string.Empty)
                            rolesName = string.Format($"{itemRoleName}");
                        else
                            rolesName += string.Format($", {itemRoleName}");
                    }
                }
                await SendMailUserAction(tenant, request, UserAction.CreateStaffUser, rolesName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "StaffUserHandlerBase.SendTenantNotification");
            }
        }
        private string GenerateTemplate(string langCode,string fileName)
        {
            string body = string.Empty;
            var bundleAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName.Contains("Pardakhtpay.Shared.Models"));
            var asmPath = new Uri(bundleAssembly.CodeBase).LocalPath;
            var templatePath = Path.Combine(Path.GetDirectoryName(asmPath), "EmailTemplates", $"{langCode}-{fileName}-staff-user.html");
            using (StreamReader reader = new StreamReader(templatePath))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
        public async Task SendMailUserAction(Tenant tenant, Shared.Models.WebService.StaffUser staffUser, UserAction userAction,string rolesName)
        {
            try
            {
                //var tenantLang = await GetTenantsLanguageFromApi(staffUser.TenantGuid);
                //var lang = tenantLang.Languages?.Where(x => x.IsDefault).Select(s => s.Code).FirstOrDefault();
                var lang = "fa";

                string subject = string.Empty;
                string body = string.Empty;


                var fromAddress = emailNotification.FromAddress;
                var fromName = emailNotification.FromEmail;

                var _tenant = tenant.PlatformMappings.FirstOrDefault();

                switch (userAction)
                {
                    case UserAction.CreateStaffUser:
                        body = GenerateTemplate(lang, "creation");
                        subject = lang == "en" ? "New Staff User Created" : "ایجاد کاربر جدید";
                        body = body.Replace("{Email}", staffUser.Email);
                        body = body.Replace("{Role}", rolesName);
                        break;
                    case UserAction.StaffUserRoleAsg:
                        body = GenerateTemplate(lang, "role-assignments");
                        subject = lang == "en" ? "Staff user’s Role changed" : "تغییر سطح دسترسی کاربر";
                        body = body.Replace("{Email}", staffUser.Email);
                        body = body.Replace("{Roles}", rolesName);
                        break;
                    case UserAction.BlockingUnBlockinUser:
                        string fileName = staffUser.IsBlocked ? "blocked" : "unblocked";
                        body = GenerateTemplate(lang, fileName);
                        subject = staffUser.IsBlocked ?
                                                    (lang == "en" ? "Staff User blocked" : "مسدود شدن دسترسی کاربر") :
                                                    (lang == "en" ? "Staff User Unblocked" : "فعال شدن دسترسی مسدود شده");
                        break;
                    case UserAction.PasswordChange:
                        body = GenerateTemplate(lang, "password-change");
                        subject = lang == "en" ? "Staff User’s Password changed" : "تغییر رمز عبور کاربر";
                        break;
                    case UserAction.DeleteUser:
                        body = GenerateTemplate(lang, "delete");
                        subject = lang == "en" ? "Staff User’s account Deleted" : "حذف شدن حساب کاربر";
                        break;
                    default:
                        break;
                }
                body = body.Replace("{TenancyName}", tenant.TenancyName);
                body = body.Replace("{Name}", staffUser.FirstName);
                body = body.Replace("{Username}", staffUser.Username);
                body = body.Replace("{Surname}", staffUser.LastName);

                var result = await SendMailAsync(body, subject, fromName, fromAddress, _tenant.Tenant?.Email, _tenant.Tenant?.Email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "StaffUserHandlerBase.SendMailUserAction Error");
            }
        }

        protected async Task<bool> SendMailAsync(string body, string subject, string fromName, string fromAddress, string toEmailName, string toEmailAddress)
        {
            var email = new EmailServiceRequest
            {
                FromEmail = new Email
                {
                    Name = fromName,
                    EmailAddress = fromAddress
                },
                ToEmail = new Email
                {
                    Name = toEmailName, //staffUser.Email,
                    EmailAddress = toEmailAddress //staffUser.Email
                },
                Body = body,
                Subject = subject,
                IsBodyHtml = true
            };

            try
            {
                var client = new SendGridClient(emailServiceSettings.ApiKey);
                var from = new EmailAddress(email.FromEmail.EmailAddress, email.FromEmail.Name);
                var toAddress = new EmailAddress(email.ToEmail.EmailAddress, email.ToEmail.Name);
                var plainTextContent = string.Empty;
                var htmlContent = email.Body;
                if (!email.IsBodyHtml.Value)
                {
                    htmlContent = string.Empty;
                    plainTextContent = email.Body;
                }

                var msg = MailHelper.CreateSingleEmail(from, toAddress, email.Subject, plainTextContent, htmlContent);

                if (email.ReplyTo != null)
                {
                    msg.ReplyTo = new EmailAddress(email.ReplyTo.EmailAddress, email.ReplyTo.Name);
                }

                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == HttpStatusCode.Accepted;

            }
            catch (Exception e)
            {
                logger.LogError(e,"Email problem");
                return false;
            }

        }

        private async Task<IEnumerable<Shared.Models.WebService.Tenant>> GetTenants()
        {
            // TODO: disabled cahcing for now - look into using lazy cache https://github.com/alastairtree/LazyCache/wiki/API-documentation-(v-2.x)
            // we need to investigate usage of cache - eg when we create a new tenant there will be a problem logging in as the new tenant will not be in the cache
            //get a list of tenants from the tenant management api and cache
            if (!tenantCache.TryGetValue(cacheName, out List<Shared.Models.WebService.Tenant> result))
            {
                result = await GetTenantsFromApi();

                if (result != null)
                {
                    var tenants = result.Where(t => t.PlatformMappings != null).ToList();

                    logger.LogInformation($"StaffUserHandlerBase: retrieved {result.Count} tenants from api");
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheTimeInMinutes));

                    tenantCache.Set("tenants", tenants, cacheEntryOptions);
                }
                else
                {
                    logger.LogWarning("StaffUserHandlerBase: api call to get tenants returned null");
                }
            }

            return result;
        }

        private async Task<List<Shared.Models.WebService.Tenant>> GetTenantsFromApi()
        {
            return new List<Tenant>()
            {
                new Tenant
                {
                    TenancyName = "1",
                    PrimaryDomainName = "1",
                    Id = 1,
                    Languages = new TenantPlatformMapLanguages[0],
                    PlatformMappings = new List<TenantPlatformMap>()
                    {
                        new TenantPlatformMap
                        {
                            TenancyName = "1",
                            Tenant = new TenantDto()
                            {
                                TenancyName = "1",
                                BusinessName = "1",
                                Email = "1"
                            },
                            PrimaryDomainName = "1",
                            BrandName = "1",
                            PlatformGuid = "1",
                            PreferenceConfig = string.Empty,
                            TenantPlatformMapGuid = "1"
                        }
                    }
                }
            };
        }

       

        public async Task<Shared.Models.WebService.TenantDetail> GetTenantsLanguageFromApi(string tenantGuid)
        {
            var url = $"{handlerArgs.tenantManagementSettings.Url}/api/TenantPlatform/tenant-select-with-languages";

            try
            {
                var token = await handlerArgs.tenantServiceApiTokenGenerator.Token();

                if (string.IsNullOrEmpty(token))
                {
                    logger.LogError("StaffUserHandlerBase: unable to generate authorization token");
                }

                // this is a pure api-api call, so we don't want to propagate the logged in user's credentials
               
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Account-Context", tenantGuid);
                var response = await client.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody.Length > 2)
                    {
                        return JsonConvert.DeserializeObject<Shared.Models.WebService.TenantDetail>(responseBody);
                    }
                }

                var contents = await response.Content.ReadAsStringAsync();
                logger.LogWarning($"StaffUserHandlerBase.GetTenantsFromApi: Problem with getting tenants for {url}  response code {response.StatusCode} - {contents}");

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"GetTenantHandler.GetTenantsFromApi: An error occurred getting tenant list from {url} - {ex}");
                return null;
            }
        }

        
    }

    public class StaffUserHandlerArgs
    {
        public readonly UserManager<ApplicationUser> identityUserManager;
        public readonly TenantManagementSettings tenantManagementSettings;
        public readonly IStaffUserManager staffUserManager;
        public readonly UserContext userContext;
        public readonly TenantServiceApiTokenGenerator tenantServiceApiTokenGenerator;
        public readonly SendGridSettings emailServiceSettings;
        public readonly RoleSettings roleSettings;
        public readonly ILogger logger;
        public readonly IHttpClientFactory httpClientFactory;
        public readonly EmailNotification emailNotification;

        public StaffUserHandlerArgs(UserManager<ApplicationUser> identityUserManager, 
                                    IStaffUserManager staffUserManager,
                                    IHttpClientFactory httpClientFactory,
                                    TenantManagementSettings tenantManagementSettings,
                                    UserContext userContext,
                                    TenantServiceApiTokenGenerator tenantServiceApiTokenGenerator,
                                    SendGridSettings emailServiceSettings,
                                    ILogger logger,
                                    RoleSettings roleSettings,
                                    EmailNotification emailNotification)
        {
            this.httpClientFactory = httpClientFactory;
            this.identityUserManager = identityUserManager;
            this.staffUserManager = staffUserManager;
            this.tenantManagementSettings = tenantManagementSettings;
            this.userContext = userContext;
            this.tenantServiceApiTokenGenerator = tenantServiceApiTokenGenerator;
            this.emailServiceSettings = emailServiceSettings;
            this.logger = logger;
            this.roleSettings = roleSettings;
            this.emailNotification = emailNotification;
        }
    }

}

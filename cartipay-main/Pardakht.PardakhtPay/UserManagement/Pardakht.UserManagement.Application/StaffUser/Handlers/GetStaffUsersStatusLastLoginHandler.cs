using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.TimeZone;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class GetStaffUsersStatusLastLoginHandler : StaffUserHandlerBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IJwtTokenService jwtTokenService;
        private readonly StaffUserHandlerArgs handlerArgs;
        public GetStaffUsersStatusLastLoginHandler(StaffUserHandlerArgs handlerArgs, IJwtTokenService jwtTokenService,IServiceProvider serviceProvider) : base(handlerArgs) {
            this.serviceProvider = serviceProvider;
            this.jwtTokenService = jwtTokenService;
            this.handlerArgs = handlerArgs;
        }

        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>>> Handle(string tenantGuid, IEnumerable<int> staffUserIds, bool all = false,string timeZoneId=null)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.HelpDeskDashboard))
                {
                    return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "User not authorised to view last logins" };
                }

                if (!string.IsNullOrEmpty(tenantGuid))
                {
                    if (!userContext.IsProviderUser())
                    {
                        if (tenantGuid != userContext.TenantGuid)
                        {
                            return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "Tenant users cannot view last logins from other tenancies" };
                        }
                    }
                }
                else
                {
                    if (!userContext.IsProviderUser())
                    {
                        return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "Tenant users cannot view provider last logins" };
                    }
                }

                var result = await staffUserManager.GetStaffUsersStatusLastLogin(tenantGuid, userContext.PlatformGuid, staffUserIds, userContext.IsProviderUser(), all);

                if (timeZoneId != null)
                {
                    ApiKeyLoginRequest request = new ApiKeyLoginRequest { ApiKey = "api_key_user_management", PlatformGuid = "UserManagementGuid" };

                    var handler = new ApiKeyLoginHandler(jwtTokenService, handlerArgs);

                    var _response = await handler.Handle(request);

                    var timeZoneService = serviceProvider.GetRequiredService<ITimeZoneService>();

                    var response = result.ToList();

                    var lastLoginDates = response.Select(x => x.LoginDateTime).ToList();
                    var timeZoneCalendarCode = TimeZoneExtension.GetCalendarCode(timeZoneId);

                    var lastLoginConvertedDates = await timeZoneService.ConvertCalendarLocal(lastLoginDates,string.Empty, timeZoneCalendarCode, _response.Payload.AccessToken);

                    for (int i = 0; i < response.Count(); i++)
                    {
                        var item = response[i];
                        if (item.LoginDateTime != null)
                        {
                            item.LoginDateTimeStr = lastLoginConvertedDates[i];
                        }
                    }

                    result = response;

                }

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetStaffUsersStatusLastLoginHandler.Handle: an error occurred getting staff users statistics for platform {userContext.PlatformGuid}, tenant {tenantGuid} - {ex}");
                throw;
            }
        }


        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>>> Handle2(string platformGuid,string tenantGuid,IEnumerable<int> staffUserIds, bool all = false, string timeZoneId = null)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.HelpDeskDashboard))
                {
                    return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "User not authorised to view last logins" };
                }

                if (!string.IsNullOrEmpty(tenantGuid))
                {
                    if (!userContext.IsProviderUser())
                    {
                        if (tenantGuid != userContext.TenantGuid)
                        {
                            return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "Tenant users cannot view last logins from other tenancies" };
                        }
                    }
                }
                else
                {
                    if (!userContext.IsProviderUser())
                    {
                        return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>> { Success = false, Message = "Tenant users cannot view provider last logins" };
                    }
                }

                var result = await staffUserManager.GetStaffUsersStatusLastLogin(tenantGuid, platformGuid, staffUserIds, userContext.IsProviderUser(), all);

                if (timeZoneId != null)
                {
                    ApiKeyLoginRequest request = new ApiKeyLoginRequest { ApiKey = "api_key_user_management", PlatformGuid = "UserManagementGuid" };

                    var handler = new ApiKeyLoginHandler(jwtTokenService, handlerArgs);

                    var _response = await handler.Handle(request);

                    var timeZoneService = serviceProvider.GetRequiredService<ITimeZoneService>();

                    var response = result.ToList();

                    var lastLoginDates = response.Select(x => x.LoginDateTime).ToList();
                    var timeZoneCalendarCode = TimeZoneExtension.GetCalendarCode(timeZoneId);

                    var lastLoginConvertedDates = await timeZoneService.ConvertCalendarLocal(lastLoginDates, string.Empty, timeZoneCalendarCode, _response.Payload.AccessToken);

                    for (int i = 0; i < response.Count(); i++)
                    {
                        var item = response[i];
                        if (item.LoginDateTime != null)
                        {
                            item.LoginDateTimeStr = lastLoginConvertedDates[i];
                        }
                    }

                    result = response;

                }

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserStatusLastLogin>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetStaffUsersStatusLastLoginHandler.Handle: an error occurred getting staff users statistics for platform {userContext.PlatformGuid}, tenant {tenantGuid} - {ex}");
                throw;
            }
        }

    }
}

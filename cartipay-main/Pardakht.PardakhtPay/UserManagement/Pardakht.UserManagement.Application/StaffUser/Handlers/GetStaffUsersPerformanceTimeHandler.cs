using System;
using System.Collections.Generic;
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
    public class GetStaffUsersPerformanceTimeHandler : StaffUserHandlerBase
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly StaffUserHandlerArgs handlerArgs;
        private readonly IServiceProvider serviceProvider;

        public GetStaffUsersPerformanceTimeHandler(StaffUserHandlerArgs handlerArgs, IJwtTokenService jwtTokenService, IServiceProvider serviceProvider) : base(handlerArgs)
        {
            this.jwtTokenService = jwtTokenService;
            this.handlerArgs = handlerArgs;
            this.serviceProvider = serviceProvider;
        }



        public async Task<WebResponse<IEnumerable<Shared.Models.WebService.StaffUserPerformanceTime>>> Handle(string tenantGuid, DateTime dateFrom, DateTime dateTo, bool all = false, string timeZoneId = null)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.HelpDeskDashboard))
                {
                    return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserPerformanceTime>> { Success = false, Message = "User not authorised to view statistics" };
                }

                if (!string.IsNullOrEmpty(tenantGuid))
                {
                    if (!userContext.IsProviderUser())
                    {
                        if (tenantGuid != userContext.TenantGuid)
                        {
                            return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserPerformanceTime>> { Success = false, Message = "Tenant users cannot view statistics from other tenancies" };
                        }
                    }
                }
                else
                {
                    if (!userContext.IsProviderUser())
                    {
                        return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserPerformanceTime>> { Success = false, Message = "Tenant users cannot view provider statistics" };
                    }
                }

                if (timeZoneId != null)
                {
                    ApiKeyLoginRequest request = new ApiKeyLoginRequest { ApiKey = "api_key_user_management", PlatformGuid = "UserManagementGuid" };

                    var handler = new ApiKeyLoginHandler(jwtTokenService, handlerArgs);

                    var response = await handler.Handle(request);


                    if (timeZoneId != null)
                    {
                        var timeZoneCode = TimeZoneExtension.GetTimeZoneCode(timeZoneId);
                        var timeZoneService = serviceProvider.GetRequiredService<ITimeZoneService>();

                        if (dateFrom != null)
                        {
                            var fromDate = await timeZoneService.ConvertCalendar(dateFrom, string.Empty, timeZoneCode, response.Payload.AccessToken);
                            dateFrom = fromDate;
                        }

                        if (dateTo != null)
                        {
                            var toDate = await timeZoneService.ConvertCalendar(dateTo, string.Empty, timeZoneCode, response.Payload.AccessToken);
                            dateTo = toDate;
                        }
                    }
                }
               
                var result = await staffUserManager.GetStaffUsersPerformanceTime(tenantGuid, userContext.PlatformGuid, dateFrom, dateTo, userContext.IsProviderUser(), all);

                return new WebResponse<IEnumerable<Shared.Models.WebService.StaffUserPerformanceTime>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetStaffUsersPerformanceTimeHandler.Handle: an error occurred getting staff users statistics for platform {userContext.PlatformGuid}, tenant {tenantGuid} - {ex}");
                throw;
            }
        }
    }
}

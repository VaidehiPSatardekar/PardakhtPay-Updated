using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.User;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.User;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.User
{
    public class UserResolverService : IUserResolverService
    {
        private readonly UserManagementSettings userManagementSettings;
        private readonly IGenericManagementFunctions<UserManagementSettings> _genericManagementFunctions;

        public UserResolverService(IGenericManagementFunctions<UserManagementSettings> genericManagementFunctions, IOptions<UserManagementSettings> userManagementSettings)
        {
            this.userManagementSettings = userManagementSettings.Value;
            _genericManagementFunctions = genericManagementFunctions;
        }

        public async Task<StaffUser> GetUser(string accountId, string tenantGuid, string platformGuid)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return null;
            }

            var url = $"/api/staffuser/get-user-details/{platformGuid}/{accountId}/{tenantGuid}{(string.IsNullOrEmpty(tenantGuid) ? "true" : "/true")}";
            var response = await _genericManagementFunctions.MakeRequest(null, null, url, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<StaffUser>(content);
            if (response.IsSuccessStatusCode && payload != null)
            {
                return payload;
            }

            return null;
        }
    }
}

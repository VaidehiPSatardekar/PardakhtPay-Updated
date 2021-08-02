namespace Pardakht.UserManagement.Shared.Models.Configuration
{
    //public class TenantManagementSettings : ApiSettings { }

    public class ApiSettings
    {
        public string ApiKey { get; set; }
        public string Url { get; set; }
        public string PlatformGuid { get; set; }
    }
    public class EmailManagementSettings : Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings.ApiSettings { }
    public class RoleSettings
    {
        public string TenantAdminRoleName { get; set; }
        public string ProviderAdminRoleName { get; set; }

    }

  //  public class TimeZoneConfiguration: Enterprise.Utilities.Models.Settings.ApiSettings { }

}

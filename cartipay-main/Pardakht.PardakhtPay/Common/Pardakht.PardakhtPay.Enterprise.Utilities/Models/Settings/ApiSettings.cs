namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings
{
    public abstract class ApiSettings
    {
        public string ApiKey { get; set; }
        public string Url { get; set; }
        public string PlatformGuid { get; set; }
        public string JwtKey { get; set; }
    }
}

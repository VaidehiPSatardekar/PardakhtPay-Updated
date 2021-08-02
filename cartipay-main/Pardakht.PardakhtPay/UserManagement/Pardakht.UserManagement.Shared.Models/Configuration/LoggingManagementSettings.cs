namespace Pardakht.UserManagement.Shared.Models.Configuration
{
    public class LoggingManagementSettings : Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings.ApiSettings
    {
        public string Stage { get; set; }
        public string LogSource { get; set; }
        public string Tag { get; set; }
        public string DefaultLogLevel { get; set; }
        public string LogEnabled { get; set; }
        public string FileLogEnabled { get; set; }
        public string FileNameAndPath { get; set; }
    }
}

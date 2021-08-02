using System;
using System.ComponentModel.DataAnnotations;
using Serilog.Events;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class LoggingSetting
    {
        [MaxLength(100, ErrorMessage = "{0} can have a max of {1} characters")]
        public string LogSource { get; set; }
        [MaxLength(100, ErrorMessage = "{0} can have a max of {1} characters")]
        public string Tag { get; set; }
        public LogEventLevel MinimumLogLevel { get; set; }
        public bool LogEnabled { get; set; }
        public bool FileLogEnabled { get; set; }
        public string FileNameAndPath { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string PlatformGuid { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}

using Serilog.Core;
using Serilog.Events;

namespace Pardakht.PardakhtPay.Shared.Models
{
    public class LogEnricher : ILogEventEnricher
    {
        public static string LogSource = "Pardakht.PardakhtPay";
        public static string Tag = "Pardakht.PardakhtPay";

        static LogEnricher()
        {
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
        {
            logEvent.AddOrUpdateProperty(pf.CreateProperty("SourceId", LogSource));
            logEvent.AddOrUpdateProperty(pf.CreateProperty("Tag", Tag));
        }
    }
}
using System;
using System.Globalization;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Language;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Language;
using Serilog;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.Language
{
    public class LocaleResolver : ILocaleResolver
    {
        public LocaleResolver()
        {
        }

        public LocaleInfo Resolve(string languageCode)
        {
            try
            {
                var result = new CultureInfo(languageCode);

                if (result != null)
                {
                    while(!result.IsNeutralCulture)
                    {
                        result = result.Parent;
                    }
                    return new LocaleInfo { Id = result.LCID, Code = result.Name, Name = result.DisplayName };
                }

                return null;
            }
            catch(Exception ex)
            {
                Log.Warning($"CultureInfo {languageCode} not found", ex);
                return null;
            }
        }
    }
}

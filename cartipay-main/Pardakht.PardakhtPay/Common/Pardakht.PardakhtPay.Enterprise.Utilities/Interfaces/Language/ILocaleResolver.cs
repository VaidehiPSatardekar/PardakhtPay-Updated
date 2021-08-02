using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Language;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Language
{
    public interface ILocaleResolver
    {
        LocaleInfo Resolve(string languageCode);
    }
}

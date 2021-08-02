using System.Globalization;

namespace Pardakht.PardakhtPay.Shared.Models.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str);
        }
    }
}

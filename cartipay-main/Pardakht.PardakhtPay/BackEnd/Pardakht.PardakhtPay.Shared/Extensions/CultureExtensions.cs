using System;
using System.Globalization;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class CultureExtensions
    {
        public static string ConvertDigitChar(this string str, string sourceCode, string destinationCode)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            try
            {
                CultureInfo destination = CultureInfo.GetCultureInfoByIetfLanguageTag(destinationCode);
                CultureInfo source = CultureInfo.GetCultureInfoByIetfLanguageTag(sourceCode);

                for (int i = 0; i <= 9; i++)
                {
                    str = str.Replace(source.NumberFormat.NativeDigits[i], destination.NumberFormat.NativeDigits[i]);
                }
                return str;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string ConvertFarsiDigitCharsToEnglish(this string str)
        {
            return ConvertDigitChar(str, Helper.Farsi, Helper.English);
        }
    }
}

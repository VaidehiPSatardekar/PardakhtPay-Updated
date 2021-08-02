using System.Text;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class PhoneExtensions
    {
        public static string GetMaskedPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length <= 8)
            {
                return phoneNumber;
            }

            StringBuilder text = new StringBuilder();

            text.Append(phoneNumber.Substring(0, 6));

            text.Append("**");

            text.Append(phoneNumber.Substring(8, phoneNumber.Length - 8));

            return text.ToString();
        }
    }
}

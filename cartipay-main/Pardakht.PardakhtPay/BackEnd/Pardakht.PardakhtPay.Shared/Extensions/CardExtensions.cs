using System;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class CardExtensions
    {
        public static string MaskCardNumber(this string cardNumber)
        {
            StringBuilder masked = new StringBuilder();

            if (!string.IsNullOrEmpty(cardNumber))
            {
                if (cardNumber.Length >= 6)
                {
                    masked.Append(cardNumber.Substring(0, 6));
                }

                masked.Append("******");

                if (cardNumber.Length >= 16)
                {
                    masked.Append(cardNumber.Substring(cardNumber.Length - 4, 4));
                }
            }

            return masked.ToString();
        }

        public static bool CheckCardNumberIsValid(this string cardNumber)
        {
            if (cardNumber.Length == 16 && Regex.IsMatch(cardNumber, @"(\d){16}"))
            {
                int sum = 0;
                int counter = 0;
                int d = 0;
                foreach (char c in cardNumber)
                {
                    d = int.Parse(c.ToString()) * (counter % 2 == 0 ? 2 : 1);
                    sum += d > 9 ? d - 9 : d;
                    counter += 1;
                }
                if (sum % 10 == 0)
                {
                    return true;
                }

                if (cardNumber.StartsWith("505801"))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckIbanIsValid(this string iban)
        {
            iban = iban.ToLower().Replace("ir", "");

            if (iban.Length == 24 && Regex.IsMatch(iban, @"(\d){24}"))
            {
                var code = iban.Substring(3, 21) + "1827" + iban.Substring(0, 3);
                var num = BigInteger.Parse(code) % 97;
                if ((int)num == 10)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetBankCodeFromIban(this string iban)
        {
            iban = iban.ToLower().Replace("ir", "");
            var bankCode = iban.Substring(2, 3);
            return bankCode;
        }

        public static string GetAccountNumberFromIban(this string iban)
        {
            if (string.IsNullOrEmpty(iban))
            {
                return null;
            }

            string code = iban.GetBankCodeFromIban();

            if (code == Helper.IzBankCode)
            {
                string accountNo = $"{Convert.ToInt32(iban.Substring(8, 4))}-{Convert.ToInt32(iban.Substring(12, 3))}-{Convert.ToInt32(iban.Substring(15, 8))}-{Convert.ToInt32(iban.Substring(24, 2))}";
                return accountNo;
            }

            return null;
        }

        public static string GetBeautyCardNumber(this string cardNumber)
        {

            if (string.IsNullOrEmpty(cardNumber))
            {
                return string.Empty;
            }

            if (cardNumber.Length != 16)
            {
                return cardNumber;
            }

            return cardNumber.Substring(0, 4) + " " + cardNumber.Substring(4, 4) + " " + cardNumber.Substring(8, 4) + " " + cardNumber.Substring(12, 4);
        }
    }
}

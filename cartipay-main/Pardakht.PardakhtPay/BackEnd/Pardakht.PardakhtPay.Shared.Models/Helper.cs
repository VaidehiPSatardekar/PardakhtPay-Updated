using System.Numerics;
using System.Text.RegularExpressions;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Shared.Models
{
    public static class Helper
    {
        public const string OpenFarsi = "باز";
        public const string OpenFarsi2 = "فعال";
        public const string OpenFarsi3 = "OPEN";
        public const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        public const string IranDateTime = "Iran Standard Time";

        public const string MobileDeviceLimit = "محدودیت استفاده از 10 کارت متفاوت در روز در تراکنش کارت به کارت موفق";
        public const string MobileDeviceLimit2 = "محدودیت انجام 20 تراکنش کارت به کارت موفق در روز";
        public const string MobileDeviceLimit3 = "محدودیت انجام عملیات کارت به کارت به دلیل پر شدن سقف 10 خطای غلط بودن رمز کارت در روز جاری";
        public const string MobileDeviceInvalidRequest = "Invalid request!";
        public const string MobileUnspecifiedTransactionResult = "نتیجه تراکنش نامشخص . لطفا پیش از انجام دوباره تراکنش نسبت به عدم کسر وجه از حساب خود، با مشاهده صورتحساب روز جاری اطمینان حاصل نمایید.";

        public const string UserNotFound = "اطلاعات کاربر یافت نشد";
        public const string MobileNumberError = "شماره موبایل و شماره کارت هردو باید متعلق به یک نفر باشد";
        public const string AsanpardakhtMobileErrorMesssage = "به دستور بانک مرکزی، انجام تراکنش تنها با کارت متعلق به صاحب شماره موبایل مجاز است.";

        public const string Farsi = "fa";
        public const string English = "en";

        public const string DeviceKeyCookieName = "1a1f99f1-f987-4607-b245-43e0beabde7c";

        public const string Registered = "Registered";

        public static readonly string[] DateTimeFormats = new string[] { "dd.MM.yyyy HH:mm", "dd.MM.yyyy", "yyyy.MM.dd HH:mm", "yyyy.MM.dd" };

        public const string AgGridDateTimeFormat = "yyyy-MM-dd";

        public const string IzBankCode = "069";

        public const int MobileDeviceBlockPeriod = -24;

        public const string NotAllowedCardMessage = "سرویس کارتی پال از مبدا بانک مورد نظر در دسترس نمی باشد";

        public const string Utc = "utc";

        public const int SekehDeviceTokenRemovedCode = 509;

        public const int HamrahCardTokenRemovedCode = 102;

        public const int SesDeviceTokenRemovedCode = -12;

        public const int MydigiTokenRemovedCode = 401;

        public const int AsanpardakhtTokenRemovedCode = 1117;

        public const int SadadDeviceTokenRemovedCode = -999;

        public const int Payment780TokenRemoveCode = -123456789;

        public static bool ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }

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
            }

            return false;
        }

        public static bool ValidateIban(string iban)
        {
            if (string.IsNullOrEmpty(iban))
            {
                return false;
            }

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

        public static bool IsAccountStatusTextOpen(string status)
        {
            return status == OpenFarsi || status == OpenFarsi2 || status == OpenFarsi3;
        }

        public static string GetBankName(PaymentProviderTypes type, ProxyPaymentApiSettings settings)
        {
            switch (type)
            {
                case PaymentProviderTypes.PardakhtPal:
                    break;
                case PaymentProviderTypes.SamanPayment:
                    return settings.SamanBankParameterName;
                case PaymentProviderTypes.Meli:
                    return settings.MeliPaymentParameterName;
                case PaymentProviderTypes.Zarinpal:
                    return settings.ZarinpalParameterName;
                case PaymentProviderTypes.Mellat:
                    return settings.MellatParameterName;
                case PaymentProviderTypes.Novin:
                    return settings.NovinParameterName;
                default:
                    break;
            }

            return string.Empty;
        }

        public static class AgGridTypes
        {
            public const string Equal = "equals";

            public const string NotEquals = "notEqual";

            public const string Contains = "contains";

            public const string NotContains = "notContains";

            public const string StartsWith = "startsWith";

            public const string EndsWith = "endsWith";

            public const string LessThan = "lessThan";

            public const string LessThanOrEqual = "lessThanOrEqual";

            public const string GreaterThan = "greaterThan";

            public const string GreaterThanOrEqual = "greaterThanOrEqual";

            public const string InRange = "inRange";
        }
    }
}

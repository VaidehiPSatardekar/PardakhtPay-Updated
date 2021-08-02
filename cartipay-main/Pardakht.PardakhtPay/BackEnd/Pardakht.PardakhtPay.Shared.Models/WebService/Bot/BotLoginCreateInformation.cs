using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotLoginCreateInformation
    {
        public int Id { get; set; }

        public int BankId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        public string MobileUsername { get; set; }

        public string MobilePassword { get; set; }

        public int LoginType { get; set; }

        public string CallbackUrl { get; set; }
        public bool IsMobileLogin { get; set; }

        public long MobileNumber { get; set; }

        public int OTP { get; set; }
    }

    public class BankBotLoginUpdateInformation
    {
        public int Id { get; set; }

        public int BankId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        public string MobileUsername { get; set; }

        public string MobilePassword { get; set; }

        public int LoginType { get; set; }

        public string LoginGuid { get; set; }

        public string CallbackUrl { get; set; }

        public string SecondPassword { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }
        public int ProcessCountIn24Hrs { get; set; }

    }

    public class BankBotLoginRequestChangedDTO
    {
        public int Id { get; set; }

        public bool Success { get; set; }

        public List<string> Accounts { get; set; }
    }
}

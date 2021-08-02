namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotLoginInformation
    {
        public int Id { get; set; }

        public int BankId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int LoginType { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsWrongPassword { get; set; }

        public bool IsInvalidCaptcha { get; set; }

        public bool IsLoggedIn { get; set; }

        public bool IsActive { get; set; }

        public string LoginGuid { get; set; }

        public int LoginProcessStatus { get; set; }

        public string LoginProcessStatusDescription { get; set; }

        public string CallbackUrl { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsBlockCard { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }
        public int? ProcessCountIn24Hrs { get; set; }
        public int ConnectionProgramId { get; set; }
        public string BankConnectionProgram { get; set; }

    }

    public enum BankNames
    {
        DayBank = 1,
        IZBank = 2,
        SamanBank = 3,
        AnsarBank = 4,
        ShahrBank = 5,
        HekmatBank = 6,
        MehrIranBank = 7,
        SarmayehBank = 8,
        SinaBank = 9,
        EnBank = 10,
        Tejarat = 11,
        Ayandeh = 12,
        Mellat = 13,
        SepahBank = 99999
    }

    public class BankBotLoginRecentDetails
    {
        public int Id { get; set; }

        public int BankId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string MobileUsername { get; set; }

        public string MobilePassword { get; set; }

        public int LoginType { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsWrongPassword { get; set; }

        public bool IsActive { get; set; }

        public string LoginGuid { get; set; }

        public int LoginProcessStatus { get; set; }

        public string LoginProcessStatusDescription { get; set; }

        public bool IsDeleted { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }

        public int ConnectionProgramId { get; set; }
        public string BankConnectionProgram { get; set; }

    }
}

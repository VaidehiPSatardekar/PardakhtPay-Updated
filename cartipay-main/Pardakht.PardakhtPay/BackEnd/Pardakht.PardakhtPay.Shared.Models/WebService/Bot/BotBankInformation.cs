namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotBankInformation
    {
        public int Id { get; set; }

        public string BankName { get; set; }

        public string BankGuid { get; set; }

        public string BankCode { get; set; }

        public string BankNameInFarsi { get; set; }

        public bool IsSecondPasswordNeeded { get; set; }

        public long? ThresholdLimit { get; set; }

        public bool IsEmailAddressNeeded { get; set; }
    }
}

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotCustomerDetail
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }

        public string IBAN { get; set; }

        public string Name { get; set; }

        public string BankName { get; set; }

        public string CardNumber { get; set; }
    }
}

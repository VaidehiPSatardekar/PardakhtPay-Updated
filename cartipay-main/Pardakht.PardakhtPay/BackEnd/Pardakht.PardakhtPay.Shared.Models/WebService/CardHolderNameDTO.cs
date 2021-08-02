namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CardHolderNameDTO
    {
        public bool IsSuccess { get; set; }

        public int Id { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public int Type { get; set; }

        public string CallbackUrl { get; set; }

        public string AccountContext { get; set; }
        public int Status { get; set; }

    }
}

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class AccountStatusChangedDTO
    {
        public string CallbackUrl { get; set; }

        public string Status { get; set; }

        public string Guid { get; set; }
    }
}

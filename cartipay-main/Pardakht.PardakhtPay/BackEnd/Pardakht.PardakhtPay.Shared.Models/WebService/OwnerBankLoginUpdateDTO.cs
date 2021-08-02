namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class OwnerBankLoginUpdateDTO : BaseEntityDTO
    {
        public string FriendlyName { get; set; }

        public bool IsBlockCard { get; set; }

        public string SecondPassword { get; set; }

        public string MobileNumber { get; set; }
    }
}

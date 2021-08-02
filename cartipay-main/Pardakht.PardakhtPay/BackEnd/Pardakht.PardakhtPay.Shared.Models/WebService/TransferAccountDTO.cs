namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TransferAccountDTO : BaseEntityDTO
    {
        public string TenantGuid { get; set; }

        public string AccountNo { get; set; }

        //public string AccountHolderName { get; set; }

        public string AccountHolderFirstName { get; set; }

        public string AccountHolderLastName { get; set; }

        public string Iban { get; set; }

        public string FriendlyName { get; set; }

        public bool IsActive { get; set; }

        public string OwnerGuid { get; set; }
    }
}

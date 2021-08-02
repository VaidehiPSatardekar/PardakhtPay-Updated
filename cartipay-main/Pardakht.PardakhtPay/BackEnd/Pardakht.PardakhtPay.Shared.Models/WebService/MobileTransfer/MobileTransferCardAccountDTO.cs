namespace Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer
{
    public class MobileTransferCardAccountDTO: BaseEntityDTO
    {
        public int PaymentProviderType { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string MerchantId { get; set; }

        public string MerchantPassword { get; set; }

        public string TerminalId { get; set; }

        public string Title { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public long ThresholdAmount { get; set; }

        public string CardToCardAccountGuid { get; set; }
    }
}

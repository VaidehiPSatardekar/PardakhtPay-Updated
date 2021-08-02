namespace Pardakht.PardakhtPay.Shared.Models.MobileTransfer
{
    public class MobileTransferStartTransferModel
    {
        public string MobileNo { get; set; }

        public string FromCardNo { get; set; }

        public string ToCardNo { get; set; }

        public string Cvv2 { get; set; }

        public string CardPin { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public int Amount { get; set; }

        public int ApiType { get; set; }

        public string TransactionToken { get; set; }

        public string UniqueId { get; set; }

        public string Captcha { get; set; }
    }
}

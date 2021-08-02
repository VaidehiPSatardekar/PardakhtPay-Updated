namespace Pardakht.PardakhtPay.Shared.Models.MobileTransfer
{
    public class MobileTransferActivateDeviceModel
    {
        public string MobileNo { get; set; }

        public string VerificationCode { get; set; }

        public int ApiType { get; set; }
    }
}

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class OwnerSettingDTO : BaseEntityDTO
    {
        public string OwnerGuid { get; set; }

        public bool WaitAmountForCurrentWithdrawal { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CardToCardAccountDTO : BaseEntityDTO
    {
        public int? BankLoginId { get; set; }

        public int? BankAccountId { get; set; }

        public string AccountNo { get; set; }

        [Required]
        public string LoginGuid { get; set; }

        [Required]
        public string AccountGuid { get; set; }

        //[Required]
        public string CardNumber { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        //[Required]
        public string SafeAccountNumber { get; set; }

        public decimal TransferThreshold { get; set; }

        public bool IsActive { get; set; }

        public bool IsTransferThresholdActive { get; set; }

        public decimal? TransferThresholdLimit { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public string FriendlyName { get; set; }

        public int LoginType { get; set; }

        public bool SwitchOnLimit { get; set; }

        public bool SwitchIfHasReserveAccount { get; set; }

        public decimal? SwitchLimitAmount { get; set; }

        public decimal? SwitchCreditDailyLimit { get; set; }

        public int AccountType { get; set; }
    }

    public enum CardToCardAccountType
    {
        None = 0,
        Withdrawal = 1,
        Deposit = 2,
        Both = 3
    }
}

using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class MerchantBankAccountDTO : BaseEntityDTO
    {
        public int MerchantId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string CardNumber { get; set; }

        [Required]
        [MaxLength(2000)]
        public string AccountNumber { get; set; }

        //public int PaymentGatewayId { get; set; }

        public bool IsActive { get; set; }


        public decimal TransferTreshold { get; set; }

        //[Required]
        //[MaxLength(2000)]
        public string BusinessAccountNumber { get; set; }

        [Required]
        public string ApiKey { get; set; }
    }
}

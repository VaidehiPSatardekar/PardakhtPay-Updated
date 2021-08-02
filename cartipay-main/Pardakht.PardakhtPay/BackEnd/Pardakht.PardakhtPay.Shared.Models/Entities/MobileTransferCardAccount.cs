using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MobileTransferCardAccount : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public int PaymentProviderType { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string MerchantId { get; set; }

        [StringLength(200)]
        public string MerchantPassword { get; set; }

        [StringLength(200)]
        public string TerminalId { get; set; }

        public string Title { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public long ThresholdAmount { get; set; }

        [StringLength(100)]
        public string CardToCardAccountGuid { get; set; }
    }

    public enum PaymentProviderTypes
    {
        PardakhtPay = 1,
        PardakhtPal = 2,
        SamanPayment = 3,
        Meli = 4,
        Zarinpal = 5,
        Mellat = 6,
        Novin = 7
    }
}

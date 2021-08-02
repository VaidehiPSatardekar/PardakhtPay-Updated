using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class CardHolderName : BaseEntity
    {
        public int RecordId { get; set; }

        [StringLength(200)]
        public string CardNumber { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public string CallbackUrl { get; set; }

        public string AccountContext { get; set; }
    }

    public enum CardHolderNameType
    {
        CardNumber = 1,
        Iban = 2
    }

    public enum CardHolderRequestStatus
    {
        Incomplete = 0,
        InProgress = 1,
        Complete = 2,
        Pending = 3,
        Failed = 4,
        InSufficientTime = 5,
        InvalidOrNoSourceAccount = 6,
        InalidDestinationCardStatus = 7,
        InvalidDestinationCardNumber = 8,
        NoPermission = 9,
        TechnicalError = 10,
        SystemError = 11,
        CantTransferToDestinationCard = 12,
        InvalidIBANNumber = 13,
        TargetPasswordMissing = 14,
        BankSideErrorWhenGettingCardHolderNameByIBAN = 15
    }
}

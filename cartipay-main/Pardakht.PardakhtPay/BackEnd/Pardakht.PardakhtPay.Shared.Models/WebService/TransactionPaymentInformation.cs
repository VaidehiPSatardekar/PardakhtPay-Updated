using System;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TransactionPaymentInformation
    {
        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string CustomerCardNumber { get; set; }

        public int Amount { get; set; }

        public string ReturnUrl { get; set; }

        public int TransactionId { get; set; }

        public DateTime CreationDate { get; set; }

        public PaymentType PaymentType { get; set; }

        public int Status { get; set; }

        public string CaptchaImage { get; set; }
    }

    public class TransactionPaymentInformationWithTransaction : TransactionPaymentInformation
    {
        public Transaction Transaction { get; set; }

        public TransactionResultEnum Result { get; set; }
    }
}

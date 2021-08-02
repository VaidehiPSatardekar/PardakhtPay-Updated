using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoiceDetailDTO : BaseEntityDTO
    {
        public DateTime CreateDate { get; set; }

        public int InvoiceId { get; set; }

        public int MerchantId { get; set; }

        public int ItemTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalCount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }
    }
}

using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoiceSearchDTO
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateStr { get; set; }

        public DateTime DueDate { get; set; }

        public string OwnerGuid { get; set; }

        public DateTime StartDate { get; set; }

        public string StartDateStr { get; set; }

        public DateTime EndDate { get; set; }

        public string EndDateStr { get; set; }

        public decimal Amount { get; set; }

        public string TenantGuid { get; set; }
    }
}

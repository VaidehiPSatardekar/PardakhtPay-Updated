using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoicePaymentDTO
    {
        public int Id { get; set; }

        public string OwnerGuid { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentDateStr { get; set; }

        public string PaymentReference { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateStr { get; set; }

        public string TenantGuid { get; set; }
    }

    public class InvoicePaymentSearchArgs: AgGridSearchArgs
    {
        public string OwnerGuid { get; set; }

        public Dictionary<string, dynamic> FilterModel { get; set; }
    }
}

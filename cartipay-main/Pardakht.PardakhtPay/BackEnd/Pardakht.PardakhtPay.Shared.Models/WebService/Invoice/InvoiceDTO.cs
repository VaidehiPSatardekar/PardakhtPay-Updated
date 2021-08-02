using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoiceDTO : BaseEntityDTO
    {
        public DateTime CreateDate { get; set; }

        public DateTime DueDate { get; set; }

        public string OwnerGuid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Amount { get; set; }

        public string TenantGuid { get; set; }

        public List<InvoiceDetailDTO> Items { get; set; }
    }
}

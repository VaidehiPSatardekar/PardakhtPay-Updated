using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Invoice
{
    public class InvoiceSearchArgs: AgGridSearchArgs
    {
        public string OwnerGuid { get; set; }

        public Dictionary<string, dynamic> FilterModel { get; set; }
    }
}

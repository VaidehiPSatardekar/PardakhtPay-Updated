namespace Pardakht.PardakhtPay.Shared.Models.Invoice
{
    public class InvoiceCreateResponse
    {
        public int status { get; set; }

        public string invoice_key { get; set; }

        public int deposit_id { get; set; }
    }

    public class InvoiceErrorResponse
    {
        public int status { get; set; }

        public string errorCode { get; set; }

        public string errorDescription { get; set; }

        public string card_number { get; set; }

        public int payment_type { get; set; }

        public int inProcess { get; set; }
    }

    public class CheckInvoiceResponse
    {
        public int status { get; set; }

        public int amount { get; set; }

        public string bank_code { get; set; }

        public string card_number { get; set; }

        public int payment_type { get; set; }

        public int inProcess { get; set; }
    }
}

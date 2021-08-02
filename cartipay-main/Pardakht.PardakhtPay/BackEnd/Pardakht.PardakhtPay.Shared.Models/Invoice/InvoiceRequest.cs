using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Invoice
{
    public class InvoiceRequest
    {
        [Encrypt]
        public string api_key { get; set; }

        public int amount { get; set; }

        public string card_number { get; set; }

        public string return_url { get; set; }

        public string description { get; set; }

        public string website_name { get; set; }

        public string user_id { get; set; }

        public string user_register_date { get; set; }

        public long? user_total_deposit { get; set; }

        public long? user_total_withdraw { get; set; }

        public int? user_deposit_number { get; set; }

        public int? user_withdraw_number { get; set; }

        public int user_activity_score { get; set; }

        public string user_group_name { get; set; }

        public string user_last_activity { get; set; }

        public string reference { get; set; }

        public long? user_total_sportbook { get; set; }

        public int? user_sportbook_number { get; set; }

        public long? user_total_casino { get; set; }

        public int? user_casino_number { get; set; }

        public int? payment_type { get; set; }
    }
}

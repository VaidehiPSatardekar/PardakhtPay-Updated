using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotTransferStatusResponse
    {
        public int Id { get; set; }

        public string TransferRequestGuid { get; set; }

        public string TransferFromAccount { get; set; }

        public string TransferToAccount { get; set; }

        public decimal TransferBalance { get; set; }

        public int TransferPriority { get; set; }

        public string TransferNotes { get; set; }

        public int TransferStatus { get; set; }

        public string TransferStatusDescription { get; set; }

        public string TransferType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime TransferRequestDate { get; set; }
    }

    public class BankBotTransferReceiptRequest
    {
        public string TrackingNumber { get; set; }
    }

    public class BankBotTransferReceiptResponse
    {
        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        public string TrackingNumber { get; set; }
    }
}

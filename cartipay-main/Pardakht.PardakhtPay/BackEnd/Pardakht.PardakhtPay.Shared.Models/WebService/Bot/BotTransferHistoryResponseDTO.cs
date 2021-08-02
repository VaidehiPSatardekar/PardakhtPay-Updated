namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotTransferHistoryResponseDTO
    {
        public int Id { get; set; }

        public int TransferRequestId { get; set; }

        public string TransferType { get; set; }

        public string TransactionDateTime { get; set; }

        public string TrackingNumber { get; set; }

        public string Balance { get; set; }

        public string SourceIBANNumber { get; set; }

        public string ReceiverName { get; set; }

        public string DestinationBankName { get; set; }

        public string TransactionStatus { get; set; }

        public string PageHTML { get; set; }

        public string ScreenshotImageURL { get; set; }
    }
}

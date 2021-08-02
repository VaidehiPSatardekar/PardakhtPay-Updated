using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class TransferRequestResponse
    {
        public int Id { get; set; }
        public Guid TransferRequestGuid { get; set; }
        public string TransferFromAccount { get; set; }
        public string TransferToAccount { get; set; }
        public long TransferBalance { get; set; }
        public int TransferPriority { get; set; }
        public string TransferNotes { get; set; }
        public int TransferStatus { get; set; }
        public int TransferType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime TransferRequestDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? BankSubmissionDate { get; set; }
        public int? StatementId { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }
    }
}

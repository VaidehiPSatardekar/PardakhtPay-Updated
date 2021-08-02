using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class ManualTransferDTO : BaseEntityDTO
    {
        public int? BankLoginId { get; set; }

        public int? BankAccountId { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreationDateStr { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }
        public List<int> CardToCardAccountIds { get; set; }
        public int CardToCardAccountId { get; set; }

        public string AccountGuid { get; set; }

        public int TransferType { get; set; }

        public decimal Amount { get; set; }

        public int TransferAccountId { get; set; }

        [Encrypt]
        public string ToAccountNo { get; set; }

        [Encrypt]
        public string Iban { get; set; }

        [Encrypt]
        public string FirstName { get; set; }

        [Encrypt]
        public string LastName { get; set; }

        public int Status { get; set; }

        public int Priority { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string ProcessedDateStr { get; set; }

        public DateTime? CancelledDate { get; set; }

        public string CancelledDateStr { get; set; }

        public bool ImmediateTransfer { get; set; }

        public DateTime? ExpectedTransferDate { get; set; }

        public string ExpectedTransferDateStr { get; set; }

        public string Comment { get; set; }

        public string CreatorId { get; set; }

        public string UpdaterId { get; set; }

        public string CancellerId { get; set; }

        public bool TransferWholeAmount { get; set; }

        public List<ManualTransferDetailDTO> Details { get; set; }

     //   public string CardToCardAccountIds { get; set; }
    }

    public class ManualTransferDetailDTO : BaseEntityDTO
    {
        public int ManualTransferId { get; set; }

        public decimal Amount { get; set; }

        public string TransferGuid { get; set; }

        public int? TransferId { get; set; }

        public string TransferNotes { get; set; }

        public string TrackingNumber { get; set; }

        public int TransferStatus { get; set; }

        public DateTime? TransferDate { get; set; }

        public DateTime? TransferRequestDate { get; set; }

        public int? BankStatementId { get; set; }
    }

    public class ManualTransferSourceCardDetailsDTO 
    {
        public int ManualTransferId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int CardToCardAccountId { get; set; }

        public string AccountGuid { get; set; }
    }


    public class ManualTransferSearchArgs : AgGridSearchArgs
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string ToAccountNo { get; set; }

        public int? TransferType { get; set; }

        public int? Status { get; set; }

        public List<string> AccountGuids { get; set; }

        public Dictionary<string, dynamic> FilterModel { get; set; }
    }
}

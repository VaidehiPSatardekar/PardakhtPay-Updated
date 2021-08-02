//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;
//using Pardakht.PardakhtPay.Shared.Models.Models;

//namespace Pardakht.PardakhtPay.Shared.Models.Entities
//{
//    public class MerchantBankAccount : BaseEntity
//    {
//        public int MerchantId { get; set; }

//        [ForeignKey(nameof(MerchantId))]
//        public Merchant Merchant { get; set; }

//        [Required]
//        [MaxLength(2000)]
//        public string CardNumber { get; set; }

//        [Required]
//        [MaxLength(2000)]
//        public string AccountNumber { get; set; }

//        public bool IsActive { get; set; }

//        [Column(TypeName = "decimal(18, 2)")]

//        public decimal TransferTreshold { get; set; }

//        //[Required]
//        //[MaxLength(2000)]
//        public string BusinessAccountNumber { get; set; }

//        [Required]
//        public string ApiKey { get; set; }

//        public List<Transaction> Transactions { get; set; }
//    }
//}

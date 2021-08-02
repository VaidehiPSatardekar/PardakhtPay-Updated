using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CreateTransactionRequest
    {
        [Required]
        [Encrypt]
        public string ApiKey { get; set; }

        [Required]
        public int Amount { get; set; }

        public string CustomerCardNumber { get; set; }

        [Required]
        [Url]
        public string ReturnUrl { get; set; }

        //public string Description { get; set; }

        public string WebsiteName { get; set; }

        public string UserId { get; set; }

        public DateTime? UserRegisterDate { get; set; }

        public long? UserTotalDeposit { get; set; }

        public long? UserTotalWithdraw { get; set; }

        public int? UserDepositNumber { get; set; }

        public int? UserWithdrawNumber { get; set; }

        public int UserActivityScore { get; set; }

        public string UserGroupName { get; set; }

        public DateTime? UserLastActivity { get; set; }

        public string Reference { get; set; }

        public long? UserTotalSportbook { get; set; }

        public int? UserSportbookNumber { get; set; }

        public long? UserTotalCasino { get; set; }

        public int? UserCasinoNumber { get; set; }

        public int? PaymentType { get; set; }
    }
}

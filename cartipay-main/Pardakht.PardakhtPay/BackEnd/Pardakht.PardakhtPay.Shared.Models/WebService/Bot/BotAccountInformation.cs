using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotAccountInformation
    {
        public int Id { get; set; }

        public string AccountNo { get; set; }

        public string AccountGuid { get; set; }

        public string LoginGuid { get; set; }

        public bool IsDeleted { get; set; }

        public int LoginId { get; set; }
    }

    public class BankBotAccountCreateDTO
    {
        public int Id { get; set; }

        public int LoginId { get; set; }

        public string AccountNo { get; set; }
    }

    public class BankBotAccountWithStatusDTO
    {
        //[JsonProperty("id")]
        public int Id { get; set; }

        //[JsonProperty("lid")]
        public int LoginId { get; set; }

        //[JsonProperty("lg")]
        public string LoginGuid { get; set; }

        //[JsonProperty("an")]
        public string AccountNo { get; set; }

        //[JsonProperty("sd")]
        public string StatusDescription { get; set; }

        //[JsonProperty("ag")]
        public string AccountGuid { get; set; }

        //[JsonProperty("isd")]
        public bool IsDeleted { get; set; }

        //[JsonProperty("b")]
        public long Balance { get; set; }

        //[JsonProperty("bb")]
        public long BlockedBalance { get; set; }

        //[JsonProperty("wb")]
        public long WithdrawableBalance { get; set; }

        //[JsonProperty("wrafd")]
        public long WithdrawRemainedAmountForDay { get; set; }

        //[JsonProperty("wrafm")]
        public long WithdrawRemainedAmountForMonth { get; set; }

        //[JsonProperty("mwet")]
        public long MaximumWithdrawalOfEachTransaction { get; set; }

        //[JsonProperty("maxwet")]
        public long MinimumWithdrawalOfEachTransaction { get; set; }

        //[JsonProperty("tt")]
        public int TransferType { get; set; }

        public bool IsOpen(long blockLimit)
        {
            return !IsDeleted && Helper.IsAccountStatusTextOpen(StatusDescription) && ( blockLimit == 0 || (blockLimit >= BlockedBalance && blockLimit >= (-1 * WithdrawableBalance))  );
        }

        public long WithdrawableLimit
        {
            get
            {
                return Math.Min(WithdrawRemainedAmountForDay, WithdrawRemainedAmountForMonth);
            }
        }
    }

    public class BankBotAccountStatus
    {
        public string AcccountNumber { get; set; }

        public string Balance { get; set; }


        public string Status { get; set; }

        public string WithdrawableBalance { get; set; }

        public long BlockedAmount { get; set; }

        public bool IsOpen
        {
            get
            {
                return Helper.IsAccountStatusTextOpen(Status);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotLoginSelect
    {
        public int Id { get; set; }

        public int BankLoginId { get; set; }

        public string LoginGuid { get; set; }

        public string FriendlyName { get; set; }

        public string BankName { get; set; }

        public string OwnerGuid { get; set; }

        public string TenantGuid { get; set; }

        public int Status { get; set; }

        public int BankId { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public List<string> Accounts { get; set; }

        public int LoginRequestId { get; set; }

        public int LoginType { get; set; }

        public bool IsSecondPasswordNeeded { get; set; }

        public bool IsBlockCard { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public string MobileNumber { get; set; }

        public string QRRegistrationStatus { get; set; }

        public int? QRRegistrationId{ get; set; }

        public int? QRRegistrationStatusId { get; set; }

        public int? LoginDeviceStatusId { get; set; }
        public string BankConnectionProgram { get; set; }
    }
}

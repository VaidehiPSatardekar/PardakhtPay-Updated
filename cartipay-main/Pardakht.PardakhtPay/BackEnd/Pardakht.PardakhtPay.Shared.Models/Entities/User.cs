using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class User : BaseCreateUpdateEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastLoginIpAddress { get; set; }
    }
}

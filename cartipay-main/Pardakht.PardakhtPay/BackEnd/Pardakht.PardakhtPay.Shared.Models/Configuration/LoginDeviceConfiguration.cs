using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class LoginDeviceConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan Interval { get; set; }

    }
}

using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class CacheConfiguration
    {
        public CacheTypes CacheType { get; set; }

        public TimeSpan CacheDuration { get; set; }
    }

    public enum CacheTypes
    {
        Memory = 1,
        Redis = 2
    }
}

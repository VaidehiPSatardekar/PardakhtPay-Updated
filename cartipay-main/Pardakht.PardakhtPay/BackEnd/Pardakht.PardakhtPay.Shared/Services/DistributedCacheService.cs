using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Shared.Services
{
    public class DistributedCacheService : ICacheService
    {
        private IDistributedCache Cache { get; set; }
        private IOptions<CacheConfiguration> Configuration { get; set; }

        public DistributedCacheService(IDistributedCache cache, IOptions<CacheConfiguration> configuration)
        {
            Configuration = configuration;
            Cache = cache;
        }

        public T Get<T>(string key)
        {
            var json = Cache.GetString(key);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T Set<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var json = JsonConvert.SerializeObject(item);

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? Configuration.Value.CacheDuration
            };

            Cache.SetString(key, json, options);

            return item;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var json = await Cache.GetStringAsync(key);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<T> SetAsync<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var json = JsonConvert.SerializeObject(item);

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? Configuration.Value.CacheDuration
            };

            await Cache.SetStringAsync(key, json, options);

            return item;
        }

        public async Task Remove(string key)
        {
            await Cache.RemoveAsync(key);
        }
    }
}

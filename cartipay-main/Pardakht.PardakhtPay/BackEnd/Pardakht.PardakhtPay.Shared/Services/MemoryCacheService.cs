using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Shared.Services
{
    public class MemoryCacheService : ICacheService
    {
        MemoryCache Cache = null;
        private IOptions<CacheConfiguration> Configuration { get; set; }

        public MemoryCacheService(IOptions<CacheConfiguration> configuration)
        {
            Configuration = configuration;
            Cache = new MemoryCache(new MemoryCacheOptions()
            {
             
            });
        }

        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return Get<T>(key);
            });

            var item = await task;

            return item;
        }

        public T Set<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? Configuration.Value.CacheDuration
            };
            return Cache.Set<T>(key, item, options);
        }

        public async Task<T> SetAsync<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return Set<T>(key, item, absoluteExpirationRelativeToNow);
            });

            var cacheItem = await task;

            return cacheItem;
        }

        public async Task Remove(string key)
        {
            await Task.Run(() =>
            {
                Cache.Remove(key);
            });
        }
    }
}

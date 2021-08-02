using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<CacheService> logger;
        public CacheService(IServiceProvider serviceProvider, ILogger<CacheService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<T> ReadFromCache<T>(string key)
        {
            try
            {
                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                var itemBytes = await distributedCache.GetAsync(key);
                if (itemBytes == null)
                    return default(T);

                var itemString = Encoding.UTF8.GetString(itemBytes);
                var item = JsonConvert.DeserializeObject<T>(itemString);
                return item;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ReadFromCache Error");
                return default(T);
            }
        }

        public async Task SetCache<T>(T obj, string key, int cacheMinutes)
        {
            try
            {
                var itemString = JsonConvert.SerializeObject(obj);
                var itemBytes = Encoding.UTF8.GetBytes(itemString);

                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheMinutes));

                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                await distributedCache.SetAsync(key, itemBytes, options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SetCache Error");
            }
        }

        public async Task ClearCache(string key)
        {
            try
            {
                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                await distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ClearCache Error");
            }
        }
    }
}

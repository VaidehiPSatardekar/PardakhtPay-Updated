using System;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    /// <summary>
    /// Manages cache items. Depends on the configuration, i may be the MemoryCache or RedisCache etc.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get item by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Sets cache item with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Unique key of cache</param>
        /// <param name="item">The item will be cached</param>
        /// <param name="absoluteExpirationRelativeToNow">Expriation date of cache item. If it is null, system will use default cache expriation time</param>
        /// <returns></returns>
        T Set<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null);

        /// <summary>
        /// Gets item by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Sets item by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Unique key of cache</param>
        /// <param name="item">The item will be cached</param>
        /// <param name="absoluteExpirationRelativeToNow">Expriation date of cache item. If it is null, system will use default cache expriation time</param>
        /// <returns></returns>
        Task<T> SetAsync<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null);

        Task Remove(string key);
    }
}

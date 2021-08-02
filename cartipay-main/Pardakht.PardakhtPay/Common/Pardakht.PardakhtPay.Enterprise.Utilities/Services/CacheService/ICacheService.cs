using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.CacheService
{
    public interface  ICacheService
    {
        Task<T> ReadFromCache<T>(string key);
        Task SetCache<T>(T obj, string key, int cacheMinutes);
        Task ClearCache(string key);
    }
}

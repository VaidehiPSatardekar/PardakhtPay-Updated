using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.SqlRepository
{
    public class CachedObjectManager : ICachedObjectManager
    {
        static SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
        ICacheService _CacheService = null;
        PardakhtPayDbContext _Context = null;
        ILogger _Logger = null;
        IServiceProvider _Provider = null;

        TimeSpan _CacheTime = TimeSpan.FromMinutes(5);

        public CachedObjectManager(ICacheService cacheService,
            ILogger<CachedObjectManager> logger,
            IServiceProvider provider,
            PardakhtPayDbContext context)
        {
            _CacheService = cacheService;
            _Provider = provider;
            _Context = context;
            _Logger = logger;
        }

        public async Task ClearCachedItems<T>()
        {
            try
            {
                await _Semaphore.WaitAsync();
                var key = GetKey<T>();

                await _CacheService.Remove(key);
            }
            finally
            {
                _Semaphore.Release();
            }
        }

        public async Task<List<T>> GetCachedItems<T, TRepository>() where T : class, IEntity where TRepository: IGenericRepository<T>
        {
            var key = GetKey<T>();

            var items = _CacheService.Get<List<T>>(key);

            if (items != null)
            {
                return items;
            }

            try
            {
                await _Semaphore.WaitAsync();

                items = _CacheService.Get<List<T>>(key);

                if (items != null)
                {
                    return items;
                }

                var repository = _Provider.GetRequiredService<TRepository>();

                //var query = repository.GetQuery();

                //items = await query.AsNoTracking().ToListAsync();

                items = await repository.GetCacheItems();

                await SetItems(items);

                return items;
            }
            finally
            {
                _Semaphore.Release();
            }

        }

        private async Task SetItems<T>(List<T> items)
        {
            var key = GetKey<T>();
            _Logger.LogInformation($"Key : {key} has been added to the cache");
            await _CacheService.SetAsync(key, items, _CacheTime);
        }

        private string GetKey<T>()
        {
            return $"COL_{typeof(T).FullName}";
        }
    }
}

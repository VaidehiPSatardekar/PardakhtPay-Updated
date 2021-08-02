//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Infrastructure.Interfaces;
//using Pardakht.PardakhtPay.Shared.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.SqlRepository.Repositories
//{
//    public class TenantApiRepository : ITenantApiRepository
//    {
//        CallbackDbContext _Context = null;
//        CurrentUser _CurrentUser = null;
//        ICacheService _CacheService = null;
//        static readonly object _LockObject = new object();
//        ILogger _Logger = null;

//        public TenantApiRepository(CallbackDbContext context,
//            ICacheService cacheService,
//            CurrentUser currentUser,
//            ILogger<TenantApiRepository> logger)
//        {
//            _Context = context;
//            _CurrentUser = currentUser;
//            _CacheService = cacheService;
//            _Logger = logger;
//        }

//        public string GetCallbackUrl(string tenantGuid)
//        {
//            var items = GetCachedItems();

//            var item = items.FirstOrDefault(t => t.TenantGuid == tenantGuid && t.IsServiceUrl);

//            if(item != null)
//            {
//                return item.ApiUrl;
//            }

//            return null;
//        }

//        public async Task<int> GetMerchantId(string url)
//        {
//            var requestUri = new Uri(url);

//            string host = requestUri.Host;

//            var items = GetCachedItems();

//            items = items.Where(t => t.IsPaymentUrl && t.ApiUrl.ToLowerInvariant().Contains(host.ToLowerInvariant())).AsQueryable().ToList();

//            if (items.Count == 0)
//            {
//                return 0;
//            }

//            for (int i = 0; i < items.Count; i++)
//            {
//                var item = items[i];

//                var uri = new Uri(item.ApiUrl);

//                if (uri.Host == host)
//                {
//                    return item.MerchantId;
//                }
//            }

//            return 0;
//        }

//        public async Task<TenantApi> GetTenantApi(string url)
//        {
//            var requestUri = new Uri(url);

//            string host = requestUri.Host;

//            var items = GetCachedItems();

//            items = items.Where(t => t.ApiUrl.ToLower().Contains(host.ToLowerInvariant())).AsQueryable().ToList();

//            if (items.Count == 0)
//            {
//                return null;
//            }

//            for (int i = 0; i < items.Count; i++)
//            {
//                var item = items[i];

//                var uri = new Uri(item.ApiUrl);

//                if (uri.Host == host)
//                {
//                    return item;
//                }
//            }

//            return null;
//        }

//        public string GetTenantGuid(string url)
//        {
//            var requestUri = new Uri(url);

//            string host = requestUri.Host;

//            var items = GetCachedItems();

//            items = items.Where(t => t.ApiUrl.ToLowerInvariant().Contains(host.ToLowerInvariant())).ToList();

//            if(items.Count == 0)
//            {
//                return null;
//            }

//            for (int i = 0; i < items.Count; i++)
//            {
//                var item = items[i];

//                var uri = new Uri(item.ApiUrl);

//                if (uri.Host == host)
//                {
//                    return item.TenantGuid;
//                }
//            }

//            return null;
//        }


//        public async Task<TenantApi> InsertAsync(TenantApi item)
//        {
//            var result = await _Context.AddAsync(item);

//            return result.Entity;
//        }

//        public TenantApi Insert(TenantApi item)
//        {
//            return _Context.Add(item).Entity;
//        }

//        public TenantApi Update(TenantApi item)
//        {
//            var result = _Context.Update(item);

//            return result.Entity;
//        }

//        public async Task<TenantApi> UpdateAsync(TenantApi item)
//        {
//            var result = await Task.Run(() =>
//            {
//                return _Context.Update(item);
//            });

//            return result.Entity;
//        }

//        public async Task DeleteAsync(int id)
//        {
//            var item = await GetEntityByIdAsync(id);

//            if (item != null)
//            {
//                _Context.Remove(item);
//            }

//            ClearCache();
//        }

//        public void Delete(int id)
//        {
//            var item = GetEntityById(id);

//            if (item == null)
//            {
//                throw new Exception(string.Format("Entity {0} could not be found with Id : {1}", typeof(TenantApi).Name, id));
//            }

//            Delete(item);
//        }

//        public void Delete(TenantApi item)
//        {
//            _Context.Remove(item);
//            ClearCache();
//        }

//        public async Task DeleteAsync(TenantApi item)
//        {
//            await Task.Run(() =>
//            {
//                _Context.Remove(item);
//            });

//            ClearCache();
//        }

//        public async Task<TenantApi> GetEntityByIdAsync(int id)
//        {
//            var items = GetCachedItems();

//            var item = items.AsQueryable().FirstOrDefault(t => t.Id == id);

//            return item;
//        }

//        public TenantApi GetEntityById(int id)
//        {
//            var items = GetCachedItems();

//            var item = items.FirstOrDefault(t => t.Id == id);

//            return item;
//        }

//        public async Task<int> SaveChangesAsync()
//        {
//            return await _Context.SaveChangesAsync();
//        }

//        public IQueryable<TenantApi> GetQuery()
//        {
//            var items = GetCachedItems();

//            var query = items.Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).AsQueryable();

//            return query;
//        }

//        public async Task<List<TenantApi>> GetAllAsync()
//        {
//            var items = GetCachedItems();

//            return items.Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).AsQueryable().ToList();
//        }

//        public async Task<TenantApi> GetItemAsync(Expression<Func<TenantApi, bool>> criteria)
//        {
//            var items = GetCachedItems();

//            return items.Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).AsQueryable().FirstOrDefault(criteria);
//        }

//        public List<TenantApi> GetAll()
//        {
//            var items = GetCachedItems();

//            return items.Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).ToList();
//        }

//        public IQueryable<TenantApi> GetQuery(Expression<Func<TenantApi, bool>> criteria)
//        {
//            var items = GetCachedItems();

//            var query = items.AsQueryable().Where(criteria).Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).AsQueryable();

//            return query;
//        }

//        public int SaveChanges()
//        {
//            var result = _Context.SaveChanges();
//            ClearCache();

//            return result;
//        }

//        public async Task<List<TenantApi>> GetItemsAsync(Expression<Func<TenantApi, bool>> criteria)
//        {
//            var items = GetCachedItems();
//            var query = items.Where(t => t.TenantGuid == _CurrentUser.CurrentTenantGuid).AsQueryable().Where(criteria);

//            return query.ToList();
//        }

//        private List<TenantApi> GetCachedItems()
//        {
//            string key = "TenantApiCacheItems";

//            _Logger.LogInformation($"Before cache get");

//            var items = _CacheService.Get<List<TenantApi>>(key);

//            int count = items == null ? -1 : items.Count;

//            _Logger.LogInformation($"After cache get { count }");

//            if (items != null)
//            {
//                return items;
//            }

//            lock (_LockObject)
//            {
//                _Logger.LogInformation($"Before cache get2");

//                items = _CacheService.Get<List<TenantApi>>(key);

//                count = items == null ? -1 : items.Count;

//                _Logger.LogInformation($"After cache get2 { count }");

//                if (items != null)
//                {
//                    return items;
//                }

//                items = _Context.TenantsApis.ToList();

//                _Logger.LogInformation($"After ToList()");

//                _CacheService.Set(key, items);

//                count = items == null ? -1 : items.Count;

//                _Logger.LogInformation($"Return { count }");

//                return items;
//            }
//        }

//        private void ClearCache()
//        {
//            string key = "TenantApiCacheItems";

//            var task = _CacheService.Remove(key);

//            task.Wait();
//        }

//        public void SoftDelete(TenantApi item)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SoftDeleteAsync(TenantApi item)
//        {
//            throw new NotImplementedException();
//        }

//        public void SoftDelete(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SoftDeleteAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<List<TenantApi>> GetCacheItems()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<List<TModel>> GetModelItemsAsync<TModel>(IQueryable<TModel> query) where TModel : class
//        {
//            throw new NotImplementedException();
//        }

//        public Task<int> GetModelCountAsync<TModel>(IQueryable<TModel> query) where TModel : class
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

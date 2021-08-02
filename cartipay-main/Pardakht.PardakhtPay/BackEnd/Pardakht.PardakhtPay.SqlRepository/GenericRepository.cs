using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.SqlRepository
{
    /// <summary>
    /// Generic repository for entity objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        public PardakhtPayDbContext Context { get; set; }

        private CurrentUser _User;
        protected IServiceProvider ServiceProvider { get; private set; }

        public GenericRepository(PardakhtPayDbContext context,
            IServiceProvider provider)
        {
            Context = context;
            _User = provider.GetRequiredService<CurrentUser>();
            ServiceProvider = provider;
        }

        public async Task<T> InsertAsync(T item)
        {
            PardakhtPaySecurity(item);
            SetCreateProperties(item);
            var result = await Context.AddAsync(item);

            return result.Entity;
        }

        public T Insert(T item)
        {
            PardakhtPaySecurity(item);
            SetCreateProperties(item);
            return Context.Add(item).Entity;
        }

        public T Update(T item)
        {
            PardakhtPaySecurity(item);
            var result = Context.Update(item);

            return result.Entity;
        }

        public async Task<T> UpdateAsync(T item)
        {
            PardakhtPaySecurity(item);
            var result = await Task.Run(() =>
            {
                return Context.Update(item);
            });

            return result.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetEntityByIdAsync(id);

            if (item != null)
            {
                PardakhtPaySecurity(item);
                Context.Remove(item);
            }
        }

        public void Delete(int id)
        {
            var item = GetEntityById(id);

            if (item == null)
            {
                throw new Exception(string.Format("Entity {0} could not be found with Id : {1}", typeof(T).Name, id));
            }

            PardakhtPaySecurity(item);
            Delete(item);
        }

        public void Delete(T item)
        {
            PardakhtPaySecurity(item);
            Context.Remove(item);
        }

        public async Task DeleteAsync(T item)
        {
            await Task.Run(() =>
            {
                PardakhtPaySecurity(item);
                Context.Remove(item);
            });
        }

        public void SoftDelete(T item)
        {
            if(item is IDeletedEntity)
            {
                ((IDeletedEntity)item).IsDeleted = true;

                Update(item);
            }
        }

        public async Task SoftDeleteAsync(T item)
        {
            await Task.Run(() =>
            {
                SoftDelete(item);
            });
        }

        public void SoftDelete(int id)
        {
            var item = GetEntityById(id);

            if (item == null)
            {
                throw new Exception(string.Format("Entity {0} could not be found with Id : {1}", typeof(T).Name, id));
            }

            SoftDelete(item);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var item = await GetEntityByIdAsync(id);

            await SoftDeleteAsync(item);
        }

        public async Task<T> GetEntityByIdAsync(int id)
        {
            var query = Context.Set<T>().AsNoTracking();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            var item = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (item != null)
            {
                PardakhtPaySecurity(item);
            }

            return item;
        }

        public T GetEntityById(int id)
        {
            var query = Context.Set<T>().AsNoTracking();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            var item = query.FirstOrDefault(t => t.Id == id);

            if (item != null)
            {
                PardakhtPaySecurity(item);
            }
            return item;
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await Context.SaveChangesAsync();

            var entries = Context.ChangeTracker.Entries()
                .ToList();

            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }

            OnAfterSave();

            return result;
        }

        public IQueryable<T> GetQuery()
        {
            var query = Context.Set<T>().AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return query;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var query = Context.Set<T>().AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> criteria)
        {
            var query = Context.Set<T>().AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return await query.AsNoTracking().FirstOrDefaultAsync(criteria);
        }

        public List<T> GetAll()
        {
            var query = Context.Set<T>().AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return query.AsNoTracking().ToList();
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> criteria)
        {
            var query = Context.Set<T>().AsQueryable().Where(criteria).AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return query;
        }

        public int SaveChanges()
        {
            var result = Context.SaveChanges();

            var entries = Context.ChangeTracker.Entries()
                .ToList();

            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }

            OnAfterSave();

            return result;
        }

        private void SetCreateProperties(T item)
        {
            if (item is ICreationDateEntity)
            {
                ((ICreationDateEntity)item).CreationDate = DateTime.UtcNow;
            }
        }

        public async Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> criteria)
        {
            var query = Context.Set<T>().Where(criteria).AsQueryable();

            query = PardakhtPayFilter(query);

            query = FilterDeletedItems(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<TModel>> GetModelItemsAsync<TModel>(IQueryable<TModel> query) where TModel: class
        {
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<int> GetModelCountAsync<TModel>(IQueryable<TModel> query) where TModel : class
        {
            return await query.AsNoTracking().CountAsync();
        }

        private IQueryable<T> PardakhtPayFilter(IQueryable<T> query)
        {
            //if we want to use same database for different tenants, we will need to uncomment these lines

            //if (!string.IsNullOrEmpty(_User.CurrentTenantGuid) && typeof(ITenantGuid).IsAssignableFrom(typeof(T)))
            //{
            //    query = query.Where(t => ((ITenantGuid)t).TenantGuid == _User.CurrentTenantGuid);
            //}

            if (_User.IsProviderAdmin || _User.SeeAllOwners || _User.ApiCall)
            {
                return query;
            }

            //if (typeof(ITenantGuid).IsAssignableFrom(typeof(T)))
            //{
            //    query = query.Where(t => ((ITenantGuid)t).TenantGuid == _User.CurrentTenantGuid);
            //}

            if (!_User.IsProviderAdmin && !_User.IsTenantAdmin)
            {
                if (typeof(IOwnerGuid).IsAssignableFrom(typeof(T)))
                {
                    var guids = new List<string>();
                    guids.Add(_User.IdentifierGuid);

                    if (!string.IsNullOrEmpty(_User.ParentAccountId))
                    {
                        guids.Add(_User.ParentAccountId);
                    }

                    query = query.Where(t => guids.Contains(((IOwnerGuid)t).OwnerGuid));
                }
            }

            return query;
        }

        private void PardakhtPaySecurity(T item)
        {
            if (item == null || _User.ApiCall || _User.SeeAllOwners || _User.IsProviderAdmin)
            {
                return;
            }

            if (!_User.IsProviderAdmin && !_User.IsTenantAdmin && !_User.SeeAllOwners)
            {
                if (typeof(IOwnerGuid).IsAssignableFrom(typeof(T)))
                {
                    var ownerGuid = ((IOwnerGuid)item).OwnerGuid;

                    var guids = new List<string>();
                    guids.Add(_User.IdentifierGuid);

                    if (!string.IsNullOrEmpty(_User.ParentAccountId))
                    {
                        guids.Add(_User.ParentAccountId);
                    }

                    if (!guids.Contains(ownerGuid))
                    {
                        throw new UnauthorizedAccessException($"You don't have any permission to access this record.");
                    }

                    if (!string.IsNullOrEmpty(_User.ParentAccountId) && !_User.CanCreateOwnRecords)
                    {
                        ((IOwnerGuid)item).OwnerGuid = _User.ParentAccountId;
                    }
                }
            }
        }

        private IQueryable<T> FilterDeletedItems(IQueryable<T> query)
        {
            if (typeof(IDeletedEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(t => ((IDeletedEntity)t).IsDeleted == false);
            }

            return query;
        }

        protected virtual void OnAfterSave()
        {

        }

        public virtual async Task<List<T>> GetCacheItems()
        {
            return await GetAllAsync();
        }
    }
}

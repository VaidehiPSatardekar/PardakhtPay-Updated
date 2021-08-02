using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Domain.Base
{
    public abstract class BaseManager<T, TRepository> : IBaseManager<T> where T :class, IEntity where TRepository : IGenericRepository<T>
    {
        protected TRepository Repository { get; set; }

        public BaseManager(TRepository repository)
        {
            Repository = repository;
        }

        public T Add(T item)
        {
            return Repository.Insert(item);
        }

        public virtual async Task<T> AddAsync(T item)
        {
            return await Repository.InsertAsync(item);
        }

        public void Delete(T item)
        {
            Repository.Delete(item);
        }

        public void Delete(int id)
        {
            Repository.Delete(id);
        }

        public virtual async Task DeleteAsync(T item)
        {
            await Repository.DeleteAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            await Repository.DeleteAsync(id);
        }

        public async Task SoftDeleteAsync(int id)
        {
            await Repository.SoftDeleteAsync(id);
        }

        public List<T> GetAll()
        {
           return Repository.GetAll();
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> criteria)
        {
            return await Repository.GetItemAsync(criteria);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Repository.GetAllAsync();
        }

        public T Update(T item)
        {
            return Repository.Update(item);
        }

        public async Task<T> UpdateAsync(T item)
        {
            return await Repository.UpdateAsync(item);
        }

        public async Task<int> SaveAsync()
        {
            return await Repository.SaveChangesAsync();
        }

        public async Task<T> GetEntityByIdAsync(int id)
        {
            return await Repository.GetEntityByIdAsync(id);
        }

        public T GetEntityById(int id)
        {
            return Repository.GetEntityById(id);
        }

        public async Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> criteria)
        {
            return await Repository.GetItemsAsync(criteria);
        }

        public IQueryable<T> GetQuery()
        {
            return Repository.GetQuery();
        }
    }
}

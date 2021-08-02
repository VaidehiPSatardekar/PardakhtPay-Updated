using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    /// <summary>
    /// Generic repository with default operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class, IEntity
    {
        Task<T> InsertAsync(T item);

        T Insert(T item);

        T Update(T item);

        Task<T> UpdateAsync(T item);

        void Delete(int id);

        Task DeleteAsync(int id);

        void Delete(T item);

        Task DeleteAsync(T item);

        void SoftDelete(T item);

        Task SoftDeleteAsync(T item);

        void SoftDelete(int id);

        Task SoftDeleteAsync(int id);

        int SaveChanges();

        Task<int> SaveChangesAsync();

        IQueryable<T> GetQuery();

        IQueryable<T> GetQuery(Expression<Func<T, bool>> criteria);

        Task<T> GetItemAsync(Expression<Func<T, bool>> criteria);

        Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> criteria);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetCacheItems();

        List<T> GetAll();

        Task<T> GetEntityByIdAsync(int id);

        T GetEntityById(int id);

        Task<List<TModel>> GetModelItemsAsync<TModel>(IQueryable<TModel> query) where TModel : class;

        Task<int> GetModelCountAsync<TModel>(IQueryable<TModel> query) where TModel : class;
    }
}

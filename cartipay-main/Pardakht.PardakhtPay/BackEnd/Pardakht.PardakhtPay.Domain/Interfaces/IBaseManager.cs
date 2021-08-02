using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IBaseManager<T> where T: IEntity
    {
        T Add(T item);

        Task<T> AddAsync(T item);

        T Update(T item);

        Task<T> UpdateAsync(T item);

        void Delete(T item);

        Task DeleteAsync(T item);

        void Delete(int id);

        Task DeleteAsync(int id);

        Task SoftDeleteAsync(int id);

        Task<T> GetEntityByIdAsync(int id);

        T GetEntityById(int id);

        List<T> GetAll();

        Task<T> GetItemAsync(Expression<Func<T, bool>> criteria);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> criteria);

        Task<int> SaveAsync();

        IQueryable<T> GetQuery();
    }
}

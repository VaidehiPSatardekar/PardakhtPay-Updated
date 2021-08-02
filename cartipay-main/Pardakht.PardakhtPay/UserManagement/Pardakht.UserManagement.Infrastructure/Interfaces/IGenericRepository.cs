using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> GetByIdAsNoTracking(int id);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<int> Delete(int id);
        Task CommitChanges();

        Task<int> ExecuteSqlCommand(string query);

    }
}
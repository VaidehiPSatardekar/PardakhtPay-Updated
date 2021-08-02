using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pardakht.UserManagement.Domain
{
    public interface IBasicManager<T> where T : class
    {
        Task<IEnumerable<T>> GetList();
        Task<T> Update(int id, T model);
        Task<T> Create(T model);
        Task<T> GetDetail(int id);
    }
}
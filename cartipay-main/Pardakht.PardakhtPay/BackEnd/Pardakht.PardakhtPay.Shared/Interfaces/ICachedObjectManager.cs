using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface ICachedObjectManager
    {
        Task<List<T>> GetCachedItems<T, TRepository>() where T : class, IEntity where TRepository : IGenericRepository<T>;

        Task ClearCachedItems<T>();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application
{
    /// <summary>
    /// A generic interface for base service operations
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <typeparam name="TManager">Type of manager</typeparam>
    public interface IServiceBase<T, TManager> where T : class, IEntity
        where TManager : IBaseManager<T>
    {
        /// <summary>
        /// Returns all items async
        /// </summary>
        /// <returns></returns>
        Task<WebResponse<List<T>>> GetAllAsync();

        /// <summary>
        /// Returns the entity with given id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<WebResponse<T>> GetEntityByIdAsync(int id);

        /// <summary>
        /// Deletes the entity with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<WebResponse> DeleteAsync(int id);
    }
}

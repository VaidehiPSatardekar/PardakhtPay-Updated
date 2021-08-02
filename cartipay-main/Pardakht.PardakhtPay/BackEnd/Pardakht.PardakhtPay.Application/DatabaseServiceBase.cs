using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application
{
    /// <summary>
    /// Represents base service operations for database entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TManager"></typeparam>
    public abstract class DatabaseServiceBase<T, TManager> : ServiceBase, IServiceBase<T, TManager> where T : class, IEntity
        where TManager : IBaseManager<T>
    {
        public TManager Manager { get; set; }

        public DatabaseServiceBase(TManager manager, ILogger logger):base(logger)
        {
            Manager = manager;
            Logger = logger;
        }

        public virtual async Task<WebResponse<List<T>>> GetAllAsync()
        {
            try
            {
                var items = await Manager.GetAllAsync();

                return new WebResponse<List<T>>(true, string.Empty, items);
            }
            catch(Exception ex)
            {
                return new WebResponse<List<T>>(ex);
            }
        }

        public virtual async Task<WebResponse<T>> GetEntityByIdAsync(int id)
        {
            try
            {
                var item = await Manager.GetEntityByIdAsync(id);

                if(item == null)
                {
                    throw new Exception("Item could not be found");
                }

                return new WebResponse<T>(item);
            }
            catch (Exception ex)
            {
                return new WebResponse<T>(ex);
            }
        }

        public virtual async Task<WebResponse> DeleteAsync(int id)
        {
            try
            {
                await Manager.DeleteAsync(id);

                await Manager.SaveAsync();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }
    }
}

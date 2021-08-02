using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Application
{
    /// <summary>
    /// Base service class
    /// </summary>
    public abstract class ServiceBase
    {
        protected ILogger Logger { get; set; }

        public ServiceBase(ILogger logger)
        {

        }

        /// <summary>
        /// Base method for executing operations. We guarantee to log when any exception occurs
        /// </summary>
        /// <param name="action"></param>
        protected void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Base method for executing operations. We guarantee to log when any exception occurs
        /// </summary>
        /// <param name="action"></param>
        protected async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Base method for executing operations. We guarantee to log when any exception occurs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T Execute<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}

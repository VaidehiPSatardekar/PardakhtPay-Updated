using System;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces
{
    public interface IInvoker
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}

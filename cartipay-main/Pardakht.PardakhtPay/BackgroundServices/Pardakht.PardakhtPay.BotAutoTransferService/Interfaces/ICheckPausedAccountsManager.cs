using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.BotAutoTransferService.Interfaces
{
    public interface ICheckPausedAccountsManager
    {
        Task Run();
    }
}

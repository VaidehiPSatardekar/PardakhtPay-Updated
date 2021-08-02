using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobileTransferCardAccountGroupItemService: DatabaseServiceBase<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemManager>, IMobileTransferCardAccountGroupItemService
    {
        public MobileTransferCardAccountGroupItemService(IMobileTransferCardAccountGroupItemManager manager, ILogger<MobileTransferCardAccountGroupItemService> logger):base(manager, logger)
        {

        }
    }
}

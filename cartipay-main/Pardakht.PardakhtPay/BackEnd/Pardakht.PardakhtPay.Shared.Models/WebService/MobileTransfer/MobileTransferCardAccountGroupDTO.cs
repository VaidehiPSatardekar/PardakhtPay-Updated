using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer
{
    public class MobileTransferCardAccountGroupDTO: BaseEntityDTO
    {
        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<MobileTransferCardAccountGroupItemDTO> Items { get; set; }
    }
}

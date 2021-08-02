using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer
{
    public class MobileTransferCardAccountGroupItemDTO: BaseEntityDTO
    {
        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public int Status { get; set; }

        public List<int> UserSegmentGroups { get; set; }
    }
}

using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CardToCardAccountGroupDTO : BaseEntityDTO
    {
        public string Name { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public string Accounts { get; set; }

        public List<CardToCardAccountGroupItemDTO> Items { get; set; }

        public CardToCardAccountGroupDTO()
        {
            Items = new List<CardToCardAccountGroupItemDTO>();
        }
    }
}

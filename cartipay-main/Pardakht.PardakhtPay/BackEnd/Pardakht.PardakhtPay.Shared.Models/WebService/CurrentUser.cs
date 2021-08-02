using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CurrentUser
    {
        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public string IdentifierGuid { get; set; }

        public string ParentAccountId { get; set; }

        public List<string> LoginGuids { get; set; }

        public List<string> Roles { get; set; }

        public bool ApiCall { get; set; }

        public string Origin { get; set; }

        public string CallbackUrl { get; set; }

        public CurrentUser()
        {
            //MerchantIds = new List<int>();
            LoginGuids = new List<string>();
            Roles = new List<string>();
            //Users = new List<string>();
        }

        public bool IsProviderAdmin
        {
            get
            {
                return HasRole(Permissions.ProviderAdmin);
                //return string.IsNullOrEmpty(TenantGuid);
            }
        }

        public bool CanCreateOwnRecords
        {
            get
            {
                return HasRole(Permissions.CreateOwnRecords);
            }
        }

        public bool SeeAllOwners
        {
            get
            {
                return HasRole(Permissions.SeeAllOwners);
            }
        }

        public bool IsTenantAdmin
        {
            get
            {
                return HasRole(Permissions.TenantAdmin);
            }
        }

        public bool IsStandardUser
        {
            get
            {
                return HasRole(Permissions.StandardUser);
            }
        }

        public bool HasRole(string role)
        {
            if(Roles == null)
            {
                return false;
            }

            return Roles.Contains(role);
        }
    }
}

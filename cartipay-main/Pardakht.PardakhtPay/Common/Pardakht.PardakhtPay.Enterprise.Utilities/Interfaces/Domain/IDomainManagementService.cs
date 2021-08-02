using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Domain;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Domain
{
    public interface IDomainManagementService
    {
        Task<IEnumerable<DomainResponse>> GetAllTenantDomain();
    }
}

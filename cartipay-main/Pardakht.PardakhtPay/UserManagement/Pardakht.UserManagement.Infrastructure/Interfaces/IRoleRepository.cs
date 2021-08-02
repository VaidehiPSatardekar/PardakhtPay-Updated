using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByIdWithMappings(int roleId);
        Task<Role> GetByIdWithMappingsAsNoTracking(int roleId);
        Task<IEnumerable<Role>> Find(System.Func<Role, bool> term);
        Task<IEnumerable<Role>> GetUserRoles(int userId);
        IQueryable<Role> GetRolesWithPermissionsAsNoTracking(string platformGuid);
    }
}


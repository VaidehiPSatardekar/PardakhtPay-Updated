using System.Linq;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        //Task<IEnumerable<Permission>> GetAllIncludeDetails();
        //Task<IEnumerable<Permission>> Find(System.Func<User, bool> term);
        //Task<IEnumerable<Permission>> GetPermissionListByPlatformGuid(string[] platformGuid);
        IQueryable<PermissionGroup> GetPermissionGroupsAsNoTracking(string platformGuid);
    }
}
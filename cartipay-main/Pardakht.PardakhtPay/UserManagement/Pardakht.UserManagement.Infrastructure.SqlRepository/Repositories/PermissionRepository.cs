using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext)
        {
        }

        //public async Task<IEnumerable<Permission>> GetAllIncludeDetails()
        //{
        //    return await Task.Run(() => DbContext.Permissions.Include(p => p.PermissionGroup).AsEnumerable());
        //}

        //public async Task<IEnumerable<Permission>> Find(Func<User, bool> term)
        //{
        //    return null;
        //    // return await Task.Run(() => DbContext.Permissions.Include(p => p.PermissionGroup).Where(term).AsEnumerable());
        //}

        //public async Task<IEnumerable<Permission>> GetPermissionListByPlatformGuid(string[] platformGuid)
        //{
        //    return await Task.Run(() => DbContext.Permissions.Include(p => p.PermissionGroup));
        //}

        public IQueryable<PermissionGroup> GetPermissionGroupsAsNoTracking(string platformGuid)
        {
            return DbContext.PermissionGroups
                .Include(p => p.Permissions)
                .Where(p => p.PlatformGuid == platformGuid || p.PlatformGuid == null) // include global
                .AsNoTracking()
                .OrderBy(g => g.Name);
        }
    }
}
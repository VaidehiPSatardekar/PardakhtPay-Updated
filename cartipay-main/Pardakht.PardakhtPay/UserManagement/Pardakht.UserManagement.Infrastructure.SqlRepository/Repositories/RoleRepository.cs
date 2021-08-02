using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {

        public RoleRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Role> GetByIdWithMappingsAsNoTracking(int roleId)
        {
            return await DbContext.Roles
                .Include(rp => rp.RolePermissions)
                    .ThenInclude(p => p.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<Role> GetByIdWithMappings(int roleId)
        {
            return await DbContext.Roles
                .Include(rp => rp.RolePermissions)
                    .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<IEnumerable<Role>> Find(Func<Role, bool> term)
        {
            return await Task.Run(() => DbContext.Roles.Where(term));
        }

        public async Task<IEnumerable<Role>> GetUserRoles(int userId)
        {
            return await Task.Run(() => DbContext.UserPlatformRoles.Where(p => p.UserPlatform.StaffUserId == userId).Select(p => p.Role).AsEnumerable());
        }

        public IQueryable<Role> GetRolesWithPermissionsAsNoTracking(string platformGuid)
        {
            return DbContext.Roles
                .Include(rp => rp.RolePermissions)
                    .ThenInclude(p => p.Permission)
                .Where(r => r.PlatformGuid == platformGuid)
                .AsNoTracking();
        }
    }
}
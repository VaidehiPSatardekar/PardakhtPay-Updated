using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class StaffUserRepository : GenericRepository<StaffUser>, IStaffUserRepository
    {
        public StaffUserRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<StaffUser>> Find(Func<StaffUser, bool> term)
        {
            return await Task.Run(() => DbContext.StaffUsers
                .AsNoTracking()
                .Include(p => p.UserPlatforms)
                    .ThenInclude(p => p.UserPlatformRoles)
                        .ThenInclude(r => r.Role)
                            .ThenInclude(rp => rp.RolePermissions)
                                .ThenInclude(p => p.Permission)
                .Include(s => s.UserSuspensions)
                .Where(term));
        }

        public async Task<StaffUser> GetByIdWithMappings(int id)
        {
            return await DbContext.StaffUsers
                .Include(p => p.UserPlatforms)
                    .ThenInclude(p => p.UserPlatformRoles)
                        .ThenInclude(r => r.Role)
                            .ThenInclude(rp => rp.RolePermissions)
                                .ThenInclude(p => p.Permission)
                .Include(s => s.UserSuspensions)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StaffUser> GetByIdWithMappingsAsNoTracking(int id)
        {
            return await DbContext.StaffUsers
                .Include(p => p.UserPlatforms)
                    .ThenInclude(p => p.UserPlatformRoles)
                        .ThenInclude(r => r.Role)
                            .ThenInclude(rp => rp.RolePermissions)
                                .ThenInclude(p => p.Permission)
                .Include(s => s.UserSuspensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StaffUser> GetByAccountWithMappingsAsNoTracking(string accountId)
        {
            return await DbContext.StaffUsers
                .Include(p => p.UserPlatforms)
                    .ThenInclude(p => p.UserPlatformRoles)
                        .ThenInclude(r => r.Role)
                            .ThenInclude(rp => rp.RolePermissions)
                                .ThenInclude(p => p.Permission)
                .Include(s => s.UserSuspensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.AccountId == accountId);
        }

        public List<Role> GetRoles()
        {
            return  DbContext.Roles.ToList();
        }


        public IQueryable<StaffUser> GetAllUsersWithMappings()
        {
            return DbContext.StaffUsers
                .Include(p => p.UserPlatforms)
                    .ThenInclude(p => p.UserPlatformRoles)
                        .ThenInclude(r => r.Role)
                            .ThenInclude(rp => rp.RolePermissions)
                                .ThenInclude(p => p.Permission)
                .Include(s => s.UserSuspensions)
                .AsNoTracking();
        }

        public IQueryable<UserSuspension> GetUserSuspensions()
        {
            return DbContext.UserSuspensions;
        }

        public async Task AddSuspension(UserSuspension suspension)
        {
            await this.DbContext.UserSuspensions.AddAsync(suspension);
        }
    }
}

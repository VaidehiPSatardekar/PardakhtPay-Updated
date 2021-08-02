using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IStaffUserRepository : IGenericRepository<StaffUser>
    {
        List<Role> GetRoles();
        Task<IEnumerable<StaffUser>> Find(System.Func<StaffUser, bool> term);
        Task<StaffUser> GetByAccountWithMappingsAsNoTracking(string accountId);
        Task<StaffUser> GetByIdWithMappings(int id);
        Task<StaffUser> GetByIdWithMappingsAsNoTracking(int id);
        IQueryable<StaffUser> GetAllUsersWithMappings();
        IQueryable<UserSuspension> GetUserSuspensions();
        Task AddSuspension(UserSuspension suspension);

    }
}

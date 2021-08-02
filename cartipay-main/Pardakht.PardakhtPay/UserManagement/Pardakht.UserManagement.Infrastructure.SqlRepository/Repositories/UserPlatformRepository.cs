using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class UserPlatformRepository : GenericRepository<UserPlatform>, IUserPlatformRepository
    {
        public UserPlatformRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext)
        {
        }
    }
}

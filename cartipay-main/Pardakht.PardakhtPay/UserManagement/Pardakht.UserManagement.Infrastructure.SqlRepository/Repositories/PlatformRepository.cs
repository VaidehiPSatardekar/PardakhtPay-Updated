using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class PlatformRepository : GenericRepository<Platform>, IPlatformRepository
    {
        public PlatformRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Platform> GetByPlatformGuid(string platformGuid)
        {
            return await Task.Run(() => DbContext.Platforms.FirstOrDefault(p=>p.PlatformGuid == platformGuid));
        }
    }
}

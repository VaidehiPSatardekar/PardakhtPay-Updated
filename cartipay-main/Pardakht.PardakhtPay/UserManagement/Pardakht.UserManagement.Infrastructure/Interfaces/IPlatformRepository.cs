using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IPlatformRepository : IGenericRepository<Platform>
    {
        Task<Platform> GetByPlatformGuid(string platformGuid);
    }
}


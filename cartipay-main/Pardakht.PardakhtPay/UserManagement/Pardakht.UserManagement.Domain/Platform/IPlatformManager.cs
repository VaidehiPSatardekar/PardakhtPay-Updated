using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.Platform
{
    public interface IPlatformManager
    {
        Task<IEnumerable<PlatformDto>> GetList();
        Task<PlatformDto> GetByPlatformGuid(string platformGuid);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.Platform
{
    public class PlatformManager : IPlatformManager
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformManager(IPlatformRepository platformRepository)
        {
            _platformRepository = platformRepository;
        }


        public async Task<IEnumerable<PlatformDto>> GetList()
        {
            var response = await _platformRepository.GetAll();
            return Mapper.Map<IEnumerable<PlatformDto>>(response);
        }

        public async Task<PlatformDto> GetByPlatformGuid(string platformGuid)
        {
            var response = await _platformRepository.GetByPlatformGuid(platformGuid);
            return Mapper.Map<PlatformDto>(response);
        }
    }
}

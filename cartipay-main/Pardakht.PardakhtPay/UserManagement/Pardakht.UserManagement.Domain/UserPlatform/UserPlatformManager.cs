using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.UserPlatform
{
    public class UserPlatformManager :  IUserPlatformManager
    {
        private readonly IUserPlatformRepository userPlatformRepository;
        public UserPlatformManager(IUserPlatformRepository userPlatformRepository)
        {
            this.userPlatformRepository = userPlatformRepository;
        }
        public async Task<UserPlatformDto> Create(UserPlatformDto model)
        {
            try
            {
                var entity = Mapper.Map<Shared.Models.Infrastructure.UserPlatform>(model);
                var response = await userPlatformRepository.Create(entity);
                await userPlatformRepository.CommitChanges();
                return Mapper.Map<UserPlatformDto>(response);
            }
            catch (System.Exception ex)
            {

                throw;
            }
  
        }

        public Task<UserPlatformDto> GetDetail(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<UserPlatformDto>> GetList()
        {
            throw new System.NotImplementedException();
        }

        public Task<UserPlatformDto> Update(int id, UserPlatformDto model)
        {
            throw new System.NotImplementedException();
        }
    }
}

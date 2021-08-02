using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;
using InfrastructureModels = Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public abstract class StaffUserEditBase
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IRoleRepository roleRepository;

        public StaffUserEditBase(IStaffUserRepository staffUserRepository, IRoleRepository roleRepository)
        {
            this.staffUserRepository = staffUserRepository;
            this.roleRepository = roleRepository;
        }

        public async Task AddPlatformAndRoleMappings(IEnumerable<DomainModels.StaffUserEditPlatformRoleContainer> platformRoleMappings, InfrastructureModels.StaffUser user)
        {
            if (platformRoleMappings != null)
            {
                user.UserPlatforms = new List<InfrastructureModels.UserPlatform>();
                foreach (var mapping in platformRoleMappings)
                {
                    // TODO: validate platform
                    var userPlatform = new InfrastructureModels.UserPlatform
                    {
                        PlatformGuid = mapping.PlatformGuid,
                        UserPlatformRoles = new List<InfrastructureModels.UserPlatformRole>()
                    };

                    // add to roles
                    if (mapping.Roles != null)
                    {
                        foreach (var roleMapping in mapping.Roles)
                        {
                            var role = await roleRepository.GetByIdAsNoTracking(roleMapping);
                            if (role == null)
                            {
                                throw new Exception($"Invalid role {roleMapping}");
                            }
                            if (!string.IsNullOrEmpty(role.PlatformGuid) && role.PlatformGuid != mapping.PlatformGuid)
                            {
                                throw new Exception($"Invalid role for platform: role = {role.Name}, platform = {mapping.PlatformGuid}");
                            }
                            userPlatform.UserPlatformRoles.Add(new InfrastructureModels.UserPlatformRole
                            {
                                RoleId = role.Id,
                            });
                        }
                    }

                    user.UserPlatforms.Add(userPlatform);
                }
            }
        }
    }
}

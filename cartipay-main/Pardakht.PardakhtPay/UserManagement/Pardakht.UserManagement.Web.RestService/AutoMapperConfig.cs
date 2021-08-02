using AutoMapper;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Web.RestService
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Permission, PermissionDto>();
                cfg.CreateMap<PermissionDto, Permission>();

                cfg.CreateMap<UserPlatform, UserPlatformDto>();
                cfg.CreateMap<UserPlatformDto, UserPlatform>();

                cfg.CreateMap<UserPlatformRole, UserPlatformRoleDto>();
                cfg.CreateMap<UserPlatformRoleDto, UserPlatformRole>();

                cfg.CreateMap<PermissionGroup, PermissionGroupDto>();
                cfg.CreateMap<PermissionGroupDto, PermissionGroup>();

                cfg.CreateMap<Role, RoleDto>();
                cfg.CreateMap<RoleDto, Role>();

            });
        }
    }
}

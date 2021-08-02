using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class RolePermissionSeedData : ISeedData
    {
        private readonly ParadakhtUserManagementDbContext _dbContext;
        public RolePermissionSeedData(IServiceProvider service)
        {
            _dbContext = service.GetRequiredService<ParadakhtUserManagementDbContext>();
        }

        public RolePermissionSeedData(ParadakhtUserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed(bool clearExistingData)
        {
            if (clearExistingData)
            {
                ClearData();
            }

            //_dbContext.RolePermissions.Add(new RolePermission
            //{
            //    Permission = _dbContext.Permissions.FirstOrDefault(p => p.Name == "Customer management access"),
            //    //PlatformGuid = "CartiPayGuid",
            //    Role = _dbContext.Roles.FirstOrDefault(p => p.Name == "Admin")
            //});

            //_dbContext.RolePermissions.Add(new RolePermission
            //{
            //    Permission = _dbContext.Permissions.FirstOrDefault(p=>p.Name == "Customer management access"),
            //    //PlatformGuid = "GamingGuid",
            //    Role = _dbContext.Roles.FirstOrDefault(p=>p.Name == "Admin")
            //});

            //_dbContext.RolePermissions.Add(new RolePermission
            //{
            //    Permission = _dbContext.Permissions.FirstOrDefault(p => p.Name == "Customer management access"),
            //    //PlatformGuid = "GamingGuid",
            //    Role = _dbContext.Roles.FirstOrDefault(p => p.Name == "Admin")
            //});

            //_dbContext.RolePermissions.Add(new RolePermission
            //{
            //    Permission = _dbContext.Permissions.FirstOrDefault(p => p.Name == "Tenant management access"),
            //    //PlatformGuid = "GamingGuid",
            //    Role = _dbContext.Roles.FirstOrDefault(p => p.Name == "Admin")
            //});




            _dbContext.SaveChanges();

        }

        private void ClearData()
        {
           
        }
    }
}

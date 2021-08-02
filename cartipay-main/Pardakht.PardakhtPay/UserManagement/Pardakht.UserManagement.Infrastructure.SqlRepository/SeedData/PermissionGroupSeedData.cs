using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class PermissionGroupSeedData : ISeedData
    {
        private readonly ParadakhtUserManagementDbContext _dbContext;
        public PermissionGroupSeedData(IServiceProvider service)
        {
            _dbContext = service.GetRequiredService<ParadakhtUserManagementDbContext>();
        }

        public PermissionGroupSeedData(ParadakhtUserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed(bool clearExistingData)
        {
            if (clearExistingData)
            {
                ClearData();
            }

            _dbContext.PermissionGroups.Add(new PermissionGroup
            {
                Name = "Customer Management",
                PlatformGuid = "PardakhtPayGuid"
            });

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Customer Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Tenant Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Provider Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Staff Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Role Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Content Management",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.PermissionGroups.Add(new PermissionGroup
            //{
            //    Name = "Dashboard Management",
            //    PlatformGuid = "GamingGuid"
            //});

            _dbContext.SaveChanges();

        }

        private void ClearData()
        {
            var permissions = _dbContext.Permissions.ToList();
            foreach (var item in permissions)
            {
                _dbContext.Permissions.Remove(item);
            }

            var domains = _dbContext.PermissionGroups.ToList();
            foreach (var item in domains)
            {
                _dbContext.PermissionGroups.Remove(item);
            }
        }
    }
}

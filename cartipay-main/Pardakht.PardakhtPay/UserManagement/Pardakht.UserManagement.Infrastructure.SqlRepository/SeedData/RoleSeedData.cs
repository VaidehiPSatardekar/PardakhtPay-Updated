using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class RoleSeedData : ISeedData
    {
        private readonly ParadakhtUserManagementDbContext _dbContext;


        public RoleSeedData(IServiceProvider service)
        {
            _dbContext = service.GetRequiredService<ParadakhtUserManagementDbContext>();
        }

        public RoleSeedData(ParadakhtUserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed(bool clearExistingData)
        {
            if (clearExistingData)
            {
                ClearData();
            }

            _dbContext.Roles.Add(new Role
            {
                IsFixed = true,
                Name = "Admin",
                RoleHolderTypeId = "P",
                PlatformGuid = "PardakhtPayGuid"
            });


            _dbContext.Roles.Add(new Role
            {
                IsFixed = true,
                Name = "Admin",
                RoleHolderTypeId = "T",
                PlatformGuid = "PardakhtPayGuid"
            });

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Admin",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Supervisor-Support",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Agent-Support",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Supervisor-Sales",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Agent-Sales",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Admin",
            //    RoleHolderTypeId = "T",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Supervisor-Support",
            //    RoleHolderTypeId = "T",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Agent-Support",
            //    RoleHolderTypeId = "T",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Supervisor-Sales",
            //    RoleHolderTypeId = "T",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Agent-Sales",
            //    RoleHolderTypeId = "T",
            //    PlatformGuid = "GamingGuid"
            //});

            //_dbContext.Roles.Add(new Role
            //{
            //    IsFixed = true,
            //    Name = "Supervistor-plus",
            //    RoleHolderTypeId = "P",
            //    PlatformGuid = "GamingGuid"
            //});

            _dbContext.SaveChanges();

        }

        private void ClearData()
        {
            var domains = _dbContext.Roles.ToList();
            foreach (var item in domains)
            {
                _dbContext.Roles.Remove(item);
            }
        }
    }
}

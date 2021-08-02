using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class PermissionSeedData : ISeedData
    {
        private readonly ParadakhtUserManagementDbContext _dbContext;
        public PermissionSeedData(IServiceProvider service)
        {
            _dbContext = service.GetRequiredService<ParadakhtUserManagementDbContext>();
        }

        public PermissionSeedData(ParadakhtUserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed(bool clearExistingData)
        {
            if (clearExistingData)
            {
                ClearData();
            }

            //_dbContext.Permissions.Add(new Permission
            //{
            //    Name = "Customer management access",
            //    Code = "CUST-ACCESS",
            //    IsRestricted = false,
            //    PermissionGroup = _dbContext.PermissionGroups.FirstOrDefault(p=>p.Name == "Customer Management")
            //});

            //_dbContext.Permissions.Add(new Permission
            //{
            //    Name = "Registration form customisation",
            //    Code = "CUST-REG-FORM",
            //    IsRestricted = false,
            //    PermissionGroup = _dbContext.PermissionGroups.FirstOrDefault(p => p.Name == "Customer Management")
            //});

            //_dbContext.Permissions.Add(new Permission
            //{
            //    Name = "Tenant management access",
            //    Code = "TNT-ACCESS",
            //    IsRestricted = false,
            //    PermissionGroup = _dbContext.PermissionGroups.FirstOrDefault(p => p.Name == "Tenant Management")
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

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class PlatformSeedData : ISeedData
    {
        private readonly ParadakhtUserManagementDbContext _dbContext;
        public PlatformSeedData(IServiceProvider service)
        {
            _dbContext = service.GetRequiredService<ParadakhtUserManagementDbContext>();
        }

        public PlatformSeedData(ParadakhtUserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed(bool clearExistingData)
        {
            if (clearExistingData)
            {
                ClearData();
            }

            //_dbContext.Platforms.Add(new Platform
            //{
            //    Name = "Gaming",
            //    PlatformGuid = "GamingGuid",
            //    JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            //});

            //_dbContext.Platforms.Add(new Platform
            //{
            //    Name = "TenantManagement",
            //    PlatformGuid = "TenantManagementGuid",
            //    JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            //});


            _dbContext.Platforms.Add(new Platform
            {
                Name = "UserManagement",
                PlatformGuid = "UserManagementGuid",
                JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            });


            //_dbContext.Platforms.Add(new Platform
            //{
            //    Name = "HelpDesk",
            //    PlatformGuid = "HelpDeskGuid",
            //    JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            //});


            //_dbContext.Platforms.Add(new Platform
            //{
            //    Name = "DomainManagement",
            //    PlatformGuid = "DomainManagementGuid",
            //    JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            //});


            _dbContext.Platforms.Add(new Platform
            {
                Name = "PardakhtPay",
                PlatformGuid = "PardakhtPayGuid",
                JwtKey = "sBIFolm7sxEIC_TlThmO8xNS4mB5orl-45gOJR_stew"
            });


            _dbContext.SaveChanges();

        }

        private void ClearData()
        {
           
        }
    }
}

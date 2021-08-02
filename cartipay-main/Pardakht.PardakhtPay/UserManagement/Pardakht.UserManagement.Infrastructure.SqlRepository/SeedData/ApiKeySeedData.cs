using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class ApiKeySeedData : ISeedData
    {
        private readonly AccountRepository _accountDbContext;
        private readonly ParadakhtUserManagementDbContext _dbContext;

        public ApiKeySeedData(DbContextOptions<AccountRepository> options, DbContextOptions<ParadakhtUserManagementDbContext> dbContextOptions, IConnectionStringManager manager)
        {
            _accountDbContext = new AccountRepository(options, manager);
            _dbContext = new ParadakhtUserManagementDbContext(dbContextOptions);

        }

        private async Task CreateApiKeyUser(string apiKey, string platformGuid)
        {
            var userStore = new UserStore<ApplicationUser>(_accountDbContext);

            var apiKeyUserManagement = new ApplicationUser
            {
                UserName = apiKey,
                NormalizedUserName = apiKey,
                EmailConfirmed = true,
                LockoutEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userStore.CreateAsync(apiKeyUserManagement);
            var newUser = await userStore.FindByNameAsync(apiKey);
            _dbContext.StaffUsers.Add(new StaffUser
            {
                AccountId = newUser.Id,
                Username = apiKey,
                UserType = UserType.ApiUser,
                UserPlatforms = new List<UserPlatform>
                {
                    new UserPlatform
                    {
                        PlatformGuid = platformGuid

                    }
                }
            });

            _dbContext.SaveChanges();
            _accountDbContext.SaveChanges();
        }


        public async Task Seed(bool clearExistingData)
        {
            await CreateApiKeyUser("api_key_user_management", "UserManagementGuid");
            //await CreateApiKeyUser("api_key_tenant_management", "TenantManagementGuid");
            await CreateApiKeyUser("api_key_carti_pay", "PardakhtPayGuid");
            //await CreateApiKeyUser("api_key_sports_book", "GamingGuid");
            //await CreateApiKeyUser("api_key_domain_management", "DomainManagementGuid");
            //await CreateApiKeyUser("api_key_help_desk_management", "HelpDeskGuid");



        }

    }
}

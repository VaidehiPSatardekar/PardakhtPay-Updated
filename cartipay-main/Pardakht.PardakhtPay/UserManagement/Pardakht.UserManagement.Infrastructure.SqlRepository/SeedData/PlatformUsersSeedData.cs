using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public class PlatformUsersSeedData : ISeedData
    {
        private readonly AccountRepository _accountDbContext;
        private readonly ParadakhtUserManagementDbContext _paradakhtUserManagementDbContext;

        public PlatformUsersSeedData(DbContextOptions<AccountRepository> options, DbContextOptions<ParadakhtUserManagementDbContext> dbContextOptions, IConnectionStringManager manager)
        {
            _accountDbContext = new AccountRepository(options, manager);
            _paradakhtUserManagementDbContext = new ParadakhtUserManagementDbContext(dbContextOptions);

        }

        private async Task CreateUsers(string prefixPlatformName, string userPassword, string platformGuid)
        {
            try
            {
                var sportsBookProviderAdmin = new ApplicationUser
                {
                    UserName = $"{prefixPlatformName}_providerAdmin",
                    NormalizedUserName = $"{prefixPlatformName}_providerAdmin",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(sportsBookProviderAdmin, userPassword);
                sportsBookProviderAdmin.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(_accountDbContext);
                var createResponse = await userStore.CreateAsync(sportsBookProviderAdmin);
                await _accountDbContext.SaveChangesAsync();


                var newUser = await userStore.FindByNameAsync($"{prefixPlatformName}_providerAdmin");
                var providerAdminRole = _paradakhtUserManagementDbContext.Roles.FirstOrDefault(p => p.Name == "Admin" && p.RoleHolderTypeId == "P" && p.PlatformGuid == platformGuid);
                _paradakhtUserManagementDbContext.StaffUsers.Add(new StaffUser
                {
                    AccountId = newUser.Id,
                    Email = $"{prefixPlatformName}_providerAdmin@{prefixPlatformName}.com",
                    FirstName = $"{prefixPlatformName}_providerAdmin",
                    LastName = $"{prefixPlatformName}_providerAdmin",
                    Username = $"{prefixPlatformName}_providerAdmin",
                    UserType = UserType.StaffUser,
                    UserPlatforms = new List<UserPlatform>
                {
                    new UserPlatform
                    {
                        PlatformGuid = platformGuid,
                        UserPlatformRoles = new List<UserPlatformRole>
                        {
                            new UserPlatformRole
                            {
                                Role = providerAdminRole
                            }
                        }
                    }
                }
                });

                await _paradakhtUserManagementDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task Seed(bool clearExistingData)
        {
            try
            {
                //await CreateUsers("gaming", "P@ssword123", "GamingGuid");
                await CreateUsers("pardakhtPay", "P@ssword123", "PardakhtPayGuid");

              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        



        }

    }
}

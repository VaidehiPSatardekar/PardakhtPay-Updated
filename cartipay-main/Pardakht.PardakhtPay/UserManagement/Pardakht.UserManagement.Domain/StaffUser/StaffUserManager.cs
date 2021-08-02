using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Domain.StaffUser.Handlers;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser
{
    public class StaffUserManager : IStaffUserManager
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IAuditLogManager auditLogManager;
        private readonly ILogger<StaffUserManager> logger;

        public StaffUserManager(IStaffUserRepository staffUserRepository, IRoleRepository roleRepository, IAuditLogManager auditLogManager, ILogger<StaffUserManager> logger)
        {
            this.staffUserRepository = staffUserRepository;
            this.roleRepository = roleRepository;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the appropriate staff user. When using internally, use tenantId to determine whether user is a provider or tenant rather than tenantGuid
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="tenantPlatformMapGuid">Populate this if this is going to be used externally</param>
        /// <returns></returns>
        public async Task<DomainModels.StaffUser> GetByAccountId(string accountId, string tenantPlatformMapGuid, string platformGuid, bool all = false)
        {
            var response = await staffUserRepository.GetByAccountWithMappingsAsNoTracking(accountId);

            return response == null || (response.IsDeleted && !all) ? null : UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<DomainModels.StaffUser> GetById(int id, string platformGuid)
        {
            var response = await staffUserRepository.GetByIdWithMappingsAsNoTracking(id);

            return response == null || response.IsDeleted ? null : UserMapper.Map(response, string.Empty, platformGuid);
        }

        public async Task<DomainModels.StaffUser> GetByEmail(string email)
        {
            var response = await staffUserRepository
                .Find(p => p.Email != null && p.Email.ToLower() == email.ToLower() && !p.IsDeleted
            );

            var user = response.FirstOrDefault();

            // TODO: this may need to be modified to include platform mapping - at the moment it is only being used to check if a user exists in the entire system
            return user == null ? null : UserMapper.Map(user, string.Empty, string.Empty);
        }

        public async Task<DomainModels.StaffUser> GetByUsername(string username)
        {
            var response = await staffUserRepository
                .Find(p => p.Username != null && p.Username.ToLower() == username.ToLower() && !p.IsDeleted
            );

            var user = response.FirstOrDefault();

            // TODO: this may need to be modified to include platform mapping - at the moment it is only being used to check if a user exists in the entire system
            return user == null ? null : UserMapper.Map(user, string.Empty, string.Empty);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetAllStaffUsers(string platformGuid)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid) && !u.IsDeleted)
                .ToListAsync();

            return UserMapper.Map(response, string.Empty, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsers(string platformGuid)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && !u.IsDeleted
                    && u.TenantId == null)
                .ToListAsync();

            return UserMapper.Map(response, string.Empty, platformGuid);
        }

        public async Task<Shared.Models.Infrastructure.StaffUser> GetStaffUserByAccountId(string accountId)
        {
            var response = await staffUserRepository.GetByAccountWithMappingsAsNoTracking(accountId);
            return response;
        }

        public List<Shared.Models.Infrastructure.Role> GetRoles()
        {
            return  staffUserRepository.GetRoles();
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsersByUserTypeAsync(string platformGuid, UserType userType)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                                                    && !u.IsDeleted
                                                    && u.TenantId == null
                                                    && u.UserType == userType)
                .ToListAsync().ConfigureAwait(false);

            return UserMapper.Map(response, string.Empty, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsersByUserTypeAsync(string platformGuid, UserType userType, int? brandId)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                                                    && !u.IsDeleted
                                                    && u.BrandId == brandId
                                                    && u.TenantId == null
                                                    && u.UserType == userType)
                .ToListAsync().ConfigureAwait(false);

            return UserMapper.Map(response, string.Empty, platformGuid);
        }
        public async Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsersByUserTypeAsync(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, UserType userType)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                                                    && u.TenantId == tenantId
                                                    && !u.IsDeleted
                                                    && u.UserType == userType
                                                    && ((accountGuid != null && u.ParentAccountId == accountGuid) || allOwners || u.AccountId == accountGuid))
                .ToListAsync().ConfigureAwait(false);

            return UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }
        public async Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsersByUserTypeAsync(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, UserType userType, int? brandId)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                                                    && u.TenantId == tenantId
                                                    && u.BrandId == brandId
                                                    && !u.IsDeleted
                                                    && u.UserType == userType
                                                    && ((accountGuid != null && u.ParentAccountId == accountGuid) || allOwners || u.AccountId == accountGuid))
                .ToListAsync().ConfigureAwait(false);

            return UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }
        public async Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsers(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, int? brandId = null)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && u.TenantId == tenantId
                    && u.BrandId == brandId
                    && !u.IsDeleted
                    && u.UserType != UserType.AffiliateUser
                    && ((accountGuid != null && u.ParentAccountId == accountGuid) || allOwners || u.AccountId == accountGuid))
                .ToListAsync();

            return UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetAllUsersWithPermission(string platformGuid, string permissionCode)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null
                    && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && !u.IsDeleted
                    && u.UserPlatforms.SelectMany(upr => upr.UserPlatformRoles).SelectMany(rp => rp.Role.RolePermissions).Any(p => p.Permission.Code == permissionCode))
                .ToListAsync();

            return UserMapper.Map(response, string.Empty, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetUsersWithPermission(string platformGuid, int? tenantId, string permissionCode, string tenantPlatformMapGuid)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null
                    && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && (tenantId.HasValue && u.TenantId == tenantId || !tenantId.HasValue)
                    && (!tenantId.HasValue && u.TenantId == null || tenantId.HasValue)
                    && !u.IsDeleted
                    && u.UserPlatforms.SelectMany(upr => upr.UserPlatformRoles).SelectMany(rp => rp.Role.RolePermissions).Any(p => p.Permission.Code == permissionCode))
                .ToListAsync();

            return UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetAllUsersWithPermissions(string platformGuid, IList<string> permissionCodes)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null
                    && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && !u.IsDeleted
                    && u.UserPlatforms.SelectMany(upr => upr.UserPlatformRoles).SelectMany(rp => rp.Role.RolePermissions).Any(p => permissionCodes.Contains(p.Permission.Code)))
                .ToListAsync();

            return UserMapper.Map(response, string.Empty, platformGuid);
        }
        public async Task<Shared.Models.Infrastructure.StaffUser> GetByIdWithMappings(int id)
        {
            return await staffUserRepository.GetByIdWithMappings(id);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> GetUsersWithPermissions(string platformGuid, int? tenantId, IList<string> permissionCodes, string tenantPlatformMapGuid)
        {
            var response = await staffUserRepository.GetAllUsersWithMappings()
                .Where(u => u.UserPlatforms != null
                    && u.UserPlatforms.Any(up => up.PlatformGuid == platformGuid)
                    && (tenantId.HasValue && u.TenantId == tenantId || !tenantId.HasValue)
                    && (!tenantId.HasValue && u.TenantId == null || tenantId.HasValue)
                    && !u.IsDeleted
                    && u.UserPlatforms.SelectMany(upr => upr.UserPlatformRoles).SelectMany(rp => rp.Role.RolePermissions).Any(p => permissionCodes.Contains(p.Permission.Code)))
                .ToListAsync();

            return UserMapper.Map(response, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUserStatusLastLogin>> GetStaffUsersStatusLastLogin(string tenantGuid, string platformGuid, IEnumerable<int> staffUserIds, bool isProviderUser, bool all = false)
        {
            string term = string.Empty;
            var auditLogs = (IEnumerable<Shared.Models.Infrastructure.AuditLog>)null;
            if (isProviderUser && all)
            {
                //term = $"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
                auditLogs = await this.auditLogManager.GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType.User,
                                                                                                 AuditActionType.UserPerformanceActivity,
                                                                                                 staffUserIds,
                                                                                                 platformGuid,
                                                                                                 null);
            }
            else
            {
                auditLogs = await this.auditLogManager.GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType.User,
                                                                                                 AuditActionType.UserPerformanceActivity,
                                                                                                 staffUserIds,
                                                                                                 platformGuid,
                                                                                                 tenantGuid);
                //term = $"%\"{nameof(AuditLogStaffUserTimePerformance.TenantPlatformMapGuid)}\":\"{tenantGuid}\"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
            }
            //var auditLogs = await this.auditLogManager.GetStatusLastLoginByTermAndStaffUserIds(AuditType.User, AuditActionType.UserPerformanceActivity, term, staffUserIds);

            var result = new List<DomainModels.StaffUserStatusLastLogin>();
            if (!string.IsNullOrEmpty(platformGuid))
            {
                auditLogs.ToList().ForEach(m =>
                {
                    //var performance = JsonConvert.DeserializeObject<AuditLogStaffUserTimePerformance>(m.Message);
                    var staffUserPerformanceTime = new DomainModels.StaffUserStatusLastLogin
                    {
                        Id = m.TypeId,
                        LoginDateTime = m.DateTime,
                        IsActive = m.IsActive && m.DateTime > DateTime.UtcNow.AddMinutes(-60),
                        LogonTime = m.LogonTime,
                        ActiveTime = m.ActiveTime,
                        IdleTime = m.IdleTime
                    };
                    result.Add(staffUserPerformanceTime);
                });

            }
            return result;
        }

        public async Task<IEnumerable<DomainModels.StaffUserPerformanceTime>> GetStaffUsersPerformanceTime(string tenantGuid, string platformGuid, DateTime dateFrom, DateTime dateTo, bool isProviderUser, bool all = false)
        {
            string term = string.Empty;
            var auditLogs = (IEnumerable<Shared.Models.Infrastructure.AuditLog>)null;
            if (isProviderUser && all)
            {
                //term = $"%\"{nameof(AuditLogStaffUserTimePerformance.IsActive)}\":false%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
                auditLogs = await this.auditLogManager.GetByTermAndDateRange(AuditType.User, 
                                                                            AuditActionType.UserPerformanceActivity,  
                                                                            dateFrom, 
                                                                            dateTo,
                                                                            platformGuid,
                                                                            null,
                                                                            false);
            }
            else
            {
                //term = $"%\"{nameof(AuditLogStaffUserTimePerformance.IsActive)}\":false%\"{nameof(AuditLogStaffUserTimePerformance.TenantPlatformMapGuid)}\":\"{tenantGuid}\"" +
                //    $"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
                auditLogs = await this.auditLogManager.GetByTermAndDateRange(AuditType.User, 
                                                                             AuditActionType.UserPerformanceActivity, 
                                                                             dateFrom, 
                                                                             dateTo, 
                                                                             platformGuid, 
                                                                             tenantGuid, 
                                                                             false);
            }
            var result = new List<DomainModels.StaffUserPerformanceTime>();
            auditLogs.ToList().ForEach(m =>
            {
                if (!m.IsActive)
                {
                    if (result.Any(m1 => m1.Id == m.TypeId))
                    {
                        var staffUserPerformanceTime = result.Single(m1 => m1.Id == m.TypeId);
                        staffUserPerformanceTime.ActiveTime += m.ActiveTime;
                        staffUserPerformanceTime.IdleTime += m.IdleTime;
                        staffUserPerformanceTime.LogonTime += m.LogonTime;
                    }
                    else
                    {
                        var staffUserPerformanceTime = new DomainModels.StaffUserPerformanceTime
                        {
                            Id = m.TypeId,
                            ActiveTime = m.ActiveTime,
                            IdleTime = m.IdleTime,
                            LogonTime = m.LogonTime
                        };
                        result.Add(staffUserPerformanceTime);
                    }
                }
                //var performance = JsonConvert.DeserializeObject<AuditLogStaffUserTimePerformance>(m.Message);
                //if (!performance.IsActive)
                //{
                //    if (result.Any(m1 => m1.Id == m.TypeId))
                //    {
                //        var staffUserPerformanceTime = result.Single(m1 => m1.Id == m.TypeId);
                //        staffUserPerformanceTime.ActiveTime += performance.ActiveTime;
                //        staffUserPerformanceTime.IdleTime += performance.IdleTime;
                //        staffUserPerformanceTime.LogonTime += performance.LogonTime;
                //    }
                //    else
                //    {
                //        var staffUserPerformanceTime = new StaffUserPerformanceTime
                //        {
                //            Id = m.TypeId,
                //            ActiveTime = performance.ActiveTime,
                //            IdleTime = performance.IdleTime,
                //            LogonTime = performance.LogonTime
                //        };
                //        result.Add(staffUserPerformanceTime);
                //    }
                //}
            });

            return result;
        }

        public async Task<DomainModels.StaffUser> CreateStaffUser(DomainModels.StaffUser staffUserDto, DomainModels.StaffUser actionedByUser, int? tenantId, string platformGuid)
        {
            var handler = new CreateStaffUserHandler(staffUserRepository, roleRepository, auditLogManager);

            return await handler.Handle(staffUserDto, actionedByUser, tenantId, platformGuid);
        }

        public async Task<DomainModels.StaffUser> UpdateStaffUser(DomainModels.StaffUser staffUserDto, DomainModels.StaffUser staffUser, string platformGuid)
        {
            var handler = new UpdateStaffUserHandler(staffUserRepository, roleRepository, auditLogManager, logger);

            return await handler.Handle(staffUserDto, staffUser, platformGuid);
        }

        public async Task<DomainModels.StaffUser> BlockStaffUser(int userId, DomainModels.StaffUser actionedByUser, bool block, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new BlockStaffUserHandler(staffUserRepository, auditLogManager);

            return await handler.Handle(userId, actionedByUser, block, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<DomainModels.StaffUser> DeleteStaffUser(int userId, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new DeleteStaffUserHandler(staffUserRepository, auditLogManager);

            return await handler.Handle(userId, actionedByUser, tenantPlatformMapGuid, platformGuid);
        }

        public async Task LoginStaffUser(DomainModels.StaffUser staffUser, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new LoginStaffUserHandler(staffUserRepository, auditLogManager);

            await handler.Handle(staffUser, actionedByUser, tenantPlatformMapGuid, platformGuid);
        }

        public async Task LogoutStaffUser(DomainModels.StaffUser staffUser, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new LogoutStaffUserHandler(staffUserRepository, auditLogManager);

            await handler.Handle(staffUser, actionedByUser, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<DomainModels.StaffUserPerformanceTime> AddIdleMinutesStaffUser(int userId, int addIdleMinutes, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new AddIdleMinutesStaffUserHandler(staffUserRepository, auditLogManager);

            return await handler.Handle(userId, addIdleMinutes, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<DomainModels.StaffUserPerformanceTime> UpdateTrackingTimeStaffUser(int userId, string tenantPlatformMapGuid, string platformGuid)
        {
            var handler = new UpdateTrackingTimeStaffUserHandler(staffUserRepository, auditLogManager);

            return await handler.Handle(userId, tenantPlatformMapGuid, platformGuid);
        }

        public async Task<IEnumerable<DomainModels.StaffUser>> BlockAllTenantUsers(int tenantId, DomainModels.StaffUser actionedByUser, DomainModels.BlockAllUsersRequest request)
        {
            var handler = new BlockAllTenantUsersHandler(staffUserRepository, roleRepository, auditLogManager, logger);
            var staffUsers = await GetTenantUsers(request.PlatformGuid, tenantId, true, null, request.TenantPlatformMapGuid);
            return await handler.Handle(actionedByUser, staffUsers, request);
        }


    }
}

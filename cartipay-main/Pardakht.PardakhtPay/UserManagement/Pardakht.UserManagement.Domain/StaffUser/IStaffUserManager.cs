using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser
{
    public interface IStaffUserManager
    {
        List<Shared.Models.Infrastructure.Role> GetRoles();
        Task<Shared.Models.Infrastructure.StaffUser> GetByIdWithMappings(int id);
        Task<Shared.Models.Infrastructure.StaffUser> GetStaffUserByAccountId(string accountId);
        Task<DomainModels.StaffUser> CreateStaffUser(DomainModels.StaffUser staffUserDto, DomainModels.StaffUser actionedByUser, int? tenantId, string platformGuid);
        Task<DomainModels.StaffUser> UpdateStaffUser(DomainModels.StaffUser staffUserDto, DomainModels.StaffUser staffUser,  string platformGuid);
        Task<DomainModels.StaffUser> GetByAccountId(string id, string tenantPlatformMapGuid, string platformGuid, bool all = false);
        Task<DomainModels.StaffUser> GetByEmail(string email);
        Task<DomainModels.StaffUser> GetByUsername(string username);
        Task<DomainModels.StaffUser> GetById(int id, string platformGuid);
        Task<IEnumerable<DomainModels.StaffUser>> GetAllStaffUsers(string platformGuid);
        Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsers(string platformGuid);
        Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsers(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, int? brandId = null);
        Task<IEnumerable<DomainModels.StaffUser>> GetAllUsersWithPermission(string platformGuid, string permissionCode);
        Task<IEnumerable<DomainModels.StaffUser>> GetAllUsersWithPermissions(string platformGuid, IList<string> permissionCodes);
        Task<IEnumerable<DomainModels.StaffUser>> BlockAllTenantUsers(int tenantId, DomainModels.StaffUser actionedByUser, DomainModels.BlockAllUsersRequest request);
        Task<IEnumerable<DomainModels.StaffUser>> GetUsersWithPermission(string platformGuid, int? tenantId, string permissionCode, string tenantPlatformMapGuid);
        Task<IEnumerable<DomainModels.StaffUser>> GetUsersWithPermissions(string platformGuid, int? tenantId, IList<string> permissionCodes, string tenantPlatformMapGuid);
        Task<DomainModels.StaffUser> BlockStaffUser(int userId, DomainModels.StaffUser actionedByUser, bool block, string tenantPlatformMapGuid, string platformGuid);
        Task<DomainModels.StaffUser> DeleteStaffUser(int userId, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid);
        Task LoginStaffUser(DomainModels.StaffUser staffUser, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid);
        Task LogoutStaffUser(DomainModels.StaffUser staffUser, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid);
        Task<DomainModels.StaffUserPerformanceTime> AddIdleMinutesStaffUser(int userId, int addIdleMinutes, string tenantPlatformMapGuid, string platformGuid);
        Task<DomainModels.StaffUserPerformanceTime> UpdateTrackingTimeStaffUser(int userId, string tenantPlatformMapGuid, string platformGuid);
        Task<IEnumerable<DomainModels.StaffUserStatusLastLogin>> GetStaffUsersStatusLastLogin(string tenantGuid, string platformGuid, IEnumerable<int> staffUserIds, bool isProviderUser, bool all = false);
        Task<IEnumerable<DomainModels.StaffUserPerformanceTime>> GetStaffUsersPerformanceTime(string tenantGuid, string platformGuid, DateTime dateFrom, DateTime dateTo, bool isProviderUser, bool all = false);

        Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsersByUserTypeAsync(string platformGuid, Shared.Models.Infrastructure.UserType userType);
        Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsersByUserTypeAsync(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, Shared.Models.Infrastructure.UserType userType);
        Task<IEnumerable<DomainModels.StaffUser>> GetProviderUsersByUserTypeAsync(string platformGuid, Shared.Models.Infrastructure.UserType userType, int? brandId = null);
        Task<IEnumerable<DomainModels.StaffUser>> GetTenantUsersByUserTypeAsync(string platformGuid, int tenantId, bool allOwners, string accountGuid, string tenantPlatformMapGuid, Shared.Models.Infrastructure.UserType userType, int? brandId = null);
    }
}

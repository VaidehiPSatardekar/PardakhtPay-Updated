using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository
{
    public class ParadakhtUserManagementDbContext : DbContext
    {

        public ParadakhtUserManagementDbContext(DbContextOptions<ParadakhtUserManagementDbContext> options) : base(options) { }

        public ParadakhtUserManagementDbContext(string connectionString) : base(GetOptions(connectionString)) { }

        private static DbContextOptions GetOptions(string connectionString)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<ParadakhtUserManagementDbContext>();
            dbOptionsBuilder.UseSqlServer(connectionString);
            dbOptionsBuilder.EnableSensitiveDataLogging();

            return dbOptionsBuilder.Options;
        }

        public DbSet<StaffUser> StaffUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<UserPlatform> UserPlatforms { get; set; }
        public DbSet<UserPlatformRole> UserPlatformRoles { get; set; }
        public DbSet<UserSuspension> UserSuspensions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Permission>()
                .HasIndex(p => p.PlatformGuid);

            builder.Entity<Role>()
                .HasIndex(p => p.PlatformGuid);

            builder.Entity<StaffUser>()
                .HasIndex(u => u.AccountId);

            builder.Entity<StaffUser>()
                .HasIndex(u => u.TenantId);

            builder.Entity<StaffUser>()
                .HasIndex(u => u.ParentAccountId);

            builder.Entity<UserPlatform>()
                .HasIndex(up => new { up.StaffUserId, up.PlatformGuid })
                .IsUnique();

            builder.Entity<UserPlatform>()
                .HasOne(u => u.StaffUser)
                .WithMany(up => up.UserPlatforms)
                .HasForeignKey(u => u.StaffUserId);

            builder.Entity<UserPlatformRole>()
                .HasKey(upr => new { upr.UserPlatformId, upr.RoleId });

            builder.Entity<UserPlatformRole>()
                .HasOne(r => r.Role)
                .WithMany(upr => upr.UserPlatformRoles)
                .HasForeignKey(r => r.RoleId);

            builder.Entity<UserPlatformRole>()
                .HasOne(up => up.UserPlatform)
                .WithMany(upr => upr.UserPlatformRoles)
                .HasForeignKey(up => up.UserPlatformId);

            builder.Entity<StaffUser>()
                .HasMany(u => u.UserSuspensions)
                .WithOne(s => s.StaffUser)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<StaffUser>()
            //    .HasMany(u => u.UserSuspensions)
            //    .WithOne(s => s.StaffUser)
            //    .HasForeignKey(up => up.CreatedByUserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserSuspension>()
                .Property(u => u.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<AuditLog>()
                .HasIndex(p => p.PlatformGuid);

            builder.Entity<AuditLog>()
                .HasIndex(p => new { p.TenantPlatformMapGuid, p.PlatformGuid });

            builder.Entity<AuditLog>()
                .HasIndex(p => new { p.Type, p.TypeId, p.ActionType, p.PlatformGuid, p.TenantPlatformMapGuid });
        }
      
    }
}

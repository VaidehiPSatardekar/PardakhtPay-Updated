using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Migrations.ParadakhtUserManagementDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    PlatformGuid = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    PlatformGuid = table.Column<string>(type: "varchar(200)", nullable: false),
                    JwtKey = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsFixed = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    RoleHolderTypeId = table.Column<string>(type: "char(1)", nullable: false),
                    TenantGuid = table.Column<string>(type: "varchar(200)", nullable: true),
                    PlatformGuid = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "varchar(225)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(400)", nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ParentAccountId = table.Column<string>(type: "varchar(225)", nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    BrandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsRestricted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    PermissionGroupId = table.Column<int>(nullable: false),
                    PlatformGuid = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionGroups_PermissionGroupId",
                        column: x => x.PermissionGroupId,
                        principalTable: "PermissionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    IdleTime = table.Column<int>(nullable: false),
                    ActiveTime = table.Column<int>(nullable: false),
                    LogonTime = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    TenantPlatformMapGuid = table.Column<string>(maxLength: 100, nullable: true),
                    PlatformGuid = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_StaffUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffUserId = table.Column<int>(nullable: false),
                    PlatformGuid = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlatforms_StaffUsers_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSuspensions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSuspensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSuspensions_StaffUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSuspensions_StaffUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatformRoles",
                columns: table => new
                {
                    UserPlatformId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatformRoles", x => new { x.UserPlatformId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_UserPlatforms_UserPlatformId",
                        column: x => x.UserPlatformId,
                        principalTable: "UserPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PlatformGuid",
                table: "AuditLogs",
                column: "PlatformGuid");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantPlatformMapGuid_PlatformGuid",
                table: "AuditLogs",
                columns: new[] { "TenantPlatformMapGuid", "PlatformGuid" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Type_TypeId_ActionType_PlatformGuid_TenantPlatformMapGuid",
                table: "AuditLogs",
                columns: new[] { "Type", "TypeId", "ActionType", "PlatformGuid", "TenantPlatformMapGuid" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionGroupId",
                table: "Permissions",
                column: "PermissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PlatformGuid",
                table: "Permissions",
                column: "PlatformGuid");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_PlatformGuid",
                table: "Roles",
                column: "PlatformGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_AccountId",
                table: "StaffUsers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_ParentAccountId",
                table: "StaffUsers",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_TenantId",
                table: "StaffUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatformRoles_RoleId",
                table: "UserPlatformRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatforms_StaffUserId_PlatformGuid",
                table: "UserPlatforms",
                columns: new[] { "StaffUserId", "PlatformGuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSuspensions_CreatedByUserId",
                table: "UserSuspensions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSuspensions_UserId",
                table: "UserSuspensions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserPlatformRoles");

            migrationBuilder.DropTable(
                name: "UserSuspensions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserPlatforms");

            migrationBuilder.DropTable(
                name: "PermissionGroups");

            migrationBuilder.DropTable(
                name: "StaffUsers");
        }
    }
}

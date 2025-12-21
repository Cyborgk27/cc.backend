using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShowName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AuditCreateUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditCreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditUpdateUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditDeleteUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditDeleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ShowName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuditCreateUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditCreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditUpdateUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditDeleteUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditDeleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShowName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FeatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    AuditCreateUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditCreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditUpdateUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditDeleteUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditDeleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    EmailConfirmationToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditCreateUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditCreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditUpdateUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditDeleteUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditDeleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PermissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AuditCreateUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuditCreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditUpdateUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuditDeleteUser = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuditDeleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_Name",
                table: "Features",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_FeatureId",
                table: "Permissions",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Features");
        }
    }
}

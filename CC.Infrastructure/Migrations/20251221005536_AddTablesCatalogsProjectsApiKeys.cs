using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesCatalogsProjectsApiKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShowName = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsParent = table.Column<bool>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_Catalog_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Catalog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShowName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsIndefinite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowedIp = table.Column<string>(type: "TEXT", nullable: true),
                    AllowedDomain = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_ProjectApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectApiKeys_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CatalogId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_ProjectCatalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCatalogs_Catalog_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectCatalogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ParentId",
                table: "Catalog",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApiKeys_Key",
                table: "ProjectApiKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApiKeys_ProjectId",
                table: "ProjectApiKeys",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCatalogs_CatalogId",
                table: "ProjectCatalogs",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCatalogs_ProjectId",
                table: "ProjectCatalogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectApiKeys");

            migrationBuilder.DropTable(
                name: "ProjectCatalogs");

            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}

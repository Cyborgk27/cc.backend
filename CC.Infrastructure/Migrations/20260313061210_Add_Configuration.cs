using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuditCreateUser",
                table: "SystemAudits",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AuditDeleteUser",
                table: "SystemAudits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuditUpdateUser",
                table: "SystemAudits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SystemAudits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_CreatedAt",
                table: "SystemAudits",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_UserId",
                table: "SystemAudits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SystemAudits_CreatedAt",
                table: "SystemAudits");

            migrationBuilder.DropIndex(
                name: "IX_SystemAudits_UserId",
                table: "SystemAudits");

            migrationBuilder.DropColumn(
                name: "AuditCreateUser",
                table: "SystemAudits");

            migrationBuilder.DropColumn(
                name: "AuditDeleteUser",
                table: "SystemAudits");

            migrationBuilder.DropColumn(
                name: "AuditUpdateUser",
                table: "SystemAudits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SystemAudits");
        }
    }
}

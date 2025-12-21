using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_Catalog_ParentId",
                table: "Catalog");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCatalogs_Catalog_CatalogId",
                table: "ProjectCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalog",
                table: "Catalog");

            migrationBuilder.RenameTable(
                name: "Catalog",
                newName: "Catalogs");

            migrationBuilder.RenameIndex(
                name: "IX_Catalog_ParentId",
                table: "Catalogs",
                newName: "IX_Catalogs_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalogs",
                table: "Catalogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_Name",
                table: "Catalogs",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Catalogs_ParentId",
                table: "Catalogs",
                column: "ParentId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCatalogs_Catalogs_CatalogId",
                table: "ProjectCatalogs",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Catalogs_ParentId",
                table: "Catalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCatalogs_Catalogs_CatalogId",
                table: "ProjectCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalogs",
                table: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_Name",
                table: "Catalogs");

            migrationBuilder.RenameTable(
                name: "Catalogs",
                newName: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_Catalogs_ParentId",
                table: "Catalog",
                newName: "IX_Catalog_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalog",
                table: "Catalog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Catalog_ParentId",
                table: "Catalog",
                column: "ParentId",
                principalTable: "Catalog",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCatalogs_Catalog_CatalogId",
                table: "ProjectCatalogs",
                column: "CatalogId",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

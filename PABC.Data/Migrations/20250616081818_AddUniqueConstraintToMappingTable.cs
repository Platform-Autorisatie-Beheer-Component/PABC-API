using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId",
                table: "Mappings");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId",
                table: "Mappings",
                columns: new[] { "ApplicationRoleId", "DomainId", "FunctionalRoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId",
                table: "Mappings");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId",
                table: "Mappings",
                columns: new[] { "ApplicationRoleId", "DomainId", "FunctionalRoleId" });
        }
    }
}

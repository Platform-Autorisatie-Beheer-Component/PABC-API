using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueMappingIndexWhenNoDomainId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_mapping_application_role_id_domain_id_functional_role_id",
                table: "mapping");

            // Delete duplicate mappings where DomainId IS NULL, keep one
            migrationBuilder.Sql(@"
                DELETE FROM mapping 
                WHERE id NOT IN (
                    SELECT DISTINCT ON (application_role_id, functional_role_id) id
                    FROM mapping
                    WHERE domain_id IS NULL
                );
            ");

            migrationBuilder.CreateIndex(
                name: "ix_mapping_application_role_id_domain_id_functional_role_id",
                table: "mapping",
                columns: new[] { "application_role_id", "domain_id", "functional_role_id" },
                unique: true,
                filter: "\"domain_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_mapping_application_role_id_functional_role_id",
                table: "mapping",
                columns: new[] { "application_role_id", "functional_role_id" },
                unique: true,
                filter: "\"domain_id\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_mapping_application_role_id_domain_id_functional_role_id",
                table: "mapping");

            migrationBuilder.DropIndex(
                name: "ix_mapping_application_role_id_functional_role_id",
                table: "mapping");

            migrationBuilder.CreateIndex(
                name: "ix_mapping_application_role_id_domain_id_functional_role_id",
                table: "mapping",
                columns: new[] { "application_role_id", "domain_id", "functional_role_id" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class LoosenConstraintForMappingWithoutEntityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping",
                sql: "\"is_all_entity_types\" = false OR \"domain_id\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping",
                sql: "(\"is_all_entity_types\" = true AND \"domain_id\" IS NULL) OR (\"is_all_entity_types\" = false AND \"domain_id\" IS NOT NULL)");
        }
    }
}

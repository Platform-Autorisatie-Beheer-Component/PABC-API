using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCaseNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DomainEntityType_Domains_DomainId",
                table: "DomainEntityType");

            migrationBuilder.DropForeignKey(
                name: "FK_DomainEntityType_EntityTypes_EntityTypesId",
                table: "DomainEntityType");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_ApplicationRoles_ApplicationRoleId",
                table: "Mappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_FunctionalRoles_FunctionalRoleId",
                table: "Mappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mappings",
                table: "Mappings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Mapping_DomainId_IsAllEntityTypes",
                table: "Mappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionalRoles",
                table: "FunctionalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityTypes",
                table: "EntityTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Domains",
                table: "Domains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DomainEntityType",
                table: "DomainEntityType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationRoles",
                table: "ApplicationRoles");

            migrationBuilder.RenameTable(
                name: "Mappings",
                newName: "mapping");

            migrationBuilder.RenameTable(
                name: "FunctionalRoles",
                newName: "functional_role");

            migrationBuilder.RenameTable(
                name: "EntityTypes",
                newName: "entity_type");

            migrationBuilder.RenameTable(
                name: "Domains",
                newName: "domain");

            migrationBuilder.RenameTable(
                name: "DomainEntityType",
                newName: "domain_entity_type");

            migrationBuilder.RenameTable(
                name: "ApplicationRoles",
                newName: "application_role");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "mapping",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsAllEntityTypes",
                table: "mapping",
                newName: "is_all_entity_types");

            migrationBuilder.RenameColumn(
                name: "FunctionalRoleId",
                table: "mapping",
                newName: "functional_role_id");

            migrationBuilder.RenameColumn(
                name: "DomainId",
                table: "mapping",
                newName: "domain_id");

            migrationBuilder.RenameColumn(
                name: "ApplicationRoleId",
                table: "mapping",
                newName: "application_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_Mappings_FunctionalRoleId",
                table: "mapping",
                newName: "ix_mapping_functional_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_Mappings_DomainId",
                table: "mapping",
                newName: "ix_mapping_domain_id");

            migrationBuilder.RenameIndex(
                name: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId",
                table: "mapping",
                newName: "ix_mapping_application_role_id_domain_id_functional_role_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "functional_role",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "functional_role",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_FunctionalRoles_Name",
                table: "functional_role",
                newName: "ix_functional_role_name");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "entity_type",
                newName: "uri");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "entity_type",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "entity_type",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "entity_type",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EntityTypeId",
                table: "entity_type",
                newName: "entity_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_EntityTypes_Type_EntityTypeId",
                table: "entity_type",
                newName: "ix_entity_type_type_entity_type_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "domain",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "domain",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "domain",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_Name",
                table: "domain",
                newName: "ix_domain_name");

            migrationBuilder.RenameColumn(
                name: "EntityTypesId",
                table: "domain_entity_type",
                newName: "entity_types_id");

            migrationBuilder.RenameColumn(
                name: "DomainId",
                table: "domain_entity_type",
                newName: "domain_id");

            migrationBuilder.RenameIndex(
                name: "IX_DomainEntityType_EntityTypesId",
                table: "domain_entity_type",
                newName: "ix_domain_entity_type_entity_types_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "application_role",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Application",
                table: "application_role",
                newName: "application");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "application_role",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationRoles_Application_Name",
                table: "application_role",
                newName: "ix_application_role_application_name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mapping",
                table: "mapping",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_functional_role",
                table: "functional_role",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_entity_type",
                table: "entity_type",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_domain",
                table: "domain",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_domain_entity_type",
                table: "domain_entity_type",
                columns: new[] { "domain_id", "entity_types_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_application_role",
                table: "application_role",
                column: "id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping",
                sql: "(\"is_all_entity_types\" = true AND \"domain_id\" IS NULL) OR (\"is_all_entity_types\" = false AND \"domain_id\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "fk_domain_entity_type_domain_domain_id",
                table: "domain_entity_type",
                column: "domain_id",
                principalTable: "domain",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_domain_entity_type_entity_type_entity_types_id",
                table: "domain_entity_type",
                column: "entity_types_id",
                principalTable: "entity_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mapping_application_role_application_role_id",
                table: "mapping",
                column: "application_role_id",
                principalTable: "application_role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mapping_domain_domain_id",
                table: "mapping",
                column: "domain_id",
                principalTable: "domain",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_mapping_functional_role_functional_role_id",
                table: "mapping",
                column: "functional_role_id",
                principalTable: "functional_role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_domain_entity_type_domain_domain_id",
                table: "domain_entity_type");

            migrationBuilder.DropForeignKey(
                name: "fk_domain_entity_type_entity_type_entity_types_id",
                table: "domain_entity_type");

            migrationBuilder.DropForeignKey(
                name: "fk_mapping_application_role_application_role_id",
                table: "mapping");

            migrationBuilder.DropForeignKey(
                name: "fk_mapping_domain_domain_id",
                table: "mapping");

            migrationBuilder.DropForeignKey(
                name: "fk_mapping_functional_role_functional_role_id",
                table: "mapping");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mapping",
                table: "mapping");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Mapping_domain_id_is_all_entity_types",
                table: "mapping");

            migrationBuilder.DropPrimaryKey(
                name: "pk_functional_role",
                table: "functional_role");

            migrationBuilder.DropPrimaryKey(
                name: "pk_entity_type",
                table: "entity_type");

            migrationBuilder.DropPrimaryKey(
                name: "pk_domain_entity_type",
                table: "domain_entity_type");

            migrationBuilder.DropPrimaryKey(
                name: "pk_domain",
                table: "domain");

            migrationBuilder.DropPrimaryKey(
                name: "pk_application_role",
                table: "application_role");

            migrationBuilder.RenameTable(
                name: "mapping",
                newName: "Mappings");

            migrationBuilder.RenameTable(
                name: "functional_role",
                newName: "FunctionalRoles");

            migrationBuilder.RenameTable(
                name: "entity_type",
                newName: "EntityTypes");

            migrationBuilder.RenameTable(
                name: "domain_entity_type",
                newName: "DomainEntityType");

            migrationBuilder.RenameTable(
                name: "domain",
                newName: "Domains");

            migrationBuilder.RenameTable(
                name: "application_role",
                newName: "ApplicationRoles");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Mappings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_all_entity_types",
                table: "Mappings",
                newName: "IsAllEntityTypes");

            migrationBuilder.RenameColumn(
                name: "functional_role_id",
                table: "Mappings",
                newName: "FunctionalRoleId");

            migrationBuilder.RenameColumn(
                name: "domain_id",
                table: "Mappings",
                newName: "DomainId");

            migrationBuilder.RenameColumn(
                name: "application_role_id",
                table: "Mappings",
                newName: "ApplicationRoleId");

            migrationBuilder.RenameIndex(
                name: "ix_mapping_functional_role_id",
                table: "Mappings",
                newName: "IX_Mappings_FunctionalRoleId");

            migrationBuilder.RenameIndex(
                name: "ix_mapping_domain_id",
                table: "Mappings",
                newName: "IX_Mappings_DomainId");

            migrationBuilder.RenameIndex(
                name: "ix_mapping_application_role_id_domain_id_functional_role_id",
                table: "Mappings",
                newName: "IX_Mappings_ApplicationRoleId_DomainId_FunctionalRoleId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "FunctionalRoles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FunctionalRoles",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_functional_role_name",
                table: "FunctionalRoles",
                newName: "IX_FunctionalRoles_Name");

            migrationBuilder.RenameColumn(
                name: "uri",
                table: "EntityTypes",
                newName: "Uri");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "EntityTypes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "EntityTypes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EntityTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "entity_type_id",
                table: "EntityTypes",
                newName: "EntityTypeId");

            migrationBuilder.RenameIndex(
                name: "ix_entity_type_type_entity_type_id",
                table: "EntityTypes",
                newName: "IX_EntityTypes_Type_EntityTypeId");

            migrationBuilder.RenameColumn(
                name: "entity_types_id",
                table: "DomainEntityType",
                newName: "EntityTypesId");

            migrationBuilder.RenameColumn(
                name: "domain_id",
                table: "DomainEntityType",
                newName: "DomainId");

            migrationBuilder.RenameIndex(
                name: "ix_domain_entity_type_entity_types_id",
                table: "DomainEntityType",
                newName: "IX_DomainEntityType_EntityTypesId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Domains",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Domains",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Domains",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_domain_name",
                table: "Domains",
                newName: "IX_Domains_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ApplicationRoles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "application",
                table: "ApplicationRoles",
                newName: "Application");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ApplicationRoles",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_application_role_application_name",
                table: "ApplicationRoles",
                newName: "IX_ApplicationRoles_Application_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mappings",
                table: "Mappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionalRoles",
                table: "FunctionalRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityTypes",
                table: "EntityTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DomainEntityType",
                table: "DomainEntityType",
                columns: new[] { "DomainId", "EntityTypesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Domains",
                table: "Domains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationRoles",
                table: "ApplicationRoles",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Mapping_DomainId_IsAllEntityTypes",
                table: "Mappings",
                sql: "(\"IsAllEntityTypes\" = true AND \"DomainId\" IS NULL) OR (\"IsAllEntityTypes\" = false AND \"DomainId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_DomainEntityType_Domains_DomainId",
                table: "DomainEntityType",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DomainEntityType_EntityTypes_EntityTypesId",
                table: "DomainEntityType",
                column: "EntityTypesId",
                principalTable: "EntityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_ApplicationRoles_ApplicationRoleId",
                table: "Mappings",
                column: "ApplicationRoleId",
                principalTable: "ApplicationRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_FunctionalRoles_FunctionalRoleId",
                table: "Mappings",
                column: "FunctionalRoleId",
                principalTable: "FunctionalRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToCaseSensitiveCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop unique indexes that depend on the nl_case_insensitive collation.
            // These must be dropped before the collation can be removed.
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_application_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_application_role_application_id_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_domain_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_entity_type_type_entity_type_id;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_functional_role_name;");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "functional_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "entity_type_id",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "domain",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "domain",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "application",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");

            // Drop the collation definition after all columns have been altered
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:CollationDefinition:nl_case_insensitive", "nl-NL-u-ks-primary,nl-NL-u-ks-primary,icu,False");

            // Recreate unique indexes with default (case-sensitive) collation
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_application_name ON application (name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_application_role_application_id_name ON application_role (application_id, name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_domain_name ON domain (name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_entity_type_type_entity_type_id ON entity_type (type, entity_type_id);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_functional_role_name ON functional_role (name);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Guard: fail fast if case-variant duplicates exist that would violate
            // the case-insensitive unique constraints after rollback.
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM application GROUP BY lower(name) HAVING count(*) > 1
                        UNION ALL
                        SELECT 1 FROM application_role GROUP BY application_id, lower(name) HAVING count(*) > 1
                        UNION ALL
                        SELECT 1 FROM domain GROUP BY lower(name) HAVING count(*) > 1
                        UNION ALL
                        SELECT 1 FROM entity_type GROUP BY lower(type), lower(entity_type_id) HAVING count(*) > 1
                        UNION ALL
                        SELECT 1 FROM functional_role GROUP BY lower(name) HAVING count(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Cannot revert to case-insensitive: duplicate values differing only in casing exist. Resolve duplicates manually before rolling back.';
                    END IF;
                END $$;
            ");

            // Drop case-sensitive indexes before altering columns
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_application_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_application_role_application_id_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_domain_name;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_entity_type_type_entity_type_id;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_functional_role_name;");

            // Recreate the collation definition first (columns will reference it)
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:nl_case_insensitive", "nl-NL-u-ks-primary,nl-NL-u-ks-primary,icu,False");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "functional_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "entity_type_id",
                table: "entity_type",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "domain",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "domain",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "application",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            // Recreate indexes (they inherit the collation from the columns)
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_application_name ON application (name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_application_role_application_id_name ON application_role (application_id, name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_domain_name ON domain (name);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_entity_type_type_entity_type_id ON entity_type (type, entity_type_id);");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ix_functional_role_name ON functional_role (name);");
        }
    }
}

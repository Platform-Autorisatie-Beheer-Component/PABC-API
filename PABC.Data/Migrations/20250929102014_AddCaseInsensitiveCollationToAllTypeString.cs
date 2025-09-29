using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseInsensitiveCollationToAllTypeString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                type: "text",
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "text");

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
                name: "application",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
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
                name: "application",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldCollation: "nl_case_insensitive");
        }
    }
}

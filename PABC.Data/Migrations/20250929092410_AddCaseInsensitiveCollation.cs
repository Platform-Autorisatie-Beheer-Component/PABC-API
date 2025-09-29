using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseInsensitiveCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:CollationDefinition:nl_case_insensitive", "nl-NL-u-ks-primary,nl-NL-u-ks-primary,icu,False");

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
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);
        }
    }
}

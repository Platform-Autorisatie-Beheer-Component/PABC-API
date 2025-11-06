using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, collation: "nl_case_insensitive")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_name",
                table: "application",
                column: "name",
                unique: true);

            // Migrate existing application names to application table
            migrationBuilder.Sql(@"
                INSERT INTO application (id, name)
                SELECT gen_random_uuid() AS id, application AS name
                FROM application_role
                GROUP BY application
                ORDER BY application;
            ");

            // Add temporarily nullable application_id column to application_role
            migrationBuilder.AddColumn<Guid>(
                name: "application_id",
                table: "application_role",
                type: "uuid",
                nullable: true);

            // Populate application_id with matching application.id
            migrationBuilder.Sql(@"
                UPDATE application_role ar
                SET application_id = a.id
                FROM application a
                WHERE ar.application = a.name;
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "application_id",
                table: "application_role",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropIndex(
                name: "ix_application_role_application_name",
                table: "application_role");

            migrationBuilder.CreateIndex(
                name: "ix_application_role_application_id_name",
                table: "application_role",
                columns: new[] { "application_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_application_role_application_application_id",
                table: "application_role",
                column: "application_id",
                principalTable: "application",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
                name: "application",
                table: "application_role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back temporarily nullable application column
            migrationBuilder.AddColumn<string>(
                name: "application",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                collation: "nl_case_insensitive");

            // Restore application names from application table
            migrationBuilder.Sql(@"
                UPDATE application_role ar
                SET application = a.name
                FROM application a
                WHERE ar.application_id = a.id;
            ");

            // Make application column non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "application",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                collation: "nl_case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldCollation: "nl_case_insensitive");

            migrationBuilder.DropForeignKey(
                name: "fk_application_role_application_application_id",
                table: "application_role");

            migrationBuilder.DropIndex(
                name: "ix_application_role_application_id_name",
                table: "application_role");

            migrationBuilder.DropColumn(
                name: "application_id",
                table: "application_role");

            migrationBuilder.CreateIndex(
                name: "ix_application_role_application_name",
                table: "application_role",
                columns: new[] { "application", "name" },
                unique: true);

            migrationBuilder.DropTable(
                name: "application");
        }
    }
}

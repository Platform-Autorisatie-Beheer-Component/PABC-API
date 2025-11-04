using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationRoleApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_application_role_application_name",
                table: "application_role");

            migrationBuilder.DropColumn(
                name: "application",
                table: "application_role");

            migrationBuilder.AddColumn<Guid>(
                name: "application_id",
                table: "application_role",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_application_role_application_application_id",
                table: "application_role");

            migrationBuilder.DropIndex(
                name: "ix_application_role_application_id_name",
                table: "application_role");

            migrationBuilder.DropColumn(
                name: "application_id",
                table: "application_role");

            migrationBuilder.AddColumn<string>(
                name: "application",
                table: "application_role",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                collation: "nl_case_insensitive");

            migrationBuilder.CreateIndex(
                name: "ix_application_role_application_name",
                table: "application_role",
                columns: new[] { "application", "name" },
                unique: true);
        }
    }
}

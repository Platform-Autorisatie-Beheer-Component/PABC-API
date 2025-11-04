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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application");
        }
    }
}

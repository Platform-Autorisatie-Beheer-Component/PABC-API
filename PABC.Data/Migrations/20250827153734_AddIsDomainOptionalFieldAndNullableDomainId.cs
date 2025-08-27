using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PABC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDomainOptionalFieldAndNullableDomainId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings");

            migrationBuilder.AlterColumn<Guid>(
                name: "DomainId",
                table: "Mappings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "IsDomainOptional",
                table: "Mappings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Mapping_DomainId_IsDomainOptional",
                table: "Mappings",
                sql: "(\"IsDomainOptional\" = true AND \"DomainId\" IS NULL) OR (\"IsDomainOptional\" = false AND \"DomainId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Mapping_DomainId_IsDomainOptional",
                table: "Mappings");

            migrationBuilder.DropColumn(
                name: "IsDomainOptional",
                table: "Mappings");

            migrationBuilder.AlterColumn<Guid>(
                name: "DomainId",
                table: "Mappings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Domains_DomainId",
                table: "Mappings",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

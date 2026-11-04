using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalNote",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedAt",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RetiredAt",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RetiredBy",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RetirementNote",
                schema: "Ectm",
                table: "ConfigurationDocuments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalNote",
                schema: "Ectm",
                table: "ConfigurationDocuments");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                schema: "Ectm",
                table: "ConfigurationDocuments");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                schema: "Ectm",
                table: "ConfigurationDocuments");

            migrationBuilder.DropColumn(
                name: "RetiredAt",
                schema: "Ectm",
                table: "ConfigurationDocuments");

            migrationBuilder.DropColumn(
                name: "RetiredBy",
                schema: "Ectm",
                table: "ConfigurationDocuments");

            migrationBuilder.DropColumn(
                name: "RetirementNote",
                schema: "Ectm",
                table: "ConfigurationDocuments");
        }
    }
}

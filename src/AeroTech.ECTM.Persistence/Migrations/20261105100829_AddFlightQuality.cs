using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightQualities",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    QualityRulesDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    QualityRulesVersion = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    TimeTrust = table.Column<int>(type: "int", nullable: true),
                    PartialStart = table.Column<bool>(type: "bit", nullable: true),
                    PartialEnd = table.Column<bool>(type: "bit", nullable: true),
                    DiscontinuityCount = table.Column<int>(type: "int", nullable: false),
                    SpanCount = table.Column<int>(type: "int", nullable: false),
                    EvaluatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightQualities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightQualities_FlightId_QualityRulesDocumentId",
                schema: "Ectm",
                table: "FlightQualities",
                columns: new[] { "FlightId", "QualityRulesDocumentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightQualities_FlightId_Status",
                schema: "Ectm",
                table: "FlightQualities",
                columns: new[] { "FlightId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightQualities",
                schema: "Ectm");
        }
    }
}

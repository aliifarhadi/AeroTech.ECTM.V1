using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisRevisionsAndPhases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightQualities_FlightId_QualityRulesDocumentId",
                schema: "Ectm",
                table: "FlightQualities");

            migrationBuilder.AddColumn<long>(
                name: "AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightQualities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AnalysisRevisions",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Trigger = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DataProfileDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DataProfileVersion = table.Column<int>(type: "int", nullable: false),
                    ParameterCatalogDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCatalogVersion = table.Column<int>(type: "int", nullable: false),
                    QualityRulesDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    QualityRulesVersion = table.Column<int>(type: "int", nullable: false),
                    PhaseRuleSetDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    PhaseRuleSetVersion = table.Column<int>(type: "int", nullable: true),
                    ProcessingJobId = table.Column<long>(type: "bigint", nullable: true),
                    FailureCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SupersededAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisRevisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightPhaseAssessments",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisRevisionId = table.Column<long>(type: "bigint", nullable: false),
                    PhaseRuleSetDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    PhaseRuleSetVersion = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AlgorithmVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NotEvaluableReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SegmentCount = table.Column<int>(type: "int", nullable: false),
                    UnknownSampleCount = table.Column<int>(type: "int", nullable: false),
                    EventSpanCount = table.Column<int>(type: "int", nullable: false),
                    EvaluatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPhaseAssessments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightQualities_AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightQualities",
                column: "AnalysisRevisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisRevisions_FlightId_Number",
                schema: "Ectm",
                table: "AnalysisRevisions",
                columns: new[] { "FlightId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisRevisions_FlightId_Status",
                schema: "Ectm",
                table: "AnalysisRevisions",
                columns: new[] { "FlightId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FlightPhaseAssessments_AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightPhaseAssessments",
                column: "AnalysisRevisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightPhaseAssessments_FlightId_Status",
                schema: "Ectm",
                table: "FlightPhaseAssessments",
                columns: new[] { "FlightId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisRevisions",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightPhaseAssessments",
                schema: "Ectm");

            migrationBuilder.DropIndex(
                name: "IX_FlightQualities_AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightQualities");

            migrationBuilder.DropColumn(
                name: "AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightQualities");

            migrationBuilder.CreateIndex(
                name: "IX_FlightQualities_FlightId_QualityRulesDocumentId",
                schema: "Ectm",
                table: "FlightQualities",
                columns: new[] { "FlightId", "QualityRulesDocumentId" },
                unique: true);
        }
    }
}

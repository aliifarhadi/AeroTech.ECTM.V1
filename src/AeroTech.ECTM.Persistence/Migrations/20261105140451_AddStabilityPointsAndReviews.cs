using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStabilityPointsAndReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EngineAssignments",
                schema: "Ectm",
                table: "AnalysisRevisions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SnapshotRulesDocumentId",
                schema: "Ectm",
                table: "AnalysisRevisions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SnapshotRulesVersion",
                schema: "Ectm",
                table: "AnalysisRevisions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlightReviews",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisRevisionId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    DecidedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReasonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OpenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightStabilityPoints",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisRevisionId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotRulesDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotRulesVersion = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NotEvaluableReason = table.Column<int>(type: "int", nullable: true),
                    EvaluatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    PhaseMetrics = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightStabilityPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightReviewEntries",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightReviewId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Actor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReasonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FlightStateAfter = table.Column<int>(type: "int", nullable: false),
                    At = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightReviewEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightReviewEntries_FlightReviews_FlightReviewId",
                        column: x => x.FlightReviewId,
                        principalSchema: "Ectm",
                        principalTable: "FlightReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightReviewEntries_At",
                schema: "Ectm",
                table: "FlightReviewEntries",
                column: "At");

            migrationBuilder.CreateIndex(
                name: "IX_FlightReviewEntries_FlightReviewId",
                schema: "Ectm",
                table: "FlightReviewEntries",
                column: "FlightReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightReviews_AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightReviews",
                column: "AnalysisRevisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightReviews_FlightId_State",
                schema: "Ectm",
                table: "FlightReviews",
                columns: new[] { "FlightId", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_FlightStabilityPoints_AnalysisRevisionId",
                schema: "Ectm",
                table: "FlightStabilityPoints",
                column: "AnalysisRevisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightStabilityPoints_FlightId_Status",
                schema: "Ectm",
                table: "FlightStabilityPoints",
                columns: new[] { "FlightId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightReviewEntries",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightStabilityPoints",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightReviews",
                schema: "Ectm");

            migrationBuilder.DropColumn(
                name: "EngineAssignments",
                schema: "Ectm",
                table: "AnalysisRevisions");

            migrationBuilder.DropColumn(
                name: "SnapshotRulesDocumentId",
                schema: "Ectm",
                table: "AnalysisRevisions");

            migrationBuilder.DropColumn(
                name: "SnapshotRulesVersion",
                schema: "Ectm",
                table: "AnalysisRevisions");
        }
    }
}

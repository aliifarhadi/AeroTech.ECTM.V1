using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBaselines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaselineParameterFlights",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BaselineId = table.Column<long>(type: "bigint", nullable: false),
                    BaselineParameterId = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    FlightDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AnalysisRevisionId = table.Column<long>(type: "bigint", nullable: false),
                    FlightStabilityPointsId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: false),
                    SnapshotKind = table.Column<int>(type: "int", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Statistic = table.Column<int>(type: "int", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SnapshotRulesDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotRulesVersion = table.Column<int>(type: "int", nullable: false),
                    SourceStartIndex = table.Column<int>(type: "int", nullable: true),
                    SourceEndIndexInclusive = table.Column<int>(type: "int", nullable: true),
                    SourcePeakIndex = table.Column<int>(type: "int", nullable: true),
                    RawObserved = table.Column<double>(type: "float", nullable: true),
                    BaselineRawValue = table.Column<double>(type: "float", nullable: false),
                    RawDelta = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reasons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CalculationVersion = table.Column<int>(type: "int", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CalculatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineParameterFlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaselinePrimarySelections",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotKind = table.Column<int>(type: "int", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ParameterCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    BaselineId = table.Column<long>(type: "bigint", nullable: false),
                    BaselineParameterId = table.Column<long>(type: "bigint", nullable: false),
                    SelectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SelectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselinePrimarySelections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Baselines",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    LineageId = table.Column<long>(type: "bigint", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    PreviousBaselineId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    DraftRevision = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeclaredEffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ShopVisitEventId = table.Column<long>(type: "bigint", nullable: true),
                    FirstPostShopDeclared = table.Column<bool>(type: "bit", nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    SourceFileSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RetiredBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RetiredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RetirementReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baselines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaselineParameters",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BaselineId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotKind = table.Column<int>(type: "int", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Statistic = table.Column<int>(type: "int", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RawValue = table.Column<double>(type: "float", nullable: true),
                    SuggestedRawValue = table.Column<double>(type: "float", nullable: true),
                    ValueOrigin = table.Column<int>(type: "int", nullable: false),
                    SourceEnginePosition = table.Column<byte>(type: "tinyint", nullable: true),
                    SourceStartIndex = table.Column<int>(type: "int", nullable: true),
                    SourceEndIndexInclusive = table.Column<int>(type: "int", nullable: true),
                    SourcePeakIndex = table.Column<int>(type: "int", nullable: true),
                    ReferenceContext = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true),
                    CatalogDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogVersion = table.Column<int>(type: "int", nullable: false),
                    SnapshotRulesDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotRulesVersion = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineParameters_Baselines_BaselineId",
                        column: x => x.BaselineId,
                        principalSchema: "Ectm",
                        principalTable: "Baselines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaselineParameterFlights_BaselineParameterId_FlightId",
                schema: "Ectm",
                table: "BaselineParameterFlights",
                columns: new[] { "BaselineParameterId", "FlightId" },
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineParameterFlights_EngineId_FlightDate",
                schema: "Ectm",
                table: "BaselineParameterFlights",
                columns: new[] { "EngineId", "FlightDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BaselineParameterFlights_FlightId_IsCurrent",
                schema: "Ectm",
                table: "BaselineParameterFlights",
                columns: new[] { "FlightId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_BaselineParameters_BaselineId_SnapshotKind_Selector_ParameterCode",
                schema: "Ectm",
                table: "BaselineParameters",
                columns: new[] { "BaselineId", "SnapshotKind", "Selector", "ParameterCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaselinePrimarySelections_BaselineId",
                schema: "Ectm",
                table: "BaselinePrimarySelections",
                column: "BaselineId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselinePrimarySelections_EngineId_SnapshotKind_Selector_ParameterCode",
                schema: "Ectm",
                table: "BaselinePrimarySelections",
                columns: new[] { "EngineId", "SnapshotKind", "Selector", "ParameterCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baselines_EngineId_State",
                schema: "Ectm",
                table: "Baselines",
                columns: new[] { "EngineId", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_Baselines_LineageId_RevisionNumber",
                schema: "Ectm",
                table: "Baselines",
                columns: new[] { "LineageId", "RevisionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaselineParameterFlights",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "BaselineParameters",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "BaselinePrimarySelections",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "Baselines",
                schema: "Ectm");
        }
    }
}

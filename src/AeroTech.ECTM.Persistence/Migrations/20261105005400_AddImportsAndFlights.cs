using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImportsAndFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SourceFileId = table.Column<long>(type: "bigint", nullable: false),
                    ImportBatchId = table.Column<long>(type: "bigint", nullable: false),
                    ImportItemId = table.Column<long>(type: "bigint", nullable: false),
                    AircraftId = table.Column<long>(type: "bigint", nullable: true),
                    Registration = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RegistrationSource = table.Column<int>(type: "int", nullable: true),
                    FlightDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FlightDateSource = table.Column<int>(type: "int", nullable: true),
                    FlightNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    FlightNumberSource = table.Column<int>(type: "int", nullable: true),
                    IdentityStatus = table.Column<int>(type: "int", nullable: false),
                    IdentityReasons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistinctFlightConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IdentityNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    StateBeforeHold = table.Column<int>(type: "int", nullable: true),
                    HoldNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HeldAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DataProfileDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DataProfileVersion = table.Column<int>(type: "int", nullable: false),
                    ParameterCatalogDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCatalogVersion = table.Column<int>(type: "int", nullable: false),
                    TimeParameterKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SampleCount = table.Column<int>(type: "int", nullable: false),
                    FirstElapsed = table.Column<double>(type: "float", nullable: true),
                    LastElapsed = table.Column<double>(type: "float", nullable: true),
                    BlockingCount = table.Column<int>(type: "int", nullable: false),
                    WarningCount = table.Column<int>(type: "int", nullable: false),
                    InfoCount = table.Column<int>(type: "int", nullable: false),
                    ImportedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportBatches",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    DataProfileDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DataProfileVersion = table.Column<int>(type: "int", nullable: false),
                    ParameterCatalogDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCatalogVersion = table.Column<int>(type: "int", nullable: false),
                    DefaultRegistration = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DefaultYear = table.Column<int>(type: "int", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    JobId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CommittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingJobs",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Processed = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LeaseExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceFiles",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    StoredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightChannels",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: true),
                    ProfileOrdinal = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ValueKind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Encoding = table.Column<int>(type: "int", nullable: false),
                    SampleCount = table.Column<int>(type: "int", nullable: false),
                    MissingCount = table.Column<int>(type: "int", nullable: false),
                    UnparseableCount = table.Column<int>(type: "int", nullable: false),
                    OutOfRangeCount = table.Column<int>(type: "int", nullable: false),
                    PayloadSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightChannels_Flights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "Ectm",
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightEngineAssignments",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: true),
                    InstallationId = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightEngineAssignments", x => x.Id);
                    table.CheckConstraint("CK_FlightEngineAssignments_Position", "[Position] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_FlightEngineAssignments_Flights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "Ectm",
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightValidationIssues",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ParameterKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Count = table.Column<long>(type: "bigint", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightValidationIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightValidationIssues_Flights_FlightId",
                        column: x => x.FlightId,
                        principalSchema: "Ectm",
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportItems",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ImportBatchId = table.Column<long>(type: "bigint", nullable: false),
                    Ordinal = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ContainerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SourceFileId = table.Column<long>(type: "bigint", nullable: true),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    RejectionCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExistingFlightId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<long>(type: "bigint", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Preview = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportItems_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalSchema: "Ectm",
                        principalTable: "ImportBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SourceFileContents",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CompressedBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceFileContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceFileContents_SourceFiles_Id",
                        column: x => x.Id,
                        principalSchema: "Ectm",
                        principalTable: "SourceFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightChannelPayloads",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightChannelPayloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightChannelPayloads_FlightChannels_Id",
                        column: x => x.Id,
                        principalSchema: "Ectm",
                        principalTable: "FlightChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightChannels_FlightId_ParameterKey",
                schema: "Ectm",
                table: "FlightChannels",
                columns: new[] { "FlightId", "ParameterKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightEngineAssignments_EngineId",
                schema: "Ectm",
                table: "FlightEngineAssignments",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightEngineAssignments_FlightId_Position",
                schema: "Ectm",
                table: "FlightEngineAssignments",
                columns: new[] { "FlightId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AircraftId_FlightDate_FlightNumber",
                schema: "Ectm",
                table: "Flights",
                columns: new[] { "AircraftId", "FlightDate", "FlightNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ImportBatchId",
                schema: "Ectm",
                table: "Flights",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_SourceFileId",
                schema: "Ectm",
                table: "Flights",
                column: "SourceFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_State",
                schema: "Ectm",
                table: "Flights",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_FlightValidationIssues_FlightId",
                schema: "Ectm",
                table: "FlightValidationIssues",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_CreatedAt",
                schema: "Ectm",
                table: "ImportBatches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ImportItems_FlightId",
                schema: "Ectm",
                table: "ImportItems",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportItems_ImportBatchId_Ordinal",
                schema: "Ectm",
                table: "ImportItems",
                columns: new[] { "ImportBatchId", "Ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportItems_SourceFileId",
                schema: "Ectm",
                table: "ImportItems",
                column: "SourceFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_State_CreatedAt",
                schema: "Ectm",
                table: "ProcessingJobs",
                columns: new[] { "State", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SourceFiles_Sha256",
                schema: "Ectm",
                table: "SourceFiles",
                column: "Sha256",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightChannelPayloads",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightEngineAssignments",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightValidationIssues",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "ImportItems",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "ProcessingJobs",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "SourceFileContents",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "FlightChannels",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "ImportBatches",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "SourceFiles",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "Flights",
                schema: "Ectm");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFleetRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "RegistryVersions",
                schema: "Ectm");

            migrationBuilder.CreateTable(
                name: "Aircraft",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Registration = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    TypeCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    OperatorCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EngineCount = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineInstallations",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    AircraftId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<byte>(type: "tinyint", nullable: false),
                    InstalledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Evidence = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineInstallations", x => x.Id);
                    table.CheckConstraint("CK_EngineInstallations_Interval", "[RemovedAt] IS NULL OR [RemovedAt] > [InstalledAt]");
                    table.CheckConstraint("CK_EngineInstallations_Position", "[Position] IN (1, 2)");
                });

            migrationBuilder.CreateTable(
                name: "EngineModels",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Manufacturer = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Family = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    VariantCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ThrustRatingCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DocumentReferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Engines",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EngineModelId = table.Column<long>(type: "bigint", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VariantCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ThrustRatingCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ConfigurationEvidenceReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConfigurationDocumentedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ConfigurationDocumentedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HoursSinceNew = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CyclesSinceNew = table.Column<int>(type: "int", nullable: true),
                    HoursSinceLastEvent = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CyclesSinceLastEvent = table.Column<int>(type: "int", nullable: true),
                    LifeCountersBasisAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LifeCountersSource = table.Column<int>(type: "int", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceEvents",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    IdentityKind = table.Column<int>(type: "int", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: true),
                    AircraftId = table.Column<long>(type: "bigint", nullable: true),
                    Position = table.Column<byte>(type: "tinyint", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    EventAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateReliability = table.Column<int>(type: "int", nullable: false),
                    HoursAtEvent = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CyclesAtEvent = table.Column<int>(type: "int", nullable: true),
                    WorkOrderReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceEvents", x => x.Id);
                    table.CheckConstraint("CK_MaintenanceEvents_Identity", "([IdentityKind] = 1 AND [EngineId] IS NOT NULL AND [AircraftId] IS NULL AND [Position] IS NULL) OR ([IdentityKind] = 2 AND [EngineId] IS NULL AND [AircraftId] IS NOT NULL AND [Position] IN (1, 2))");
                });

            migrationBuilder.CreateTable(
                name: "RegistryChanges",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    RegistryVersion = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR [Ectm].[RegistryVersions]"),
                    InstallationId = table.Column<long>(type: "bigint", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistryChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistryChanges_EngineInstallations_InstallationId",
                        column: x => x.InstallationId,
                        principalSchema: "Ectm",
                        principalTable: "EngineInstallations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineLifeCounterAdjustments",
                schema: "Ectm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    HoursSinceNew = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CyclesSinceNew = table.Column<int>(type: "int", nullable: true),
                    HoursSinceLastEvent = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CyclesSinceLastEvent = table.Column<int>(type: "int", nullable: true),
                    BasisAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AdjustedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AdjustedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineLifeCounterAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineLifeCounterAdjustments_Engines_EngineId",
                        column: x => x.EngineId,
                        principalSchema: "Ectm",
                        principalTable: "Engines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_Registration",
                schema: "Ectm",
                table: "Aircraft",
                column: "Registration",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineInstallations_AircraftId_Position",
                schema: "Ectm",
                table: "EngineInstallations",
                columns: new[] { "AircraftId", "Position" },
                unique: true,
                filter: "[RemovedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EngineInstallations_AircraftId_Position_InstalledAt",
                schema: "Ectm",
                table: "EngineInstallations",
                columns: new[] { "AircraftId", "Position", "InstalledAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineInstallations_EngineId",
                schema: "Ectm",
                table: "EngineInstallations",
                column: "EngineId",
                unique: true,
                filter: "[RemovedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EngineInstallations_EngineId_InstalledAt",
                schema: "Ectm",
                table: "EngineInstallations",
                columns: new[] { "EngineId", "InstalledAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineLifeCounterAdjustments_EngineId",
                schema: "Ectm",
                table: "EngineLifeCounterAdjustments",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineModels_Code",
                schema: "Ectm",
                table: "EngineModels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Engines_EngineModelId",
                schema: "Ectm",
                table: "Engines",
                column: "EngineModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_Manufacturer_SerialNumber",
                schema: "Ectm",
                table: "Engines",
                columns: new[] { "Manufacturer", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceEvents_AircraftId_Position",
                schema: "Ectm",
                table: "MaintenanceEvents",
                columns: new[] { "AircraftId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceEvents_EngineId",
                schema: "Ectm",
                table: "MaintenanceEvents",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistryChanges_InstallationId",
                schema: "Ectm",
                table: "RegistryChanges",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistryChanges_RegistryVersion",
                schema: "Ectm",
                table: "RegistryChanges",
                column: "RegistryVersion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aircraft",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "EngineLifeCounterAdjustments",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "EngineModels",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "MaintenanceEvents",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "RegistryChanges",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "Engines",
                schema: "Ectm");

            migrationBuilder.DropTable(
                name: "EngineInstallations",
                schema: "Ectm");

            migrationBuilder.DropSequence(
                name: "RegistryVersions",
                schema: "Ectm");
        }
    }
}

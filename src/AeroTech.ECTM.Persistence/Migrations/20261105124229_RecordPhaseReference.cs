using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.ECTM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RecordPhaseReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlgorithmVersion",
                schema: "Ectm",
                table: "FlightPhaseAssessments",
                newName: "ReferenceAlgorithm");

            migrationBuilder.AddColumn<int>(
                name: "ReferenceMatch",
                schema: "Ectm",
                table: "FlightPhaseAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceMatch",
                schema: "Ectm",
                table: "FlightPhaseAssessments");

            migrationBuilder.RenameColumn(
                name: "ReferenceAlgorithm",
                schema: "Ectm",
                table: "FlightPhaseAssessments",
                newName: "AlgorithmVersion");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyTreatmentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "PatientTrackings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "PatientTrackings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}

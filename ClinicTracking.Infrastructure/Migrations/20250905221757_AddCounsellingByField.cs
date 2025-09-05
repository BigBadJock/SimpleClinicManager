using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCounsellingByField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CounsellingBy",
                table: "PatientTrackings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounsellingBy",
                table: "PatientTrackings");
        }
    }
}

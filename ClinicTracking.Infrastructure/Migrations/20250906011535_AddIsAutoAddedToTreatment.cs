using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAutoAddedToTreatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAdded",
                table: "Treatments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoAdded",
                table: "Treatments");
        }
    }
}

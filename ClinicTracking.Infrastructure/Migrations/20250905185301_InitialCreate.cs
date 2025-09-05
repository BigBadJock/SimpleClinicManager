using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MRN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReferralDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CounsellingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelayReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SurveyReturned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEnglishFirstLanguage = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Treatment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Adjuvant = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Palliative = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DispensedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImagingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultsDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextCycleDue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextAppointment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTrackings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientTrackings_MRN",
                table: "PatientTrackings",
                column: "MRN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientTrackings");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentLookupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TreatmentId",
                table: "PatientTrackings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Treatments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientTrackings_TreatmentId",
                table: "PatientTrackings",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_Name",
                table: "Treatments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTrackings_Treatments_TreatmentId",
                table: "PatientTrackings",
                column: "TreatmentId",
                principalTable: "Treatments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Seed some common treatments
            migrationBuilder.InsertData(
                table: "Treatments",
                columns: new[] { "Id", "Name", "Description", "CreatedBy", "CreatedOn" },
                values: new object[,]
                {
                    { Guid.Parse("11111111-1111-1111-1111-111111111111"), "Chemotherapy", "Standard chemotherapy treatment", "System", DateTime.UtcNow },
                    { Guid.Parse("22222222-2222-2222-2222-222222222222"), "Radiation Therapy", "Radiation treatment", "System", DateTime.UtcNow },
                    { Guid.Parse("33333333-3333-3333-3333-333333333333"), "Immunotherapy", "Immune system enhancement therapy", "System", DateTime.UtcNow },
                    { Guid.Parse("44444444-4444-4444-4444-444444444444"), "Targeted Therapy", "Targeted drug therapy", "System", DateTime.UtcNow },
                    { Guid.Parse("55555555-5555-5555-5555-555555555555"), "Hormone Therapy", "Hormonal treatment", "System", DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTrackings_Treatments_TreatmentId",
                table: "PatientTrackings");

            migrationBuilder.DropTable(
                name: "Treatments");

            migrationBuilder.DropIndex(
                name: "IX_PatientTrackings_TreatmentId",
                table: "PatientTrackings");

            migrationBuilder.DropColumn(
                name: "TreatmentId",
                table: "PatientTrackings");
        }
    }
}
